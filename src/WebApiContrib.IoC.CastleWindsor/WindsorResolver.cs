using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Services;
using Castle.Windsor;

namespace WebApiContrib.IoC.CastleWindsor
{
    public class WindsorResolver : IDependencyResolver
    {
        private readonly IWindsorContainer container;

        public WindsorResolver(IWindsorContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (!container.Kernel.HasComponent(serviceType))
                return null;

            return container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (!container.Kernel.HasComponent(serviceType))
                return Enumerable.Empty<object>();

            return container.ResolveAll(serviceType).Cast<object>();
        }
    }
}
