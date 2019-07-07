using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ParkingLot
{
	public class Path
	{
		public Path() { }
		public int LotID { get; set; }
		public int Goal { get; set; }
		public Vector2 Start { get; set; }
	}
}