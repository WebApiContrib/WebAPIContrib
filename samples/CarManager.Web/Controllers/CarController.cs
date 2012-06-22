using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using CarManager.Data;

namespace CarManager.Web.Controllers
{
    public class CarController : ApiController
    {
    	private static ICarRepository _repository;

		static CarController()
		{
			_repository = new CarRepository(); // TODO: add DI
			
		}

		public CarController()
		{
			
		}

    	public CarController(ICarRepository repository)
    	{
    		_repository = repository;
    	}

    	//
        // GET: /Car/id
		public Car Get([FromUri]int id)
		{
			return _repository.Get(id);
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
