using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using CarManager.Data;

namespace CarManager.CachingClient
{
	/// <summary>
	/// This sample uses CarManager.Web sample to display Caching features
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{

			try
			{
				var httpClient = new HttpClient(new HeaderTracerHandler(x => Console.WriteLine(x))
				{
					InnerHandler = new HttpClientHandler()
				});

				// assuming CarManager.Web runs at localhost:8031
				httpClient.BaseAddress = new Uri("http://localhost:8031/api/");

				// _________________________________________________________
				OutputHeader("Getting cars");
				var httpResponseMessage = httpClient.GetAsync("Cars").Result;
				string carsETag = httpResponseMessage.Headers.ETag.ToString();
				var lastModified = httpResponseMessage.Content.Headers.LastModified ?? DateTimeOffset.UtcNow.AddDays(-1);
				OutputSeparator();

				// _________________________________________________________
				OutputHeader("Getting fastest cars");
				httpResponseMessage = httpClient.GetAsync("Cars").Result;
				string fastestCarsETag = httpResponseMessage.Headers.ETag.ToString();
				var fastestLastModified = httpResponseMessage.Content.Headers.LastModified ?? DateTimeOffset.UtcNow.AddDays(-1);
				OutputSeparator();

				// _________________________________________________________
				OutputHeader("Getting cars using etag. Expecting 'Not modified'");
				var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "Cars");
				httpRequestMessage.Headers.Add("If-None-Match", new[] { carsETag });
				httpResponseMessage = httpClient.SendAsync(httpRequestMessage).Result;
				OutputSeparator();

				// _________________________________________________________
				OutputHeader("Getting cars using last modified. Expecting 'Not modified'");
				httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "Cars");
				httpRequestMessage.Headers.Add("If-Modified-Since", new[] { lastModified.ToString("r") });
				httpResponseMessage = httpClient.SendAsync(httpRequestMessage).Result;
				OutputSeparator();

				// _________________________________________________________
				var newCar = new Car()
				             	{
				             		BuildYear = 1999,
				             		Make = "Kia",
				             		Model = "Supra",
				             		MaxSpeed = 90,
				             		Price = 1300,
				             		WarrantyProvided = false
				             	};

				OutputHeader("Inserting a car with ID and get URL");
				httpResponseMessage = httpClient.PostAsync("Cars", 
					new ObjectContent<Car>(newCar, new MediaTypeFormatterCollection().JsonFormatter
						)).Result;

				var locationOfNewCar = httpResponseMessage.Headers.Location;
				OutputSeparator();

				// _________________________________________________________
				OutputHeader("Verify that the ETag and last modified for Cars has changed");
				httpResponseMessage = httpClient.GetAsync("Cars").Result;
				string newCarsETag = httpResponseMessage.Headers.ETag.ToString();
				var newLastModified = httpResponseMessage.Content.Headers.LastModified ?? DateTimeOffset.UtcNow.AddDays(-1);
				Console.WriteLine("Old values '{0}' '{1}'", carsETag, lastModified);
				Console.WriteLine("New values '{0}' '{1}'", newCarsETag, newLastModified);
				OutputSeparator();

				// _________________________________________________________
				OutputHeader("Verify that the ETag and last modified for fastest Cars has changed");
				httpResponseMessage = httpClient.GetAsync("Cars").Result;
				string newFastestCarsETag = httpResponseMessage.Headers.ETag.ToString();
				var newFastestLastModified = httpResponseMessage.Content.Headers.LastModified ?? DateTimeOffset.UtcNow.AddDays(-1);
				Console.WriteLine("Old values '{0}' '{1}'", fastestCarsETag, newFastestCarsETag);
				Console.WriteLine("New values '{0}' '{1}'", fastestLastModified, newFastestLastModified);
				OutputSeparator();

				// _________________________________________________________
				carsETag = newCarsETag;
				lastModified = newLastModified;
				fastestLastModified = newFastestLastModified;
				fastestCarsETag = newFastestCarsETag;

				// _________________________________________________________
				OutputHeader("Get the new car");
				httpResponseMessage = httpClient.GetAsync(locationOfNewCar).Result;
				string newCarETag = httpResponseMessage.Headers.ETag.ToString();
				var newCarLastModified = httpResponseMessage.Content.Headers.LastModified ?? DateTimeOffset.UtcNow.AddDays(-1);
				OutputSeparator();



			}
			catch (Exception e)
			{				
				OutputError(e.ToString());
			}


			Console.WriteLine();
			OutputHeader("Press <ENTER> to exit...");
			Console.Read();
		}

		private static void OutputHeader(string info)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(info);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static void OutputError(string error)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(error);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		private static void OutputSeparator()
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine("-----------------------------------------------------");
		}

	}
}
