using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarManager.Data
{
	public class Car
	{
		public int Id { get; set; }
		public string Make { get; set; }
		public string Model { get; set; }
		public int BuildYear { get; set; }
		public double Price { get; set; }
		public int MaxSpeed { get; set; }
		public bool WarrantyProvided { get; set; }
	}
}
