using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebApiContrib.Internal;
using WebApiContrib.ResponseMessages;
using WebApiContrib.Internal.Extensions;

namespace WebApiContrib.Caching
{
	/// <summary>
	/// Represents a message handler that implements caching and supports
	/// (loosely based on Glenn Block's ETagHandler)
	/// * Resource retrieval by ETag
	/// * Resource retrieval by LastModified
	/// * If-Match and If-None-Match for GET operations
	/// * If-Modified-Since and If-Unmodified-Since for GET operations
	/// * If-Unmodified-Since and If-Match for PUT operations
	/// * Will add ETag, LastModified and Vary headers in the response
	/// * Allows caching to be turned off based on individual message
	/// * Currently does not support If-Range headers
	/// </summary>
    public class CachingHandler : DelegatingHandler
    {
		protected readonly IEntityTagStore _entityTagStore;
		private readonly string[] _varyByHeaders;
		private object _padLock = new object();


		/// <summary>
		/// A Chain of responsibility of rules for handling various scenarios. 
		/// List is ordered. First one to return a non-null task will break the chain and 
		/// method will return
		/// </summary>
		protected IDictionary<string, Func<HttpRequestMessage, Task<HttpResponseMessage>>> RequestInterceptionRules { get; set; }


		public bool AddLastModifiedHeader { get; set; }

		public bool AddVaryHeader { get; set; }

		public CachingHandler(params string[] varyByHeader)
			: this(new InMemoryEntityTagStore(), varyByHeader)
		{
			
		}

		public CachingHandler(IEntityTagStore entityTagStore, params string[] varyByHeaders)
		{
			AddLastModifiedHeader = true;
			AddVaryHeader = true;
			_varyByHeaders = varyByHeaders;
			_entityTagStore = entityTagStore;
			ETagValueGenerator = (resourceUri, headers) => 
				new EntityTagHeaderValue(
					string.Format("\"{0}\"", Guid.NewGuid().ToString("N").ToLower()),
				varyByHeaders.Length ==0); // default ETag generation will create weak tags if varyByHeaders has zero items

			EntityTagKeyGenerator = (resourceUri, headers) =>
				new EntityTagKey(resourceUri, headers.SelectMany(h => h.Value));
			CacheController = (request) => new CacheControlHeaderValue()
				{
					Private = true, 
					MustRevalidate = true, 
					NoTransform = true,
					MaxAge = TimeSpan.FromDays(7)
				};
		}

		/// <summary>
		/// A function which receives URL of the resource and generates a unique value for ETag
		/// It also receives varyByHeaders request headers.
		/// Default value is a function that generates a guid and URL is ignored and
		/// it generates a weak ETag if no varyByHeaders is passed in
		/// </summary>
		public Func<string, IEnumerable<KeyValuePair<string, IEnumerable<string>>>, 
			EntityTagHeaderValue> ETagValueGenerator { get; set; }

		/// <summary>
		/// A function which receives URL of the resource and generates a value for ETag key
		/// It also receives varyByHeaders request headers.
		/// Default value is a function that appends URL and all varyByHeader header values.
		/// This extensibility points allows for selected values from the varyByHeader headers
		/// selected and passed in.
		/// </summary>
		public Func<string, IEnumerable<KeyValuePair<string, IEnumerable<string>>>, EntityTagKey>
			 EntityTagKeyGenerator { get; set; }

		/// <summary>
		/// This is a function that decides whether caching for a particular request
		/// is supported.
		/// Function can return null to negate any caching. In this case, responses will not be cached
		/// and ETag header will not be sent.
		/// Alternatively it can return a CacheControlHeaderValue which controls cache lifetime on the client.
		/// By default value is set so that all requests are cachable with expiry of 1 week.
		/// </summary>
		public Func<HttpRequestMessage, CacheControlHeaderValue> CacheController { get; set; }

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			EnusreRulesSetup();

			var varyByHeaders = request.Headers.Where(h => _varyByHeaders.Any(
				v => v.Equals(h.Key, StringComparison.CurrentCultureIgnoreCase)));

			Task<HttpResponseMessage> task = null;
		
			RequestInterceptionRules.Values.FirstOrDefault(r =>
			{
				task = r(request);
				return task != null;
			});				
	

