using System;
using System.Collections.Generic;
using System.Web.Http.Services;
using Microsoft.Practices.Unity;

namespace Thinktecture.Web.Http.DI
{
    public class UnityResolver : IDependencyResolver
    {
        private readonly IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }
    }
}
