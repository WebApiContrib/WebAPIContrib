using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using WebApiContrib.Caching;
using WebApiContrib.Internal;

namespace WebApiContribTests.Caching
{
	public static class CachingHandlerTests
	{
		private const string TestUrl = "http://myserver/api/stuff/";
		private static readonly string[] EtagValues = new[] { "abcdefgh", "12345678" };

		[TestCase("GET", true, true, true, false, new []{"Accept", "Accept-Language"})]
		[TestCase("PUT", true, true, false, false, new[] { "Accept", "Accept-Language" })]
		[TestCase("POST", true, false, true, true, new[] { "Accept", "Accept-Language" })]
		[TestCase("POST", false, true, false, true, new[] { "Accept", "Accept-Language" })]
		public static void TestCachingContinuation(
			string method,
			bool existsInStore,
			bool addVaryHeader,
			bool addLastModifiedHeader,
			bool alreadyHasLastModified,
			string[] varyByHeader)
		{

			// setup 
			var mocks = new MockRepository();
			var request = new HttpRequestMessage(new HttpMethod(method), TestUrl);
			var entityTagStore = mocks.StrictMock<IEntityTagStore>();
			var cachingHandler = new CachingHandler(entityTagStore, varyByHeader)
			                     	{
			                     		AddLastModifiedHeader = addLastModifiedHeader,
										AddVaryHeader = addVaryHeader
			                     	};

			var entityTagHeaderValue = new TimedEntityTagHeaderValue("\"12345678\"");
			var entityTagKey = new EntityTagKey(TestUrl, new string[0]);
			cachingHandler.EntityTagKeyGenerator = (x, y) => entityTagKey;
			cachingHandler.ETagValueGenerator = (x, y) => new EntityTagHeaderValue(entityTagHeaderValue.Tag);
			entityTagStore.Expect(x => x.TryGet(Arg<EntityTagKey>.Matches(etg => etg.ResourceUri == TestUrl),
				out Arg<TimedEntityTagHeaderValue>.Out(entityTagHeaderValue).Dummy)).Return(existsInStore);

			if (!existsInStore  || request.Method == HttpMethod.Put || request.Method == HttpMethod.Post)
			{
				entityTagStore.Expect(
					x => x.AddOrUpdate(Arg<EntityTagKey>.Matches(etk => etk == entityTagKey),
						Arg<TimedEntityTagHeaderValue>.Matches(ethv => ethv.Tag == entityTagHeaderValue.Tag)));
			}

			mocks.ReplayAll();

			var response = new HttpResponseMessage();
			response.Content = new ByteArrayContent(new byte[0]);
			if(alreadyHasLastModified)
				response.Content.Headers.Add(HttpHeaderNames.LastModified, DateTimeOffset.Now.ToString("r"));

			var cachingContinuation = cachingHandler.GetCachingContinuation(request);
			var taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();
			taskCompletionSource.SetResult(response);

			// run
			var httpResponseMessage = cachingContinuation(taskCompletionSource.Task);

			// test
			if(addLastModifiedHeader)
			{
				Assert.That(httpResponseMessage.Content.Headers.Any(x=>x.Key == HttpHeaderNames.LastModified),
					"LastModified does not exist");	
			}
			if(!addLastModifiedHeader && !alreadyHasLastModified)
			{
				Assert.That(!httpResponseMessage.Content.Headers.Any(x => x.Key == HttpHeaderNames.LastModified),
					"LastModified exists");					
			}

		}
	}
}
