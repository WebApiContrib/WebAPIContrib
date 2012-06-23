using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarManager.CachingClient
{
	public class HeaderTracerHandler : DelegatingHandler
	{
		private Action<string> _tracer;

		public HeaderTracerHandler(Action<string> tracer)
		{
			_tracer = tracer;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			_tracer(request.Headers.ToString());
			if (request.Content != null)
				_tracer(request.Content.Headers.ToString());

			_tracer("-------");
			return base.SendAsync(request, cancellationToken)
				.ContinueWith((task) =>
				              	{
									HttpResponseMessage response = null;
									try
									{
										response = task.Result;
										_tracer(response.StatusCode.ToString());
										_tracer(response.Headers.ToString());
										if(response.Content!=null)
											_tracer(response.Content.Headers.ToString());										

									}
									catch (Exception e)
									{
										_tracer(e.ToString());
									}
				              		return response;
				              	});
		}

	}
}
