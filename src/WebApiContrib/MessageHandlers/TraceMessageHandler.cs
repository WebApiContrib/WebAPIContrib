using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContrib.MessageHandlers {
    public class TraceMessageHandler : DelegatingHandler {
        public TraceMessageHandler(DelegatingHandler innerChannel)
            : base(innerChannel) {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

                   if (request.Method == HttpMethod.Trace) {
                       return Task<HttpResponseMessage>.Factory.StartNew(
                           () => {
                               var response = new HttpResponseMessage(HttpStatusCode.OK);
                               response.Content = new StringContent(request.ToString(), Encoding.UTF8, "message/http");
                               return response;
                           });
                    }
 
                    return base.SendAsync(request, cancellationToken);
        }
    }


    

}