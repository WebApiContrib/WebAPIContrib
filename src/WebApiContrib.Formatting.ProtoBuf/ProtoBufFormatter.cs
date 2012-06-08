using System;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ProtoBuf.Meta;

namespace WebApiContrib.Formatting
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

    	public override bool CanReadType(Type type)
        {
            return CanReadTypeCore(type);
        }

    	public override bool CanWriteType(Type type)
        {
            return CanReadTypeCore(type);
        }

    	public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContentHeaders contentHeaders, IFormatterLogger formatterLogger)
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

    	public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
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
            return true;
        }
    }
}
