using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContrib.MessageHandlers
{
    public class UriFormatExtensionHandler : DelegatingHandler
    {
        private static readonly Dictionary<string, MediaTypeWithQualityHeaderValue> extensionMappings = new Dictionary<string, MediaTypeWithQualityHeaderValue>();

        public UriFormatExtensionHandler(IEnumerable<UriFormatExtensionMapping> mappings)
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

    public class UriFormatExtensionMapping
    {
        public string Extension { get; set; }

        public MediaTypeWithQualityHeaderValue MediaType { get; set; }
    }

    public class UriExtensionMappings : List<UriFormatExtensionMapping>
    {
        public UriExtensionMappings()
        {
            this.AddMapping("xml", "application/xml");
            this.AddMapping("json", "application/json");
            this.AddMapping("proto", "application/x-protobuf");
            this.AddMapping("png", "image/png");
            this.AddMapping("jpg", "image/jpg");
        }
    }

    public static class UriFormatExtensionMappingExtensions
    {
        public static void AddMapping(this IList<UriFormatExtensionMapping> mappings, string extension, string mediaType)
        {
            mappings.Add(new UriFormatExtensionMapping { Extension = extension, MediaType = new MediaTypeWithQualityHeaderValue(mediaType) });
        }
    }
}