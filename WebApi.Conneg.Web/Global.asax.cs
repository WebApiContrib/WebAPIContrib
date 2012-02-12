using System.Web;
using System.Web.Http;

namespace WebApi.Conneg.Web {
	public class WebApiApplication : HttpApplication {
		public static void RegisterApis(HttpConfiguration config) {
			// NOTE: There is nothing similar to the MVC {path*} that appears to work.
			config.Routes.MapHttpRoute("Root", "{path}", new { path = RouteParameter.Optional });
			config.Routes.MapHttpRoute("Default", "{controller}/{id}", new { id = RouteParameter.Optional });
		}

		protected void Application_Start() {
			GlobalConfiguration.Configuration.MessageHandlers.Add(new SimpleHandler());
			RegisterApis(GlobalConfiguration.Configuration);
		}
	}
}
