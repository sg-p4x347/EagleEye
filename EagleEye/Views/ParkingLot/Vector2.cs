using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ParkingLot
{
	public class Vector2
	{
		public Vector2() { }
		public Vector2(Models.Vector2 vector)
		{
			X = vector.X;
			Y = vector.Y;
		}
		public double X { get; set; }
		public double Y { get; set; }
	}
}