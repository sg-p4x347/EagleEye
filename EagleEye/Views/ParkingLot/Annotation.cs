using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.ParkingLot
{
	public class Annotation
	{
		public Annotation() { }
		public Annotation(Models.Annotation annotation)
		{
			ID = annotation.ID;
			Type = annotation.Type.ToString();
			Points = annotation.Points.Select(p => new Vector2(p)).ToList();
		}
		public int ID { get; set; }
		public string Type { get; set; }
		public List<Vector2> Points { get; set; }
	}
}