using System.Web;
using System.Web.Http;

namespace WebApi.Conneg.Web
{
	public class WebApiApplication : HttpApplication
	{
		public static void RegisterApis(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute("Root", "{*path}", new { path = RouteParameter.Optional });
		}

		protected void Application_Start()
		{
			GlobalConfiguration.Configuration.MessageHandlers.Add(new SimpleHandler());
			RegisterApis(GlobalConfiguration.Configuration);
		}
	}
}
