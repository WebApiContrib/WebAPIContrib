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
        private static readonly object writeCompleted = new object();
        private static readonly MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");

        public JavaScriptSerializerFormatter()
        {
            SupportedMediaTypes.Add(DefaultMediaType);
        }

        public static MediaTypeHeaderValue DefaultMediaType
        {
            get { return mediaType; }
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, IFormatterLogger formatterLogger)
        {
            var tcs = new TaskCompletionSource<object>();

            using (var rdr = new StreamReader(stream))
            {
                var json = rdr.ReadToEnd();
                var ser = new JavaScriptSerializer();
            	try
            	{
					var result = ser.Deserialize(json, type);
					tcs.SetResult(result);
            	}
            	catch (Exception ex)
            	{
					tcs.SetException(ex);
            	}
            }

            return tcs.Task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            var tcs = new TaskCompletionSource<object>();

            var ser = new JavaScriptSerializer();
        	try
        	{
				var json = ser.Serialize(value);
				var buf = System.Text.Encoding.Default.GetBytes(json);

				stream.Write(buf, 0, buf.Length);
				stream.Flush();

				tcs.SetResult(writeCompleted);
        	}
        	catch (Exception ex)
        	{
				tcs.SetException(ex);
        	}

            return tcs.Task;
        }
    }
}
