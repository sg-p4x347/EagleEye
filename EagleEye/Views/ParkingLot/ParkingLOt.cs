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
			ID = lot.ID;
			Name = lot.Name;
			Annotations = lot.Annotations.Select(a => new Annotation(a)).ToList();
		}
		public int ID { get; set; }
		public string Name { get; set; }
		public List<Annotation> Annotations { get; set; }
	}
}