using System.Web.Http;
using Raven.Client;

namespace WebApiContrib.RavenDb.Controllers {
	/// <summary>
	/// This class contains a ASP.NET Web API base controller for use with RavenDb
	/// </summary>
	public class RavenDbController : ApiController {
		public new IDocumentSession Session { get; set; }
	}
}