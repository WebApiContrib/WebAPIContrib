using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Thinktecture.Web.Http.Handlers
{
    public class UriFormatExtensionHandler : DelegatingHandler
    {
        private static readonly Dictionary<string, MediaTypeWithQualityHeaderValue> extensionMappings = new Dictionary<string, MediaTypeWithQualityHeaderValue>();

        public UriFormatExtensionHandler(IEnumerable<UriExtensionMapping> mappings)
        {
            foreach (var mapping in mappings)
            {
                extensionMappings[mapping.Extension] = mapping.MediaType;
            }
        }
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var segments = request.RequestUri.Segments;
            var lastSegment = segments.LastOrDefault();
            MediaTypeWithQualityHeaderValue mediaType;
            var found = extensionMappings.TryGetValue(lastSegment, out mediaType);
            
            if (found)
            {
                var newUri = request.RequestUri.OriginalString.Replace("/" + lastSegment, "");
                request.RequestUri = new Uri(newUri, UriKind.Absolute);
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(mediaType);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }

    public class UriExtensionMapping
    {
        public string Extension { get; set; }

        public MediaTypeWithQualityHeaderValue MediaType { get; set; }
    }

    public static class UriExtensionMappingExtensions
    {
        public static void AddMapping(this IList<UriExtensionMapping> mappings, string extension, string mediaType)
        {
            mappings.Add(new UriExtensionMapping { Extension = extension, MediaType = new MediaTypeWithQualityHeaderValue(mediaType) });
        }
    }
}