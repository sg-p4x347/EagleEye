using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Models
{
	/*--------------------------------------------------
	Developer:
		Gage Coates

	Purpose:
		Defines a set of points that make up a 2D region
		marked as either Parking or Isle

	Dependencies:
		Vector2:
			Used to define points
	--------------------------------------------------*/
	public class Annotation
	{
		public enum AnnotationType
		{
			Parking,
			Isle
		}
		public Annotation(int id)
		{
			ID = id;
		}
		/*--------------------------------------------------
		Purpose:
			Adds a point to the Points collection
		--------------------------------------------------*/
		public void Add(Vector2 point)
		{
			Points.Add(point);
		}
		// A unique identifier
		public int ID { get; private set; } = -1;
		// A list of points that define the region
		public List<Vector2> Points { get; private set; } = new List<Vector2>();
		// The type of annotation
		public AnnotationType Type { get; set; } = AnnotationType.Parking;
	}
}