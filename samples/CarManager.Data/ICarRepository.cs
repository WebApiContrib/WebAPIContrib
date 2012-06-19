using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarManager.Data
{
	public interface ICarRepository
	{
		void Add(Car car);
		IEnumerable<Car> Get();
		Car Get(int id);
		void Update(Car car);
		void Delete(int id);
	}
}
