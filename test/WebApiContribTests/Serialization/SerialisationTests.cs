using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
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
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebApiContribTests.Data.Response.bin");
			var serializer = new MessageContentHttpMessageSerializer();
			var response = serializer.DeserializeToResponse(stream);

			var memoryStream = new MemoryStream();
			serializer.Serialize(response, memoryStream);

			memoryStream.Position = 0;
			var response2 = serializer.DeserializeToResponse(memoryStream);
			var result = DeepComparer.Compare(response, response2);
			if(result.Count()>0)
				Assert.Fail(string.Join("\r\n", result));
		}

		[Test]
		[Ignore] // !! Ignore this until RTM since this is fixed. See http://aspnetwebstack.codeplex.com/workitem/303
		public void Request_Deserialize_Serialize()
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebApiContribTests.Data.Request.bin");
			var serializer = new MessageContentHttpMessageSerializer();
			var request = serializer.DeserializeToRequest(stream);

			var memoryStream = new MemoryStream();
			serializer.Serialize(request, memoryStream);

			memoryStream.Position = 0;
			var request2 = serializer.DeserializeToRequest(memoryStream);
			var result = DeepComparer.Compare(request, request2);
			if (result.Count() > 0)
				Assert.Fail(string.Join("\r\n", result));
		}

	

	}
}
