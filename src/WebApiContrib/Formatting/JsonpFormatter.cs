using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace WebApiContrib.Formatting
{
    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private readonly HttpRequestMessage request;
        private string callbackQueryParameter;

        public JsonpMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(DefaultMediaType);
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));

            MediaTypeMappings.Add(new UriPathExtensionMapping("jsonp", DefaultMediaType));
        }

        public JsonpMediaTypeFormatter(HttpRequestMessage request)
            : this()
        {
            this.request = request;
        }

        public string CallbackQueryParameter
        {
            get { return callbackQueryParameter ?? "callback"; }
            set { callbackQueryParameter = value; }
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (request == null)
                throw new ArgumentNullException("request");

            return new JsonpMediaTypeFormatter(request);
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            string callback;
            if (IsJsonpRequest(request, out callback))
            {
                return Task.Factory.StartNew(() =>
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(callback + "(");
                    writer.Flush();

                    base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext).ContinueWith(_ =>
                    {
                        writer.Write(")");
                        writer.Flush();
                    });
                });
            }

            return base.WriteToStreamAsync(type, value, stream, contentHeaders, transportContext);
        }

        private bool IsJsonpRequest(HttpRequestMessage request, out string callback)
        {
            callback = null;

            if (request == null || request.Method != HttpMethod.Get)
            {
                return false;
            }

            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            callback = query[CallbackQueryParameter];

            return !string.IsNullOrEmpty(callback);
        }
    }
}
