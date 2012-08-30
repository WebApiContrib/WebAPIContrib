using System;
using System.Web.Http.Controllers;

namespace WebApiContrib.ModelBinders
{
    public class MvcStyleBindingAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(IActionValueBinder), new MvcActionValueBinder());
        }
    }
}
