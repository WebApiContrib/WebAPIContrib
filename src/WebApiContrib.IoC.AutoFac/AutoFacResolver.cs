using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Autofac;

namespace WebApiContrib.IoC.Autofac
{
	public class AutofacDependencyScope : IDependencyScope
	{
		private ILifetimeScope scope;

		public AutofacDependencyScope(ILifetimeScope scope)
		{
			if (scope == null)
				throw new ArgumentNullException("scope");

			this.scope = scope;
		}

		public object GetService(Type serviceType)
		{
			if (scope == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed.");

			if (!scope.IsRegistered(serviceType))
				return null;

			return scope.Resolve(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			if (scope == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed.");

			if (!scope.IsRegistered(serviceType))
                return Enumerable.Empty<object>();

			return (IEnumerable<object>) scope.Resolve(typeof (IEnumerable<>).MakeGenericType(serviceType));
		}

    	public void Dispose()
    	{
			scope.Dispose();
    		scope = null;
    	}
	}

	public class AutofacResolver : AutofacDependencyScope, IDependencyResolver
	{
		private readonly IContainer container;

		public AutofacResolver(IContainer container)
			: base(container)
		{
			this.container = container;
		}

		public IDependencyScope BeginScope()
    	{
			return new AutofacDependencyScope(container.BeginLifetimeScope());
    	}
    }
}
