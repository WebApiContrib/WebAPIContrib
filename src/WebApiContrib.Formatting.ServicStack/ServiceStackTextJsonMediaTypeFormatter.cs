using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using WebApiContrib.Internal;

namespace WebApiContrib.Formatting
{
    public class ServiceStackTextJsonMediaTypeFormatter : MediaTypeFormatter
    {
        //Uses ISO8601 date by default
        private JsonDateHandler _dateHandler = JsonDateHandler.ISO8601;

        public ServiceStackTextJsonMediaTypeFormatter(JsonDateHandler dateHandler)
            : this()
        {
            _dateHandler = dateHandler;
        }

        public ServiceStackTextJsonMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeConstants.ApplicationJson);

            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

            //TODO: Add XHR Header mapping. Will add this after a discussion with aspnetwebstack team: See: http://aspnetwebstack.codeplex.com/discussions/350758
        }

        protected override Task<object> OnReadFromStreamAsync(Type type, System.IO.Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
        {
            return Task.Factory.StartNew(() =>
            {
                JsConfig.DateHandler = _dateHandler;
                var result = JsonSerializer.DeserializeFromStream(type, stream);
                JsConfig.Reset();
                return result;
            });
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, System.IO.Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, System.Net.TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                JsConfig.DateHandler = _dateHandler;
                JsonSerializer.SerializeToStream(value, type, stream);
                JsConfig.Reset();
            });
        }

        protected override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

        protected override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }
    }
}
