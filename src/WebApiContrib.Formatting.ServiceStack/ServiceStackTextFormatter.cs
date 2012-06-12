using System;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;

namespace WebApiContrib.Formatting
{
    public class ServiceStackTextFormatter : MediaTypeFormatter
    {
        //Uses ISO8601 date by default
        private JsonDateHandler _dateHandler = JsonDateHandler.ISO8601;

        public ServiceStackTextFormatter(JsonDateHandler dateHandler)
            : this()
        {
            _dateHandler = dateHandler;
        }

        public ServiceStackTextFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));

            //TODO: Add XHR Header mapping. Will add this after a discussion with aspnetwebstack team: See: http://aspnetwebstack.codeplex.com/discussions/350758
        }

    	public override Task<object> ReadFromStreamAsync(Type type, System.IO.Stream stream, HttpContentHeaders contentHeaders, IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew(() =>
            {
                JsConfig.DateHandler = _dateHandler;
                var result = JsonSerializer.DeserializeFromStream(type, stream);
                JsConfig.Reset();
                return result;
            });
        }

    	public override Task WriteToStreamAsync(Type type, object value, System.IO.Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                JsConfig.DateHandler = _dateHandler;
                JsonSerializer.SerializeToStream(value, type, stream);
                JsConfig.Reset();
            });
        }

    	public override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

    	public override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }
    }
}
