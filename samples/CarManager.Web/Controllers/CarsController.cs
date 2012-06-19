using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CarManager.Data;

namespace CarManager.Web.Controllers
{
    public class CarsController : ApiController
    {
    	private ICarRepository _repository;

		public CarsController()
		{
			_repository = new CarRepository(); // TODO: add DI
		}

    	public CarsController(ICarRepository repository)
    	{
    		_repository = repository;
    	}

    	//
        // GET: api/Cars/
		[ActionName("")]
		[HttpGet]
		public IEnumerable<Car> Get()
		{
			return _repository.Get();
		}

		//
		// GET: api/Cars/MostExpensive
		[HttpGet]
		public IEnumerable<Car> MostExpensive()
		{
			return _repository.Get()
				.OrderByDescending(x => x.Price)
				.Take(5);
		}

		//
		// GET: api/Cars/Fastest
		[HttpGet]
		public IEnumerable<Car> Fastest()
		{
			return _repository.Get()
				.OrderByDescending(x => x.MaxSpeed)
				.Take(5);
		}

		//
		// GET: api/Cars/WithWarranty
		[HttpGet]
		public IEnumerable<Car> WithWarranty()
		{
			return _repository.Get()
				.Where(c => c.WarrantyProvided);
		}

		[ActionName("")]
		[HttpPost]
		public HttpResponseMessage Post(Car car)
		{
			_repository.Add(car);
			var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created);
			httpResponseMessage.Headers.Location = new Uri(Request.RequestUri.ToString() + "/" + car.Id.ToString());
			return httpResponseMessage;
		}
    }
}
