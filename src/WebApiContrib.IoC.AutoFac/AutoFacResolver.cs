using System;
using System.Collections.Generic;
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

			if (scope.IsRegistered(serviceType))
			{
				var service = scope.Resolve(serviceType);

				return service;
			}
			else
			{
				return null;
			}
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			if (scope == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed.");

			if (scope.IsRegistered(serviceType))
			{
				var services = (IEnumerable<object>) scope.Resolve(typeof (IEnumerable<>).MakeGenericType(new[] {serviceType}));

				return services;
			}
			else
			{
				return null;
			}
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
