using System;
using System.IO;
using RazorEngine.Templating;

namespace WebApiContrib.Formatting.RazorViewEngine
{
    public class EmbeddedResolver : ITemplateResolver
    {
        private readonly Type _rootLocatorType;

        // Type passed should be located at the root of the folder structure where the embedded templates are located
        public EmbeddedResolver(Type rootLocatorType)
        {
            _rootLocatorType = rootLocatorType;
        }


        public string Resolve(string name)
        {
            // To locate embedded files, 
            //    - they must be marked as "Embedded Resource"
            //    - you must use a case senstive path and filename
            //    - the namespaces and project folder names must match.
            //
            name = name.Replace("~/", "").Replace("/", ".");  //Convert "web path" to "resource path"
            var viewStream = _rootLocatorType.Assembly.GetManifestResourceStream(_rootLocatorType, name);

            return new StreamReader(viewStream).ReadToEnd();
        }
    }
}