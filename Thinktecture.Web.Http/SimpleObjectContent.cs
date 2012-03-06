using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Thinktecture.Web.Http
{
    // Code based on: https://gist.github.com/1651087
    public class SimpleObjectContent<T> : HttpContent
    {
        private readonly T outboundInstance;
        private readonly HttpContent inboundContent;
        private readonly MediaTypeFormatter formatter;

        // Outbound constructor
        public SimpleObjectContent(T outboundInstance, MediaTypeFormatter formatter)
        {
            this.outboundInstance = outboundInstance;
            this.formatter = formatter;
        }

        //Inbound constructor
        public SimpleObjectContent(HttpContent inboundContent, MediaTypeFormatter formatter)
        {
            this.inboundContent = inboundContent;
            this.formatter = formatter;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            // FormatterContext is required by XmlMediaTypeFormatter, but is not used by WriteToStreamAsync of XmlMediaTypeFormatter!
            return formatter.WriteToStreamAsync(typeof(T), outboundInstance, stream, Headers, new FormatterContext(new MediaTypeHeaderValue("application/bogus"), false), null);
        }

        public Task<T> ReadAsync()
        {
            return ReadAsStreamAsync()
                .ContinueWith<object>(streamTask => formatter.ReadFromStreamAsync(typeof(T), streamTask.Result, inboundContent.Headers, new FormatterContext(new MediaTypeHeaderValue("application/bogus"), false)))
                .ContinueWith<T>(objectTask => (T)((Task<object>)(objectTask.Result)).Result);
        }

        protected override Task<Stream> CreateContentReadStreamAsync()
        {
            return inboundContent.ReadAsStreamAsync();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            return false;
        }
    }
}
