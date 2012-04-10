using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http.Headers;

namespace WebApiContrib.Formatting.RazorViewEngine
{
    public interface IViewEngine
    {
        void RenderTo<T>(T model, Stream templateStream, Stream outputStream);
        Collection<MediaTypeHeaderValue> SupportedMediaTypes { get; }
    }
}