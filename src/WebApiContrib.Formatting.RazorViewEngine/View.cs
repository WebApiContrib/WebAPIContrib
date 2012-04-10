using System.IO;
using System.Net.Http;
using System.Reflection;

namespace WebApiContrib.Formatting.RazorViewEngine
{
    public class View
    {
        private readonly Stream _template;
        private readonly object _model;



        public View(Stream template, object model)
        {
            _template = template;
            _model = model;
        }


        public void WriteToStream(Stream stream, IViewEngine viewEngine)
        {
            MethodInfo method = typeof(IViewEngine).GetMethod("RenderTo");
            MethodInfo generic = method.MakeGenericMethod(_model.GetType());
            generic.Invoke(viewEngine, new object[] { _model, _template, stream });    

        }

        public StreamContent CreateContent(IViewEngine viewEngine)
        {
            var memoryStream = new MemoryStream();
            WriteToStream(memoryStream, viewEngine);
            memoryStream.Position = 0;
            return new StreamContent(memoryStream);
        }
    }
}
