using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http.Headers;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace WebApiContrib.Formatting.RazorViewEngine
{
    public class RazorViewEngine  : IViewEngine
    {

        public RazorViewEngine(Type rootLocatorType) 
        {
            var config = new TemplateServiceConfiguration();
            config.Resolver = new EmbeddedResolver(rootLocatorType);

            var templateService = new TemplateService(config);

            Razor.SetTemplateService(templateService);
        }

        public RazorViewEngine()
        {
            var config = new TemplateServiceConfiguration();

            var templateService = new TemplateService(config);

            Razor.SetTemplateService(templateService);
            
        }

        public void RenderTo<T>(T model, Stream templateStream, Stream outputStream)
        {
            string template = new StreamReader(templateStream).ReadToEnd();
            string result = Razor.Parse<T>(template, model);
            var sw = new StreamWriter(outputStream);
         
            sw.Write(result);
            sw.Flush();
        }


        public Collection<MediaTypeHeaderValue> SupportedMediaTypes
        {
            get { return new Collection<MediaTypeHeaderValue>() {new MediaTypeHeaderValue("text/html")}; }
        }

    }
}