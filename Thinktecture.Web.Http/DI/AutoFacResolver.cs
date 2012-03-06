using System;
using System.Collections.Generic;
using System.Web.Http.Services;
using Autofac;

namespace Thinktecture.Web.Http.DI
{
    public class AutoFacResolver : IDependencyResolver
    {
        private readonly IContainer container;

        public AutoFacResolver(IContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            if (container.IsRegistered(serviceType))
            {
                var service = container.Resolve(serviceType);

                return service;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (container.IsRegistered(serviceType))
            {
                var services = (IEnumerable<object>)container.Resolve(typeof(IEnumerable<>).MakeGenericType(new[] { serviceType }));

                return services;
            }
            else
            {
                return null;
            }
        }
    }
}
