using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Should;
using WebApiContrib.MessageHandlers;

namespace WebApiContribTests.MessageHandlers
{
    [TestFixture]
    public class ETagHandlerTests : MessageHandlerTester
    {
        private KeyValuePair<string, EntityTagHeaderValue> etag = new KeyValuePair<string, EntityTagHeaderValue>("foo/bar", new EntityTagHeaderValue("\"" + Guid.NewGuid() + "\""));

        [Test]
        public void Should_return_NotModified_if_ETag_is_found_in_the_cache()
        {
            var eTagHandler = GetHandler();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, etag.Key);
            requestMessage.Headers.Add("If-None-Match", etag.Value.Tag);
            AddETagValue(etag);
            var response = ExecuteRequest(eTagHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.NotModified);
        }

        [Test]
        public void Should_return_Conflict_for_Put_if_key_is_in_header_but_match_not_found_in_cache()
        {
            var eTagHandler = GetHandler();

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, etag.Key);
            requestMessage.Headers.Add("If-Match", etag.Value.Tag);

            var newRandomValue = new EntityTagHeaderValue("\"" + Guid.NewGuid() + "\"");
            AddETagValue(new KeyValuePair<string, EntityTagHeaderValue>(etag.Key, newRandomValue));

            var response = ExecuteRequest(eTagHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.Conflict);
        }

        [Test]
        public void Post_should_return_OK_and_ETag_in_Header_and_update_Cache_if_ETag_is_found_in_the_cache()
        {
            var eTagHandler = GetHandler();

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "foo/bar");
            requestMessage.Headers.Add("If-None-Match", etag.Value.Tag);
            AddETagValue(etag);
            ETagHandler.ETagCache.Count.ShouldEqual(1);
            var response = ExecuteRequest(eTagHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            response.Headers.ETag.ShouldNotBeNull();
            ETagHandler.ETagCache.Count.ShouldEqual(1);
        }

        [Test]
        public void Get_should_return_OK_and_ETag_in_Header_and_add_to_Cache_if_ETag_is_not_found_in_the_cache()
        {
            var eTagHandler = GetHandler();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            requestMessage.Headers.Add("If-None-Match", etag.Value.Tag);
            // not added to cache
            ETagHandler.ETagCache.Count.ShouldEqual(0);
            var response = ExecuteRequest(eTagHandler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            response.Headers.ETag.ShouldNotBeNull();
            ETagHandler.ETagCache.Count.ShouldEqual(1);
        }

        private static void AddETagValue(KeyValuePair<string, EntityTagHeaderValue> pair)
        {
            ETagHandler.ETagCache.AddOrUpdate(pair.Key, pair.Value, (key, val) => pair.Value);
        }

        private ETagHandler GetHandler()
        {
            return new ETagHandler();
        }
    }
}