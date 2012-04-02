using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebApiContrib.Formatting
{
    // Code based on: http://codepaste.net/dfz984
    public class JavaScriptSerializerFormatter : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");

        public JavaScriptSerializerFormatter()
        {
            SupportedMediaTypes.Add(DefaultMediaType);
        }

        public static MediaTypeHeaderValue DefaultMediaType
        {
            get { return mediaType; }
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        protected override bool CanReadType(Type type)
        {
            return true;
        }

        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream,
                                                              HttpContentHeaders contentHeaders,
                                                              FormatterContext formatterContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                using (var rdr = new StreamReader(stream))
                {
                    var json = rdr.ReadToEnd();
                    var ser = new JavaScriptSerializer();
                    var result = ser.Deserialize(json, type);

                    return result;
                }
            });

            return task;
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream,
                                                     HttpContentHeaders contentHeaders,
                                                     FormatterContext formatterContext,
                                                     TransportContext transportContext)
        {
            var task = Task.Factory.StartNew(() =>
            {
                var ser = new JavaScriptSerializer();
                var json = ser.Serialize(value);
                var buf = System.Text.Encoding.Default.GetBytes(json);

                stream.Write(buf, 0, buf.Length);
                stream.Flush();
            });

            return task;
        }
    }
}
