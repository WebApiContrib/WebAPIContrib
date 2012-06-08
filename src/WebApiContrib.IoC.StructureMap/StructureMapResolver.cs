using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using System.Web.Http.Dispatcher;
using StructureMap;
using System.Net.Http;

namespace WebApiContrib.IoC.StructureMap
{
	public class StructureMapDependencyScope : IDependencyScope
	{
        private IContainer container;

        public StructureMapDependencyScope(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
        }

        public object GetService(Type serviceType)
        {
			if (container == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed.");

            return container.TryGetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
			if (container == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed.");

            return container.GetAllInstances(serviceType).Cast<object>();
        }

		public void Dispose()
		{
			container.Dispose();
			container = null;
		}
	}

    public class StructureMapResolver : StructureMapDependencyScope, IDependencyResolver, IHttpControllerActivator
    {
        private readonly IContainer container;

        public StructureMapResolver(IContainer container)
			: base(container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;

            this.container.Inject(typeof(IHttpControllerActivator), this);
        }

    	public IDependencyScope BeginScope()
    	{
			return new StructureMapDependencyScope(container.GetNestedContainer());
    	}

    	public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
    	{
            return (IHttpController) container.GetInstance(controllerType);
    	}
    }
}
