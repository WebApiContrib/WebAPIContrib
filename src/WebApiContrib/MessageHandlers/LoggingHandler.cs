using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Thinktecture.Web.Http.Data;
using Thinktecture.Web.Http.Messages;

namespace Thinktecture.Web.Http.Handlers
{
    // Code based on: http://weblogs.asp.net/pglavich/archive/2012/02/26/asp-net-web-api-request-response-usage-logging.aspx
	public class LoggingHandler : DelegatingHandler
	{
		private ILoggingRepository _repository;

		public LoggingHandler(ILoggingRepository repository)
		{
			_repository = repository;
		}

		public LoggingHandler(HttpMessageHandler innerHandler, ILoggingRepository repository)
			: base(innerHandler)
		{
			_repository = repository;
		}

		protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
		{
			// Log the request information
			LogRequestLoggingInfo(request);

			// Execute the request
			var response = base.SendAsync(request, cancellationToken);

			response.ContinueWith((responseMsg) =>
			{
				// Extract the response logging info then persist the information
				LogResponseLoggingInfo(responseMsg.Result);
			});

			return response;
		}

		private void LogRequestLoggingInfo(HttpRequestMessage request)
		{
			var info = new ApiLoggingInfo();
			info.HttpMethod = request.Method.Method;
			info.UriAccessed = request.RequestUri.AbsoluteUri;
			info.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";
			info.MessageType = HttpMessageType.Request;
			
			ExtractMessageHeadersIntoLoggingInfo(info, request.Headers.ToList());
			
			if (request.Content != null)
			{
				request.Content.ReadAsByteArrayAsync()
					.ContinueWith((task) =>
					{
						info.BodyContent = System.Text.UTF8Encoding.UTF8.GetString(task.Result);
						_repository.Log(info);

					});

				return;
			}

			_repository.Log(info);
		}

		private void LogResponseLoggingInfo(HttpResponseMessage response)
		{
			var info = new ApiLoggingInfo();
			info.MessageType = HttpMessageType.Response;
			info.HttpMethod = response.RequestMessage.Method.ToString();
			info.ResponseStatusCode = response.StatusCode;
			info.ResponseStatusMessage = response.ReasonPhrase;
			info.UriAccessed = response.RequestMessage.RequestUri.AbsoluteUri;
			info.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";

			ExtractMessageHeadersIntoLoggingInfo(info, response.Headers.ToList());
			
			if (response.Content != null)
			{
				response.Content.ReadAsByteArrayAsync()
					.ContinueWith(t =>
					{
						var responseMsg = System.Text.UTF8Encoding.UTF8.GetString(t.Result);
						info.BodyContent = responseMsg;
						_repository.Log(info);
					});

				return;
			}

			_repository.Log(info);
		}

		private void ExtractMessageHeadersIntoLoggingInfo(ApiLoggingInfo info, List<KeyValuePair<string, IEnumerable<string>>> headers)
		{
			headers.ForEach(h =>
			{
				// convert the header values into one long string from a series of IEnumerable<string> values so it looks for like a HTTP header
				var headerValues = new StringBuilder();

				if (h.Value != null)
				{
					foreach (var hv in h.Value)
					{
						if (headerValues.Length > 0)
						{
							headerValues.Append(", ");
						}
						headerValues.Append(hv);
					}
				}
				info.Headers.Add(string.Format("{0}: {1}", h.Key, headerValues.ToString()));
			});
		}
	}
}