using System;
using System.Collections.Generic;
using System.Web.Http.Services;
using Ninject;

namespace Thinktecture.Web.Http.DI
{
    public class NinjectResolver : IDependencyResolver
    {
        private readonly IKernel kernel;

        public NinjectResolver(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
    }
}
