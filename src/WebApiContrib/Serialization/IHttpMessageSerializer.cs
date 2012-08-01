using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace WebApiContrib.Serialization
{
	public interface IHttpMessageSerializer
	{
		void Serialize(HttpResponseMessage response, Stream stream);
		void Serialize(HttpRequestMessage request, Stream stream);
		HttpResponseMessage DeserializeToResponse(Stream stream);
		HttpRequestMessage DeserializeToRequest(Stream stream);
	}

}
