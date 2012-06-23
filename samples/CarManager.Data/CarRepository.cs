using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarManager.Data
{
	public class CarRepository : ICarRepository
	{
		private static readonly CarRepository _instance = new CarRepository();

		private Dictionary<int, Car> _cars = new Dictionary<int, Car>();
		private int _nextId = 11;

		private CarRepository()
		{
			_cars.Add(1, new Car(){BuildYear = 1997, Id = 1, Make = "Vauxhall",
								Model = "Astra", MaxSpeed = 90, Price = 175, WarrantyProvided = false								
			             	});
			_cars.Add(2, new Car(){BuildYear = 2008, Id = 2, Make = "Nissan",
								Model = "Qashqai", MaxSpeed = 110, Price = 17500, WarrantyProvided = true								
			             	});
			_cars.Add(3, new Car(){BuildYear = 2003, Id = 3, Make = "Toyota",
								Model = "Yaris", MaxSpeed = 100, Price = 3750, WarrantyProvided = false								
			             	});
			_cars.Add(4, new Car(){BuildYear = 2012, Id = 4, Make = "Toyota",
								Model = "Prius", MaxSpeed = 80, Price = 22000, WarrantyProvided = true								
			             	});
			_cars.Add(5, new Car(){BuildYear = 1999, Id = 5, Make = "BMW",
								Model = "X5", MaxSpeed = 120, Price = 15000, WarrantyProvided = false								
			             	});
			_cars.Add(6, new Car(){BuildYear = 2005, Id = 6, Make = "Porsche",
								Model = "911", MaxSpeed = 150, Price = 37500, WarrantyProvided = false								
			             	});
			_cars.Add(7, new Car(){BuildYear = 2009, Id = 7, Make = "Volvo",
								Model = "XC60", MaxSpeed = 130, Price = 25000, WarrantyProvided = true								
			             	});
			_cars.Add(8, new Car(){BuildYear = 2011, Id = 8, Make = "Vauxhall",
								Model = "Corsa", MaxSpeed = 100, Price = 12500, WarrantyProvided = true								
			             	});
			_cars.Add(9, new Car(){BuildYear = 2010, Id = 9, Make = "Audi",
								Model = "A5 Coupe", MaxSpeed = 140, Price = 36000, WarrantyProvided = true								
			             	});
			_cars.Add(10, new Car(){BuildYear = 2008, Id = 10, Make = "BMW",
								Model = "318i", MaxSpeed = 145, Price = 16000, WarrantyProvided = true								
			             	});


		}

		public static CarRepository Instance
		{
			get { return _instance; }
		}

		public void Add(Car car)
		{
			car.Id = _nextId++;
			_cars.Add(car.Id, car);
		}

		public IEnumerable<Car> Get()
		{
			return _cars.Values;
		}

		public Car Get(int id)
		{
			if (_cars.ContainsKey(id))
				return _cars[id];
			else
				return null;
		}

		public void Update(Car car)
		{
			_cars[car.Id] = car;
		}

		public void Delete(int id)
		{
			_cars.Remove(id);
		}
	}
}
