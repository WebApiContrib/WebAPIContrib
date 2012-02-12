using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace WebApi.Conneg {
	public static class ContentNegotiation {
		public static string Negotiate(
			IFormatterSelector formatterSelector,
			IEnumerable<string> supportedMediaTypes,
			string accept) {
			return Negotiate(
				formatterSelector,
				supportedMediaTypes,
				accept.Split(',').Select(MediaTypeWithQualityHeaderValue.Parse));
		}

		public static string Negotiate(
			IFormatterSelector formatterSelector,
			IEnumerable<string> supportedMediaTypes,
			IEnumerable<string> accept) {
			return Negotiate(
				formatterSelector,
				supportedMediaTypes,
				accept.Select(MediaTypeWithQualityHeaderValue.Parse));
		}

		public static string Negotiate(
			IFormatterSelector formatterSelector,
			IEnumerable<string> supportedMediaTypes,
			IEnumerable<MediaTypeWithQualityHeaderValue> accept) {
			var formatters = supportedMediaTypes.Select(mt => new ConnegFormatter(mt));
			var response = new HttpResponseMessage { RequestMessage = new HttpRequestMessage() };
			foreach (var header in accept)
				response.RequestMessage.Headers.Accept.Add(header);

			MediaTypeHeaderValue mediaType;
			formatterSelector.SelectWriteFormatter(typeof (object), new FormatterContext(response, false), formatters, out mediaType);
			return mediaType.MediaType;
		}

		private class ConnegFormatter : MediaTypeFormatter {
			public ConnegFormatter(string mediaType) {
				SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
			}

			protected override bool CanReadType(Type type) {
				return true;
			}

			protected override bool CanWriteType(Type type) {
				return true;
			}
		}
	}
}
