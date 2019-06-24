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
		Store an associative pair of doubles that represent
		2d cartesian coordinates
	--------------------------------------------------*/
	public struct Vector2
	{
		public Vector2(double x, double y)
		{
			X = x;
			Y = y;
		}
		// The X component
		public double X;
		// The Y component
		public double Y;
	}
}