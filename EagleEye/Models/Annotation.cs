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
			Isle,
			Constant
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
		private bool SAT(IList<Vector2> A, IList<Vector2> B)
		{
			List<Vector2> axes = new List<Vector2>();
			if (A.Count > 1)
				for (int i = 0; i < A.Count; i++)
					axes.Add((A[i] - A[(i + 1) % 4]).Normal().Normalized());
			if (B.Count > 1)
				for (int i = 0; i < B.Count; i++)
					axes.Add((B[i] - B[(i + 1) % 4]).Normal().Normal());
			foreach (var axis in axes)
			{
				var projAmin = double.PositiveInfinity;
				var projBmin = double.PositiveInfinity;
				var projAmax = double.NegativeInfinity;
				var projBmax = double.NegativeInfinity;

				foreach (var p in A)
				{
					var proj = p.Dot(axis);
					if (proj < projAmin)
						projAmin = proj;
					if (proj > projAmax)
						projAmax = proj;
				}
				foreach (var p in B)
				{
					var proj = p.Dot(axis);
					if (proj < projBmin)
						projBmin = proj;
					if (proj > projBmax)
						projBmax = proj;
				}
				// A separating axis has been found, early out with false
				if (!(projAmin < projBmax && projBmin < projAmax))
					return false;
			}
			// Search was exhaustive, there must be an intersection
			return true;
		}
		public bool Intersects(Vector2 start, Vector2 lineDir)
		{
			Vector2 normal = lineDir.Normal().Normalized();
			int? direction = null;
			foreach (Vector2 point in Points)
			{
				double projection = normal.Dot(point - start);

				if (direction == null)
				{
					direction = Math.Sign(projection);
				} else if (direction != Math.Sign(projection)) {
					return true;
				}
			}
			return false;
		}
		public List<Vector2> Midpoints()
		{
			List<Vector2> midpoints = new List<Vector2>();
			for (int i = 0; i < 4; i++)
			{
				Vector2 a = Points[i];
				Vector2 b = Points[(i + 1) % 4];
				midpoints.Add((a + b) / 2);
			}
			return midpoints;
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