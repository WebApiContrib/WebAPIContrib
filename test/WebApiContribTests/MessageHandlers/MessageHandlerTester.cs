using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContribTests.MessageHandlers
{
    public class MessageHandlerTester : TestBase
    {
        protected HttpResponseMessage ExecuteRequest(DelegatingHandler handlerBeingTested, HttpRequestMessage requestMessage)
        {
            var executionHelper = new AsyncExecutionHelper();
            return executionHelper.ExecuteRequest(handlerBeingTested, requestMessage);
        }

        private class AsyncExecutionHelper : DelegatingHandler
        {
            public HttpResponseMessage ExecuteRequest(DelegatingHandler testTarget, HttpRequestMessage requestMessage)
            {
                testTarget.InnerHandler = new OKHandler();
                InnerHandler = testTarget;

                var requestTask = SendAsync(requestMessage, new CancellationToken());

                requestTask.Wait(5000); // 5 second timeout - tests should be quicker than this, but better than infinite for now

                return requestTask.Result;
            }

            private class OKHandler : DelegatingHandler
            {
                protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                {
                    return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK));
                }
            }
        }
        
    }
}