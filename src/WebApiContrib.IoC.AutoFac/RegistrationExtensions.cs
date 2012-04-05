using System.Reflection;
using System.Web.Http.Controllers;
using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;

namespace WebApiContrib.IoC.AutoFac
{
    public static class RegistrationExtensions
    {
        public static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            RegisterApiControllers(this ContainerBuilder builder, params Assembly[] controllerAssemblies)
        {
            return from t in builder.RegisterAssemblyTypes(controllerAssemblies)
                   where typeof(IHttpController).IsAssignableFrom(t) && t.Name.EndsWith("Controller")
                   select t;
        }
    }
}
