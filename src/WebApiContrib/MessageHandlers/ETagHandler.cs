using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebApiContrib.ResponseMessages;

namespace WebApiContrib.MessageHandlers
{
    // Applied from http://codepaste.net/4w6c6i - by Glenn Block
    public class ETagHandler : DelegatingHandler
    {
        public static ConcurrentDictionary<string, EntityTagHeaderValue> ETagCache = new ConcurrentDictionary<string, EntityTagHeaderValue>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                // should we ignore trailing slash
                var resource = request.RequestUri.ToString();

                ICollection<EntityTagHeaderValue> etags = request.Headers.IfNoneMatch;
                // compare the Etag with the one in the cache
                // do conditional get.
                EntityTagHeaderValue actualEtag = null;
                if (etags.Count > 0 && ETagHandler.ETagCache.TryGetValue(resource, out actualEtag))
                {
                    if (etags.Any(etag => etag.Tag == actualEtag.Tag))
                    {
                        return Task.Factory.StartNew<HttpResponseMessage>(task => new NotModifiedResponse(), cancellationToken);
                    }
                }
            }
            else if (request.Method == HttpMethod.Put)
            {
                // should we ignore trailing slash
                var resource = request.RequestUri.ToString();

                ICollection<EntityTagHeaderValue> etags = request.Headers.IfMatch;
                // compare the Etag with the one in the cache
                // do conditional get.

                EntityTagHeaderValue actualEtag = null;
                if (etags.Count > 0 && ETagHandler.ETagCache.TryGetValue(resource, out actualEtag))
                {
                    var matchFound = etags.Any(etag => etag.Tag == actualEtag.Tag);

                    if (!matchFound)
                    {
                        return Task.Factory.StartNew<HttpResponseMessage>(task => new ConflictResponse(), cancellationToken);
                    }
                }
            }
            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                var httpResponse = task.Result;
                var eTagKey = request.RequestUri.ToString();
                EntityTagHeaderValue eTagValue;

                // Post would invalidate the collection, put should invalidate the individual item
                if (!ETagCache.TryGetValue(eTagKey, out eTagValue) || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
                {
                    eTagValue = new EntityTagHeaderValue("\"" + Guid.NewGuid().ToString() + "\"");
                    ETagCache.AddOrUpdate(eTagKey, eTagValue, (key, existingVal) => eTagValue);
                }
                httpResponse.Headers.ETag = eTagValue;

                return httpResponse;
            });
        }
    }
}