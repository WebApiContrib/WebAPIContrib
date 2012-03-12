using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ProtoBuf.Meta;

namespace Thinktecture.Web.Http.Formatters
{
    public class ProtoBufFormatter : MediaTypeFormatter
    {
        private static readonly MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/x-protobuf");
        private static readonly RuntimeTypeModel model = TypeModel.Create();

        public ProtoBufFormatter()
        {
            model.UseImplicitZeroDefaults = false;

            SupportedMediaTypes.Add(mediaType);
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

        protected override Task<object> OnReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                object result = model.Deserialize(stream, null, type);
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return tcs.Task;
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                model.Serialize(stream, value);
                tcs.SetResult(null);
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