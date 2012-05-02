using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace WebApiContrib.Conneg
{
    public static class ContentNegotiation
    {
        public static string Negotiate(
            this IContentNegotiator contentNegotiator,
            IEnumerable<string> supportedMediaTypes,
            string accept)
        {
            return Negotiate(
                contentNegotiator,
                supportedMediaTypes,
                accept.Split(',').Select(MediaTypeWithQualityHeaderValue.Parse));
        }

        public static string Negotiate(
            this IContentNegotiator contentNegotiator,
            IEnumerable<string> supportedMediaTypes,
            IEnumerable<string> accept)
        {
            return Negotiate(
                contentNegotiator,
                supportedMediaTypes,
                accept.Select(MediaTypeWithQualityHeaderValue.Parse));
        }

        public static string Negotiate(
            this IContentNegotiator contentNegotiator,
            IEnumerable<string> supportedMediaTypes,
            IEnumerable<MediaTypeWithQualityHeaderValue> accept)
        {
            var formatters = supportedMediaTypes.Select(mt => new ConnegFormatter(mt));
            using (var request = new HttpRequestMessage())
            {
                foreach (var header in accept)
                    request.Headers.Accept.Add(header);

            	var result = contentNegotiator.Negotiate(typeof (object), request, formatters);
                return result.MediaType.MediaType;
            }
        }

        private class ConnegFormatter : MediaTypeFormatter
        {
            public ConnegFormatter(string mediaType)
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }

        	public override bool CanReadType(Type type)
            {
                return true;
            }

            public override bool CanWriteType(Type type)
            {
                return true;
            }
        }
    }
}
