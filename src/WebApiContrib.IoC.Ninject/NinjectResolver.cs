using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace WebApiContrib.IoC.Ninject
{
	/// <summary>
	/// Updated from https://gist.github.com/2417226/040dd842d3dadb810065f1edad7f2594eaebe806
	/// </summary>
	public class NinjectDependencyScope : IDependencyScope
	{
		private IResolutionRoot resolver;

		internal NinjectDependencyScope(IResolutionRoot resolver)
		{
			Contract.Assert(resolver != null);

			this.resolver = resolver;
		}

		public void Dispose()
		{
			IDisposable disposable = resolver as IDisposable;
			if (disposable != null)
				disposable.Dispose();

			resolver = null;
		}

		public object GetService(Type serviceType)
		{
			if (resolver == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed");

			return resolver.TryGet(serviceType);
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			if (resolver == null)
				throw new ObjectDisposedException("this", "This scope has already been disposed");

			return resolver.GetAll(serviceType);
		}
	}

	public class NinjectResolver : NinjectDependencyScope, IDependencyResolver
	{
		private IKernel kernel;

		public NinjectResolver(IKernel kernel)
			: base(kernel)
		{
			this.kernel = kernel;
		}

		public IDependencyScope BeginScope()
		{
			return new NinjectDependencyScope(kernel.BeginBlock());
		}
	}
}
