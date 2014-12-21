using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;
using Should;

using WebApiContrib.Serialization;

namespace WebApiContribTests.Serialization
{
	[TestFixture]
	public class UnderscoreMappingResolverTests
	{
		[Test]
		public void JsonSerializer_With_UnderscoreMappingResolver_Should_Deserialize_Properties_Having_Underscore()
		{
			const string json = @"
{
	""ID"": ""ID"",
	""First_Name"": ""First Name"",
	""middle_name"": ""Middle Name"",
	""LAST_NAME"": ""Last Name"",
}
";
			JsonSerializer serializer = new JsonSerializer { ContractResolver = new UnderscoreMappingResolver() };

			UnderscoreEntity entity = JObject.Parse(json).ToObject<UnderscoreEntity>(serializer);

			entity.ID.ShouldEqual("ID"); // one world
			entity.FirstName.ShouldEqual("First Name"); // title case
			entity.MiddleName.ShouldEqual("Middle Name"); // lower case
			entity.LastName.ShouldEqual("Last Name"); // upper case
		}

		class UnderscoreEntity
		{
			public string ID { get; set; }

			public string FirstName { get; set; }

			public string MiddleName { get; set; }

			public string LastName { get; set; }
		}
	}
}