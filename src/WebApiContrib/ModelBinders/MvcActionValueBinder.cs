using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;

namespace WebApiContrib.ModelBinders
{
    // Binder with MVC semantics. Treat the body as KeyValue pairs and model bind it.
    // See http://blogs.msdn.com/b/jmstall/archive/2012/04/18/mvc-style-parameter-binding-for-webapi.aspx
    public class MvcActionValueBinder : DefaultActionValueBinder
    {
        // Per-request storage, uses the Request.Properties bag. We need a unique key into the bag.
        private const string Key = "{5DC187FB-BFA0-462A-AB93-9E8036871EC8}";

        public override HttpActionBinding GetBinding(HttpActionDescriptor actionDescriptor)
        {
            var actionBinding = new MvcActionBinding();
 
            HttpParameterDescriptor[] parameters = actionDescriptor.GetParameters().ToArray();
            HttpParameterBinding[] binders = Array.ConvertAll(parameters, DetermineBinding);

            actionBinding.ParameterBindings = binders;
 
            return actionBinding;
        }

        private HttpParameterBinding DetermineBinding(HttpParameterDescriptor parameter)
        {
            HttpConfiguration config = parameter.Configuration;
            var attr = new ModelBinderAttribute(); // use default settings

            ModelBinderProvider provider = attr.GetModelBinderProvider(config);
	        IModelBinder binder = provider.GetBinder(config, parameter.ParameterType);

            // Alternatively, we could put this ValueProviderFactory in the global config.
            var vpfs = new List<ValueProviderFactory>(attr.GetValueProviderFactories(config)) { new BodyValueProviderFactory() };
            return new ModelBinderParameterBinding(parameter, binder, vpfs);
        }

        // Derive from ActionBinding so that we have a chance to read the body once and then share that with all the parameters.
        private class MvcActionBinding : HttpActionBinding
        {
            // Read the body upfront , add as a ValueProvider
            public override Task ExecuteBindingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
            {
                HttpRequestMessage request = actionContext.ControllerContext.Request;
                HttpContent content = request.Content;
                if (content != null)
                {
                    FormDataCollection fd = content.ReadAsAsync<FormDataCollection>().Result;
                    if (fd != null)
                    {
                        var nvc = fd.ReadAs<IEnumerable<KeyValuePair<string, string>>>();
                        IValueProvider vp = new NameValuePairsValueProvider(nvc, CultureInfo.InvariantCulture);
                        request.Properties.Add(Key, vp);
                    }
                }
 
                return base.ExecuteBindingAsync(actionContext, cancellationToken);
            }
        }

        // Get a value provider over the body. This can be shared by all parameters.
        // This gets the values computed in MvcActionBinding.
        private class BodyValueProviderFactory : ValueProviderFactory
        {
            public override IValueProvider GetValueProvider(HttpActionContext actionContext)
            {
                object vp;
                actionContext.Request.Properties.TryGetValue(Key, out vp);
                return (IValueProvider)vp; // can be null 
            }
        }
    }
}
