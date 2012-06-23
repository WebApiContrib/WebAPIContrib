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
    public class CarController : ApiController
    {

    	private ICarRepository _repository;

		public CarController()
		{
			_repository = CarRepository.Instance; // TODO: add DI instead of singleton			
		}

    	public CarController(ICarRepository repository)
    	{
    		_repository = repository;
    	}

    	//
        // GET: /Car/id
		public Car Get([FromUri]int id)
		{
			var car = _repository.Get(id);
			if (car == null)
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

			return car;
		}

		public void Put([FromBody]Car car)
		{
			_repository.Update(car);
		}

		public void Delete([FromUri]int id)
		{
			_repository.Delete(id);
		}
		

    }
}
