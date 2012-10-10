using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApiContrib.Serialization;
using WebApiContribTests.Helpers;

namespace WebApiContribTests.Serialization
{
	[TestFixture]
	public class SerialisationTests
	{


		[Test]
		public void Response_Deserialize_Serialize()
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebApiContribTests.Data.Response.cs");
			var serializer = new MessageContentHttpMessageSerializer();
			var response = serializer.DeserializeToResponseAsync(stream).Result;

			var memoryStream = new MemoryStream();
			serializer.SerializeAsync(TaskHelpers.FromResult(response), memoryStream).Wait();

			memoryStream.Position = 0;
			var response2 = serializer.DeserializeToResponseAsync(memoryStream).Result;
			var result = DeepComparer.Compare(response, response2);
			if(result.Count()>0)
				Assert.Fail(string.Join("\r\n", result));
		}

		[Test]
		public void Request_Deserialize_Serialize()
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebApiContribTests.Data.Request.cs");
			var serializer = new MessageContentHttpMessageSerializer();
			var request = serializer.DeserializeToRequestAsync(stream).Result;

			var memoryStream = new MemoryStream();
			serializer.SerializeAsync(request, memoryStream).Wait();

			memoryStream.Position = 0;
			var request2 = serializer.DeserializeToRequestAsync(memoryStream).Result;
			var result = DeepComparer.Compare(request, request2);

			if (result.Count() > 0)
				Assert.Fail(string.Join("\r\n", result));
		}

	

	}
}