			if (task == null)
				return base.SendAsync(request, cancellationToken)
					.ContinueWith(GetCachingContinuation(request));
			else
				return task;
		}

		internal Func<Task<HttpResponseMessage>, HttpResponseMessage> GetCachingContinuation(HttpRequestMessage request)
		{
			return task =>
			{
			    var httpResponse = task.Result;
				var cacheControlHeaderValue = CacheController(request);
				if (cacheControlHeaderValue == null)
					return httpResponse;

				string uri = request.RequestUri.ToString();
			    var varyHeaders = request.Headers.Where(h => 
					_varyByHeaders.Any(v => v.Equals(h.Key, StringComparison.CurrentCultureIgnoreCase)));

				var eTagKey = EntityTagKeyGenerator(uri, varyHeaders);
			    
				TimedEntityTagHeaderValue eTagValue;

			    if (!_entityTagStore.TryGet(eTagKey, out eTagValue) || request.Method == HttpMethod.Put ||
			       	request.Method == HttpMethod.Post)
			    {
					eTagValue = new TimedEntityTagHeaderValue(ETagValueGenerator(uri, varyHeaders));
			       	_entityTagStore.AddOrUpdate(eTagKey, eTagValue );
			    }

				// set ETag
			    httpResponse.Headers.ETag = eTagValue.ToEntityTagHeaderValue();
				
				// set last-modified
				if (AddLastModifiedHeader && httpResponse.Content != null && !httpResponse.Content.Headers.Any(x=>x.Key.Equals(HttpHeaderNames.LastModified, 
					StringComparison.CurrentCultureIgnoreCase)))
				{
					httpResponse.Content.Headers.Add(HttpHeaderNames.LastModified, eTagValue.LastModified.ToString("r"));
				}

				// set Vary
				if(AddVaryHeader && _varyByHeaders!=null && _varyByHeaders.Length>0)
				{
					httpResponse.Headers.Add(HttpHeaderNames.Vary, _varyByHeaders);
				}

				httpResponse.Headers.AddWithoutValidation(HttpHeaderNames.CacheControl,cacheControlHeaderValue.ToString());

				return httpResponse;
			};
		}

		private void EnsureRulesSetup()
		{
			if (RequestInterceptionRules == null)
			{
				lock (_padLock)
				{
					if (RequestInterceptionRules == null) // double if to prevent race condition
					{
						BuildRules();
					}
				}
			}
		}

		
		protected virtual void BuildRules()
		{
			RequestInterceptionRules = new Dictionary<string, Func<HttpRequestMessage, Task<HttpResponseMessage>>>();
			RequestInterceptionRules.Add("GetIfMatchNoneMatch", GetIfMatchNoneMatch());
			RequestInterceptionRules.Add("GetIfModifiedUnmodifiedSince", GetIfModifiedUnmodifiedSince());
			RequestInterceptionRules.Add("PutIfMatch", PutIfMatch());
			RequestInterceptionRules.Add("PutIfUnmodifiedSince", PutIfUnmodifiedSince());

		}

		internal Func<HttpRequestMessage, Task<HttpResponseMessage>> GetIfMatchNoneMatch()
		{
			return (request) =>
			{
				if (request.Method != HttpMethod.Get)
					return null;

				ICollection<EntityTagHeaderValue> noneMatchTags = request.Headers.IfNoneMatch;
				ICollection<EntityTagHeaderValue> matchTags = request.Headers.IfMatch;

				if(matchTags.Count==0 && noneMatchTags.Count==0)
					return null; // no etag

				if (matchTags.Count > 0 && noneMatchTags.Count > 0) // both if-match and if-none-match exist
					return new HttpResponseMessage(HttpStatusCode.BadRequest).ToTask();

				var isNoneMatch = noneMatchTags.Count > 0;
				var etags = isNoneMatch ? noneMatchTags : matchTags;

				var resource = request.RequestUri.ToString();
				var headers =
					request.Headers.Where(h => _varyByHeaders.Any(v => v.Equals(h.Key, StringComparison.CurrentCultureIgnoreCase)));
				var entityTagKey = EntityTagKeyGenerator(resource, headers);
				// compare the Etag with the one in the cache
				// do conditional get.
				TimedEntityTagHeaderValue actualEtag = null;

				bool matchFound = false;
				if (_entityTagStore.TryGet(entityTagKey, out actualEtag))
				{
					if (etags.Any(etag => etag.Tag == actualEtag.Tag))
					{
						matchFound = true;
					}
				}				
				return matchFound ^ isNoneMatch ? null : new NotModifiedResponse(actualEtag).ToTask();
			};
			
		}
		
