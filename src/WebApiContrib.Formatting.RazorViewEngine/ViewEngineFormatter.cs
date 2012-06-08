using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApiContrib.Formatting.RazorViewEngine
{
    public class ViewEngineFormatter : MediaTypeFormatter
    {
        private readonly IViewEngine _viewEngine;

        public ViewEngineFormatter(IViewEngine viewEngine)
        {
            _viewEngine = viewEngine;

            foreach (var mediaTypeHeaderValue in _viewEngine.SupportedMediaTypes)
            {
                SupportedMediaTypes.Add(mediaTypeHeaderValue);    
            }
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(View) == type;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
            var tcs = new TaskCompletionSource<Stream>();

            var view = (View) value;
            try
            {
                view.WriteToStream(stream, _viewEngine);
                tcs.SetResult(stream);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }
    }
}
