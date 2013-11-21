using System.IO;
using System.Linq;
using System.Reflection;
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
		public async Task Response_Deserialize_Serialize()
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebApiContribTests.Data.Response");
			var serializer = new MessageContentHttpMessageSerializer();
			var response = await serializer.DeserializeToResponseAsync(stream);

			var memoryStream = new MemoryStream();
			await serializer.SerializeAsync(Task.FromResult(response), memoryStream);

			memoryStream.Position = 0;
			var response2 = await serializer.DeserializeToResponseAsync(memoryStream);
			var result = DeepComparer.Compare(response, response2);
			if (result.Count() > 0)
				Assert.Fail(string.Join("\r\n", result));
		}

		[Test]
		public async Task Request_Deserialize_Serialize()
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WebApiContribTests.Data.Request");
			var serializer = new MessageContentHttpMessageSerializer();
			var request = await serializer.DeserializeToRequestAsync(stream);

			var memoryStream = new MemoryStream();
			await serializer.SerializeAsync(request, memoryStream);

			memoryStream.Position = 0;
			var request2 = await serializer.DeserializeToRequestAsync(memoryStream);
			var result = DeepComparer.Compare(request, request2);

			if (result.Count() > 0)
				Assert.Fail(string.Join("\r\n", result));
		}
	}
}