		internal Func<HttpRequestMessage, Task<HttpResponseMessage>> GetIfModifiedUnmodifiedSince()
		{
			return (request) =>
			{
			    if (request.Method != HttpMethod.Get)
			       	return null;

			    DateTimeOffset? ifModifiedSince = request.Headers.IfModifiedSince;
			    DateTimeOffset? ifUnmodifiedSince = request.Headers.IfUnmodifiedSince;

			    if (ifModifiedSince == null && ifUnmodifiedSince == null)
			       	return null; // no etag

			    if (ifModifiedSince != null && ifUnmodifiedSince != null) // both exist
			       	return new HttpResponseMessage(HttpStatusCode.BadRequest).ToTask();
			    bool ifModified = (ifUnmodifiedSince == null);
			    DateTimeOffset modifiedInQuestion = ifModified ? ifModifiedSince.Value : ifUnmodifiedSince.Value;

				var headers =
					request.Headers.Where(h => _varyByHeaders.Any(v => v.Equals(h.Key, StringComparison.CurrentCultureIgnoreCase)));
				var resource = request.RequestUri.ToString();
				var entityTagKey = EntityTagKeyGenerator(resource, headers);

			    TimedEntityTagHeaderValue actualEtag = null;

			    bool isModified = false;
			    if (_entityTagStore.TryGet(entityTagKey, out actualEtag))
			    {
			       	isModified = actualEtag.LastModified > modifiedInQuestion;
			    }

			    return isModified ^ ifModified
			       		? new NotModifiedResponse(actualEtag).ToTask()
			       		: null;

			};
		}

		internal Func<HttpRequestMessage, Task<HttpResponseMessage>> PutIfUnmodifiedSince()
		{
			return (request) =>
			{
				if (request.Method != HttpMethod.Put)
					return null;

				DateTimeOffset? ifUnmodifiedSince = request.Headers.IfUnmodifiedSince;
				if (ifUnmodifiedSince == null)
					return null;

				DateTimeOffset modifiedInQuestion = ifUnmodifiedSince.Value;

				var headers =
					request.Headers.Where(h => _varyByHeaders.Any(v => v.Equals(h.Key, StringComparison.CurrentCultureIgnoreCase)));
				var resource = request.RequestUri.ToString();
				var entityTagKey = EntityTagKeyGenerator(resource, headers);
				TimedEntityTagHeaderValue actualEtag = null;

				bool isModified = false;
				if (_entityTagStore.TryGet(entityTagKey, out actualEtag))
				{
					isModified = actualEtag.LastModified > modifiedInQuestion;
				}
				
				return isModified ? new HttpResponseMessage(HttpStatusCode.PreconditionFailed).ToTask()
					: null;

			};
		}
		internal Func<HttpRequestMessage, Task<HttpResponseMessage>> PutIfMatch()
		{
			return (request) =>
			{
				if (request.Method != HttpMethod.Put)
					return null;

				ICollection<EntityTagHeaderValue> matchTags = request.Headers.IfMatch;
				if (matchTags == null || matchTags.Count == 0)
					return null;

				var headers =
					request.Headers.Where(h => _varyByHeaders.Any(v => v.Equals(h.Key, StringComparison.CurrentCultureIgnoreCase)));
				var resource = request.RequestUri.ToString();
				var entityTagKey = EntityTagKeyGenerator(resource, headers);
				TimedEntityTagHeaderValue actualEtag = null;

				bool matchFound = false;
				if (_entityTagStore.TryGet(entityTagKey, out actualEtag))
				{
					if (matchTags.Any(etag => etag.Tag == actualEtag.Tag))
					{
						matchFound = true;
					}
				}

				return matchFound ? null
					: new HttpResponseMessage(HttpStatusCode.PreconditionFailed).ToTask();

			};
		}

	}
}
