using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO.Compression;
using System.IO;

namespace Thinktecture.Web.Http.Handlers
{
    public class EncodingHandler : DelegatingHandler
    {
        public EncodingHandler()
        {
        }

        public EncodingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var contentEncodingHeader = "Content-Encoding";
            var encoding = "gzip";

            if (!request.Content.Headers.Contains(contentEncodingHeader))
            {
                request.Content = new CompressedContent(request.Content, encoding);
            }
            else
            {
                var incomingStream = request.Content.ReadAsStreamAsync().Result;
                // we would need to store the decompressed stream
                var outputStream = new MemoryStream(4096);

                using (var decompressedStream = new GZipStream(incomingStream, CompressionMode.Decompress, leaveOpen: false))
                {
                    decompressedStream.CopyTo(outputStream);
                }
                
                outputStream.Position = 0;
                request.Content = new StreamContent(outputStream);
            }

            return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((responseToCompleteTask) =>
            {
                var response = responseToCompleteTask.Result;

                if (response.RequestMessage.Headers.AcceptEncoding != null && response.RequestMessage.Headers.AcceptEncoding.Count > 0)
                {
                    var encodingType = response.RequestMessage.Headers.AcceptEncoding.First().Value;

                    response.Content = new CompressedContent(response.Content, encodingType);
                }

                return response;
            },
            TaskContinuationOptions.OnlyOnRanToCompletion);            
        }
    }
}
