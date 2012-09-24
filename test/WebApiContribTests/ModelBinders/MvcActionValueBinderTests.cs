using System;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using NUnit.Framework;
using WebApiContrib.ModelBinders;

namespace WebApiContribTests.ModelBinders
{
	public class Customer
	{
		public string name { get; set; }
		public int age { get; set; }
		public override bool Equals(object obj)
		{
			if (obj is Customer)
			{
				var other = obj as Customer;
				return name == other.name && age == other.age;
			}

			return false;
		}
		public override int GetHashCode()
		{
			return name.GetHashCode() ^ age.GetHashCode();
		}
	}

	[MvcStyleBinding]
	public class MvcController : ApiController
	{
		[HttpGet]
		public Customer Combined(Customer item)
		{
			return item;
		}
	}

	[TestFixture]
	public class MvcActionValueBinderTests
	{
		private HttpConfiguration config;
		private HttpServer server;
		private HttpClient client;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			config = new HttpConfiguration();
			config.Routes.MapHttpRoute("Default", "{controller}/{action}", new { controller = "Home" });

			server = new HttpServer(config);
			client = new HttpClient(server);
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			client.Dispose();
			server.Dispose();
			config.Dispose();
		}

		[Test]
		public void TestOneFieldFromUriOneFromBody()
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("http://localhost:8080/Mvc/Combined?age=10"),
				Content = FormUrlContent("name=Fred")
			};

			var response = client.SendAsync(request).Result;
			var actual = response.Content.ReadAsAsync<Customer>().Result;

			var expected = new Customer { name = "Fred", age = 10 };
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void TestBothFieldsFromBody()
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("http://localhost:8080/Mvc/Combined"),
				Content = FormUrlContent("name=Fred&age=11")
			};

			var response = client.SendAsync(request).Result;
			var actual = response.Content.ReadAsAsync<Customer>().Result;

			var expected = new Customer { name = "Fred", age = 11 };
			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void TestBothFieldsFromUri()
		{
			var response = client.GetAsync("http://localhost:8080/Mvc/Combined?name=Bob&age=20").Result;
			var actual = response.Content.ReadAsAsync<Customer>().Result;

			var expected = new Customer { name = "Bob", age = 20 };
			Assert.That(actual, Is.EqualTo(expected));
		}

		static HttpContent FormUrlContent(string content)
		{
			return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
		}
	}
}
