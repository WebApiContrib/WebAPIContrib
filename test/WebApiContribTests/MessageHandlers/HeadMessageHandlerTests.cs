using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApiContrib.MessageHandlers;

namespace WebApiContribTests.MessageHandlers
{

    [TestFixture]
    public class HeadMessageHandlerTests
    {

        [Test]
        public void Should_return_headers_and_no_body_when_method_is_head()
        {
            var messageHandler = new HeadMessageHandler();
            messageHandler.InnerHandler = new PrecannedMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
                                                                          {
                                                                              ReasonPhrase = "Boo",
                                                                              Content = new StringContent("Hello world")
                                                                          });

            var requestMessage = new HttpRequestMessage(HttpMethod.Head, "http://foo/bar");
            
            var client = new HttpClient(messageHandler);

            var response = client.SendAsync(requestMessage).Result;

            Assert.AreEqual("Boo",response.ReasonPhrase);
            Assert.AreEqual(0, response.Content.Headers.ContentLength);
        }

        [Test]
        public void Should_do_nothing_when_method_is_get()
        {
            var messageHandler = new HeadMessageHandler();
            messageHandler.InnerHandler = new PrecannedMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
            {
                ReasonPhrase = "Boo",
                Content = new StringContent("Hello world")
            });

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://foo/bar");

            var client = new HttpClient(messageHandler);

            var response = client.SendAsync(requestMessage).Result;

            Assert.AreEqual("Boo", response.ReasonPhrase);
            Assert.AreEqual("Hello world", response.Content.ReadAsStringAsync().Result);
        }
    }

    public class PrecannedMessageHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _response;

        public PrecannedMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Method == HttpMethod.Get)
            {
                _response.RequestMessage = request;
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(_response);
                return tcs.Task;
            } 
                else
            {
                return null;
            }
        }
    }
}
