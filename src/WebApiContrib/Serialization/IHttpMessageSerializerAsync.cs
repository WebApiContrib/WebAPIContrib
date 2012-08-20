using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiContrib.Serialization
{
	public interface IHttpMessageSerializerAsync
	{
		Task Serialize(Task<HttpResponseMessage> response, Stream stream);
		void Serialize(HttpRequestMessage request, Stream stream);
		Task<HttpResponseMessage> DeserializeToResponse(Stream stream);
		Task<HttpRequestMessage> DeserializeToRequest(Stream stream);

	}
}
