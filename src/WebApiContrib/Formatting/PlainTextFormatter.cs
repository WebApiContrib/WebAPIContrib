using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiContrib.Formatting
{
    public class PlainTextFormatter : MediaTypeFormatter
    {
        public PlainTextFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

    	public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

    	public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

    	public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, IFormatterLogger formatterLogger)
        {
            var reader = new StreamReader(stream);
            string value = reader.ReadToEnd();

            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(value);
            return tcs.Task;
        }

    	public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            var writer = new StreamWriter(stream);
            writer.Write((string) value);
            writer.Flush();
            
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}
