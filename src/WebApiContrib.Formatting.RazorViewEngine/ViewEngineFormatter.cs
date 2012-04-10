using System;
using System.IO;
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

        protected override bool CanReadType(Type type)
        {
            return false;
        }

        protected override bool CanWriteType(Type type)
        {
            
            return typeof(View) == type;
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, System.Net.TransportContext transportContext)
        {
            var view = (View)value;
            view.WriteToStream(stream, _viewEngine);
            var tcs = new TaskCompletionSource<Stream>();
            tcs.SetResult(stream);
            return tcs.Task;
        }

        

    }
}
