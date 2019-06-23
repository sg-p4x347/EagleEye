using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ParkingLot
{
	public class ParkingLot
	{
		public ParkingLot() { }
		public ParkingLot(Models.ParkingLot lot)
		{

		}
		public int ID { get; set; }
		public string Name { get; set; }
		public List<Annotation> Annotations { get; set; }
	}
}