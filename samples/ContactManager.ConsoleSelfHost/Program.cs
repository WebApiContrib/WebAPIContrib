using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.SelfHost;
using ContactManager.Models;
using Ninject;

namespace ContactManager.ConsoleSelfHost
{
    class Program
    {
        private const string webApiUrl = "http://localhost:7777/services/cm";

        static void Main()
        {
            Assembly.Load("ContactManager.APIs");

            var host = SetupWebApiServer(webApiUrl);
            host.OpenAsync().Wait();

            Console.WriteLine("Web API host running on {0}", webApiUrl);
            Console.WriteLine();
            Console.ReadKey();

            host.CloseAsync().Wait();
        }

        private static HttpSelfHostServer SetupWebApiServer(string url)
        {
            var ninjectKernel = new StandardKernel();
            ninjectKernel.Bind<IContactRepository>().To<InMemoryContactRepository>();

            var configuration = new HttpSelfHostConfiguration(url);
            configuration.ServiceResolver.SetResolver(
                t => ninjectKernel.TryGet(t),
                t => ninjectKernel.GetAll(t));

            configuration.Routes.MapHttpRoute(
                "Default",
                "{controller}/{id}/{ext}",
                new {id = RouteParameter.Optional, ext = RouteParameter.Optional});

            var host = new HttpSelfHostServer(configuration);

            return host;
        }
    }
}
