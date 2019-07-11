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
		marked as a specific Type, which is defined by
		the AnnotationType enum

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
			Adds a point to the Points collection, ensuring
			there is a maximum of 4 points sorted in a uniform
			order about their common center
		--------------------------------------------------*/
		public void Add(Vector2 point)
		{
			if (Points.Count < 4)
			{
				Points.Add(point);
				Points.Sort(new PointSorter(Points.Centroid()));
			}
		}
		/*---------------------------------------------------
		Purpose:
			Calculates the area of a triangle defined by
			three Vector2 vertices

		Returns:
			The area of the triangle as a double
		---------------------------------------------------*/
		private double TriangleArea(Vector2 a, Vector2 b, Vector2 c)
		{
			var ab = b - a;
			// Area is defined by 1/2 * base * height
			// Taking the dot product of one edge (ac) with
			// a normal vector perpendicular to another edge (ab)
			// will yield the height of the triangle
			return 0.5 * ab.Length * Math.Abs(ab.Normal().Normalized().Dot(c - a));
		}
		// The convex area bounded by all points
		public double Area {
			get
			{
				// Sums the area of two adjacent triangles
				return TriangleArea(Points[0], Points[1], Points[2]) + TriangleArea(Points[0], Points[2], Points[3]);
			}
		}
		/*---------------------------------------------------
		Purpose:
			Determines whether the given point is contained
			within the annotation

		Returns:
			true if contained, else false
		---------------------------------------------------*/
		public bool Contains(Vector2 point)
		{
			// Sum the area of all 4 triangles (one triangle for each point in the annotation)
			double area = 0.0;
			for (int i = 0; i < Points.Count; i++)
			{
				area += TriangleArea(point, Points[i], Points[(i + 1) % Points.Count]);
			}
			// If the difference between the actual area and the 4 triangles is not equal, 
			// the point must lie outside the annotation
			return Math.Abs(area - Area) <= 0.0001; // A small fudge factor to compensate for floating point inacuraccy
		}
		/*---------------------------------------------------
		Purpose:
			the Separating Axis Theorem (SAT)
			Determines whether two convex sets of points intersect

		Returns:
			true if an intersection is found, else false
		---------------------------------------------------*/
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