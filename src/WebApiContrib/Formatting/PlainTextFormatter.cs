using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebApiContrib.Formatting
{
    public class PlainTextFormatter : MediaTypeFormatter
    {
        public PlainTextFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }


        protected override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }
        protected override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
        {
            var reader = new StreamReader(stream);
            string value = reader.ReadToEnd();

            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
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
