using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace WebApiContrib.IoC.Unity
{
	public class UnityDependencyScope : IDependencyScope
	{
        private IUnityContainer container;

        public UnityDependencyScope(IUnityContainer container)
        {
			if (container == null)
				throw new ArgumentNullException("container");

            this.container = container;
        }

        public object GetService(Type serviceType)
        {
			if (container == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed.");

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
			if (container == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed.");

            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

		public void Dispose()
		{
			container.Dispose();
			container = null;
		}
	}

    public class UnityResolver : UnityDependencyScope, IDependencyResolver
    {
        private readonly IUnityContainer container;

        public UnityResolver(IUnityContainer container)
			: base(container)
        {
            this.container = container;
        }

    	public IDependencyScope BeginScope()
    	{
    		return new UnityDependencyScope(container.CreateChildContainer());
    	}
    }
}
