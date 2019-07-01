using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EagleEye.Models.Geometry;

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
	public class Annotation : IID
	{
		public enum AnnotationType
		{
			Parking,
			Isle
		}
		public Annotation(int id, AnnotationType type)
		{
			ID = id;
			Type = type;
		}
		/*--------------------------------------------------
		Purpose:
			Adds a point to the Points collection
		--------------------------------------------------*/
		public void Add(Vector2 point)
		{
			Points.Add(point);
		}
		private double TriangleArea(Vector2 a, Vector2 b, Vector2 c)
		{
			var ab = b - a;
			return ab.Length * 0.5 * Math.Abs(ab.Normal().Normalized().Dot(c - a));
		}
		public double Area { get
			{
				return TriangleArea(Points[0], Points[1], Points[2]) + TriangleArea(Points[0], Points[2], Points[3]);
			}
		}
		public bool Contains(Vector2 point)
		{
			double area = 0.0;
			for (int i = 0; i < Points.Count; i++)
			{
				area += TriangleArea(point, Points[i], Points[(i + 1) % Points.Count]);
			}
			return Math.Abs(area - Area) <= 0.0001; // A small fudge factor to compensate for floating point inacuraccy
		}
		// A unique identifier
		public int ID { get; private set; } = -1;
		// A list of points that define the region
		public List<Vector2> Points { get; private set; } = new List<Vector2>();
		// The type of annotation
		public AnnotationType Type { get; set; } = AnnotationType.Parking;
		// The percentage of occupied pixels
		public double PercentDifference { get; set; }
	}
}