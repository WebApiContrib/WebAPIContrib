using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Web.Http.Formatters
{
    // Code based on: http://blogs.msdn.com/b/carlosfigueira/archive/2012/03/05/writing-formurlencoded-data-with-asp-net-web-apis.aspx
    public class ReadWriteFormUrlEncodedFormatter : FormUrlEncodedMediaTypeFormatter
    {
        private readonly static Encoding encoding = new UTF8Encoding(false);

        protected override bool CanWriteType(Type type)
        {
            return base.CanWriteType(type) || type == typeof(JsonObject);
        }

        protected override Task OnWriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, FormatterContext formatterContext, TransportContext transportContext)
        {
            if (type == typeof(JsonObject))
            {
                return Task.Factory.StartNew(() =>
                {
                    var pairs = new List<string>();
                    Flatten(pairs, value as JsonObject);
                    var bytes = encoding.GetBytes(string.Join("&", pairs));
                    stream.Write(bytes, 0, bytes.Length);
                });
            }
            else
            {
                return base.OnWriteToStreamAsync(type, value, stream, contentHeaders, formatterContext, transportContext);
            }
        }

        private void Flatten(List<string> pairs, JsonObject input)
        {
            var stack = new List<object>();

            foreach (var key in input.Keys)
            {
                stack.Add(key);
                Flatten(pairs, input[key], stack);
                stack.RemoveAt(stack.Count - 1);

                if (stack.Count != 0)
                {
                    throw new InvalidOperationException("Problem flattening JSONObject.");
                }
            }
        }

        private static void Flatten(List<string> pairs, JsonValue input, List<object> indices)
        {
            if (input == null)
            {
                return; // null values aren't serialized
            }

            switch (input.JsonType)
            {
                case JsonType.Array:
                    for (int i = 0; i < input.Count; i++)
                    {
                        indices.Add(i);
                        Flatten(pairs, input[i], indices);
                        indices.RemoveAt(indices.Count - 1);
                    }

                    break;
                case JsonType.Object:
                    foreach (var kvp in input)
                    {
                        indices.Add(kvp.Key);
                        Flatten(pairs, kvp.Value, indices);
                        indices.RemoveAt(indices.Count - 1);
                    }

                    break;
                default:
                    var value = input.ReadAs<string>();
                    var name = new StringBuilder();

                    for (int i = 0; i < indices.Count; i++)
                    {
                        var index = indices[i];
                        if (i > 0)
                        {
                            name.Append('[');
                        }

                        if (i < indices.Count - 1 || index is string)
                        {
                            // last array index not shown
                            name.Append(index);
                        }

                        if (i > 0)
                        {
                            name.Append(']');
                        }
                    }

                    pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(name.ToString()), Uri.EscapeDataString(value)));

                    break;
            }
        }
    }
}
