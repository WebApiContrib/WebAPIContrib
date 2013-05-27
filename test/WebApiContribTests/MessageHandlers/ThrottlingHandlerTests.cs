using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NUnit.Framework;
using Should;
using WebApiContrib.Caching;
using WebApiContrib.MessageHandlers;

namespace WebApiContribTests.MessageHandlers
{
    [TestFixture]
    public class ThrottlingHandlerTests : MessageHandlerTester
    {
        [Test]
        public void Should_inject_ratelimit_headers_when_limit_not_reached()
        {
            var handler = GetHandler(100, TimeSpan.FromMinutes(1));

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            var response = ExecuteRequest(handler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.OK);

            IEnumerable<string> values;
            Assert.True(response.Headers.TryGetValues("RateLimit-Limit", out values));
            Assert.AreEqual("100", values.First());
            Assert.True(response.Headers.TryGetValues("RateLimit-Remaining", out values));
            Assert.AreEqual("99", values.First());
        }

        [Test]
        public void Should_throttle_when_limit_reached()
        {
            var handler = GetHandler(0, TimeSpan.FromMinutes(1));

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "foo/bar");
            var response = ExecuteRequest(handler, requestMessage);

            response.StatusCode.ShouldEqual(HttpStatusCode.Conflict);

            IEnumerable<string> values;
            Assert.True(response.Headers.TryGetValues("RateLimit-Limit", out values));
            Assert.AreEqual("0", values.First());
            Assert.True(response.Headers.TryGetValues("RateLimit-Remaining", out values));
            Assert.AreEqual("0", values.First());
        }

        private ThrottlingHandler GetHandler(long maxRequests, TimeSpan period)
        {
            return new ThrottlingHandlerWithFixedIdentifier(new InMemoryThrottleStore(), identifier => maxRequests, period);
        }

        private class ThrottlingHandlerWithFixedIdentifier: ThrottlingHandler
        {
            public ThrottlingHandlerWithFixedIdentifier(IThrottleStore store, Func<string, long> maxRequestsForUserIdentifier, TimeSpan period) : base(store, maxRequestsForUserIdentifier, period)
            {
            }

            protected override string GetUserIdentifier(HttpRequestMessage request)
            {
                return "10.0.0.1";
            }
        }
    }
}