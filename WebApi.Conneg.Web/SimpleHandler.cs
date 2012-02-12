using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Conneg.Web {
	public class SimpleHandler : DelegatingHandler {
		private readonly IFormatterSelector _formatterSelector;
		private readonly IDictionary<string, string> _formatters;

		public SimpleHandler() {
			_formatterSelector = new FormatterSelector();
			_formatters = new Dictionary<string, string> {
				{ "application/xml", @"<?xml version=""1.0""?>
<root>
	<string>{0}</string>
</root>" },
				{ "application/json", @"{{""result"":""{0}""}}" },
				{ "text/html", @"<!doctype html>
<html lang=""en"">
	<head>
		<meta charset=""utf-8"" />
		<title>Simple</title>
	</head>
	<body>
		<section>
			<header>
				<h2>Simple</h2>
			</header>
			{0}
		</section>
	</body>
</html>" },
			};
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			return Task.Factory.StartNew(() => {
				var mediaType = ContentNegotiation.Negotiate(_formatterSelector, _formatters.Keys, request.Headers.Accept);
				var result = ApplyTemplate("value", mediaType);

				var response = request.CreateResponse();
				response.Content = new StringContent(result, Encoding.UTF8, mediaType);
				return response;
			}, cancellationToken);
		}

		private string ApplyTemplate(string content, string mediaType) {
			// This could run Razor or some other template engine.
			return string.Format(_formatters[mediaType], content);
		}
	}
}
