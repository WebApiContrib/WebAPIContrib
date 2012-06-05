using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WebApiContrib.Formatting
{
    // Code based on: http://blogs.msdn.com/b/carlosfigueira/archive/2012/03/05/writing-formurlencoded-data-with-asp-net-web-apis.aspx
    public class ReadWriteFormUrlEncodedFormatter : FormUrlEncodedMediaTypeFormatter
    {
        private readonly static Encoding encoding = new UTF8Encoding(false);

    	public override bool CanWriteType(Type type)
        {
            return type == typeof(JObject);
        }

    	public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, TransportContext transportContext)
        {
			return Task.Factory.StartNew(() =>
			{
				var pairs = new List<string>();
				Flatten(pairs, value as IDictionary<string, JToken>);
				var bytes = encoding.GetBytes(string.Join("&", pairs));
				stream.Write(bytes, 0, bytes.Length);
			});
        }

        private void Flatten(List<string> pairs, IDictionary<string, JToken> input)
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

        private static void Flatten(List<string> pairs, JToken input, List<object> indices)
        {
            if (input == null)
            {
                return; // null values aren't serialized
            }

            switch (input.Type)
            {
                case JTokenType.Array:
            		var count = ((ICollection<JToken>) input).Count;
                    for (int i = 0; i < count; i++)
                    {
                        indices.Add(i);
                        Flatten(pairs, input[i], indices);
                        indices.RemoveAt(indices.Count - 1);
                    }

                    break;
                case JTokenType.Object:
            		var dict = (IDictionary<string, JToken>) input;
                    foreach (var kvp in dict)
                    {
                        indices.Add(kvp.Key);
                        Flatten(pairs, kvp.Value, indices);
                        indices.RemoveAt(indices.Count - 1);
                    }

                    break;
                default:
                    var value = input.Value<string>();
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

					if (value != null)
						pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(name.ToString()), Uri.EscapeDataString(value)));

                    break;
            }
        }
    }
}
