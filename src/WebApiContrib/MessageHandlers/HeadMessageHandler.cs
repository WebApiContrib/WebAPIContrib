using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContrib.MessageHandlers {

    public class HeadMessageHandler : DelegatingHandler {
        

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            if (request.Method == HttpMethod.Head) {
                request.Method = HttpMethod.Get;
                return base.SendAsync(request, cancellationToken)
                    .ContinueWith<HttpResponseMessage>(task => {
                        var response = task.Result;
                        response.RequestMessage.Method = HttpMethod.Head;
                        if (response.Content != null)
                            response.Content = new HeadContent(response.Content);
                        return task.Result;
                    });

            }

            return base.SendAsync(request, cancellationToken);
        }
    }

    internal class HeadContent : HttpContent {

        public HeadContent(HttpContent content) {
            CopyHeaders(content.Headers, Headers);
        }

        protected override Task SerializeToStreamAsync(
                                                Stream stream,
                                                TransportContext context) {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
        }


        protected override bool TryComputeLength(out long length) {
            length = -1;
            return false;
        }

        private static void CopyHeaders(HttpContentHeaders fromHeaders,
                                        HttpContentHeaders toHeaders) {

            foreach (KeyValuePair<string, IEnumerable<string>> header in fromHeaders) {
                toHeaders.Add(header.Key, header.Value);
            }
        }
    }
}
