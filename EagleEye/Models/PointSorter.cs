using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
namespace EagleEye.Models
{
	public class PointSorter : IComparer<Point>
	{
		public PointSorter(Point center)
		{
			Center = center;
		}
		private Point Center { get; set; }
		public int Compare(Point a, Point b)
		{
			double angleA = Math.Atan2(a.X - Center.X, a.Y - Center.Y);
			if (angleA < 0)
				angleA += 2 * Math.PI;

			double angleB = Math.Atan2(b.X - Center.X, b.Y - Center.Y);
			if (angleB < 0)
				angleB += 2 * Math.PI;

			return angleA.CompareTo(angleB);

		}
	}
}