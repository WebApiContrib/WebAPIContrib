using System.Collections.Generic;
using System.Net;

namespace Thinktecture.Web.Http.Messages
{
    // Code based on: http://weblogs.asp.net/pglavich/archive/2012/02/26/asp-net-web-api-request-response-usage-logging.aspx
	public class ApiLoggingInfo
	{
		private List<string> _headers = new List<string>();

		public string HttpMethod { get; set; }
		public string UriAccessed { get; set; }
		public string BodyContent { get; set; }
		public HttpStatusCode ResponseStatusCode { get; set; }
		public string ResponseStatusMessage { get; set; }
		public string IpAddress { get; set; }
		public HttpMessageType MessageType { get; set; }

		public List<string> Headers
		{
			get { return _headers; }
		}
	}

	public enum HttpMessageType
	{
		Request,
		Response
	}
}
