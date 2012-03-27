using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Thinktecture.Web.Http.Formatters
{
    // Code based on: http://code.msdn.microsoft.com/Using-JSONNET-with-ASPNET-b2423706
    public class JsonNetFormatter : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public JsonNetFormatter(JsonSerializerSettings jsonSerializerSettings)
        {
            this.jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();

            SupportedMediaTypes.Add(mediaType);
            Encoding = new UTF8Encoding(false, true);
        }

        public static MediaTypeHeaderValue DefaultMediaType
        {
            get { return mediaType; }
        }

        protected override bool CanReadType(Type type)
        {
            return CanReadTypeCore(type);
        }

        protected override bool CanWriteType(Type type)
        {
            return CanReadTypeCore(type);
        }

        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream,
                                                              HttpContentHeaders contentHeaders,
                                                              FormatterContext formatterContext)
        {
            var serializer = JsonSerializer.Create(jsonSerializerSettings);
            var tcs = new TaskCompletionSource<object>();

            try
            {
                using (var streamReader = new StreamReader(stream, Encoding))
                {
                    using (var jsonTextReader = new JsonTextReader(streamReader))
                    {
                        var result = serializer.Deserialize(jsonTextReader, type);
                        tcs.SetResult(result);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream,
                                                     HttpContentHeaders contentHeaders,
                                                     FormatterContext formatterContext,
                                                     TransportContext transportContext)
        {
            var serializer = JsonSerializer.Create(jsonSerializerSettings);
            var tcs = new TaskCompletionSource<object>();

            try
            {
                using (var streamWriter = new StreamWriter(stream, Encoding))
                {
                    using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonTextWriter, value);
                        tcs.SetResult(null);
                    }
                }
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        private static bool CanReadTypeCore(Type type)
        {
            if (type == typeof(IKeyValueModel))
            {
                return false;
            }

            return true;
        }
    }
}