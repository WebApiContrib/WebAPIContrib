using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiContrib.RavenDb.RavenDb;

namespace WebApiContrib.RavenDb.ActionFilters {
	/// <summary>
	/// This filter will manage the session for all of the controllers that needs a Raven Document Session.
	/// It does so by automatically injecting a session to the first public property of type IDocumentSession available
	/// on the controller.
	/// </summary>
	public class RavenActionFilterAttribute : ActionFilterAttribute {
		readonly string _connectionStringName;
		readonly Uri _uri;

		public RavenActionFilterAttribute(Uri uri) {
			_uri = uri;
		}

		public RavenActionFilterAttribute(string connectionStringName) {
			_connectionStringName = connectionStringName;
		}

		public override void OnActionExecuting(HttpActionContext filterContext) {
			DocumentStoreHolder.Url = _uri ?? null;
			DocumentStoreHolder.ConnectionStringName = !string.IsNullOrEmpty(_connectionStringName)
														? _connectionStringName
														: null;
			filterContext.Request.Properties["RavenDocumentStore"] = 
				DocumentStoreHolder.TryAddSession(filterContext.ControllerContext.Controller);
		}


		public override void OnActionExecuted(HttpActionExecutedContext filterContext) {
			DocumentStoreHolder
				.TryComplete(filterContext.ActionContext.ControllerContext.Controller, filterContext.Exception == null);
		}
	}
}