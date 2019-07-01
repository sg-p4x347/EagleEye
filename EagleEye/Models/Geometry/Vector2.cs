using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Models.Geometry
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
		public double X;
		public double Y;
		static public Vector2 Zero = new Vector2(0, 0);

		static public Vector2 operator *(Vector2 vector, double scalar)
		{
			return new Vector2(vector.X * scalar, vector.Y * scalar);
		}
		static public Vector2 operator /(Vector2 vector, double scalar)
		{
			return new Vector2(vector.X / scalar, vector.Y / scalar);
		}
		static public Vector2 operator +(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X + b.X, a.Y + b.Y);
		}
		static public Vector2 operator -(Vector2 a, Vector2 b)
		{
			return new Vector2(a.X - b.X, a.Y - b.Y);
		}
		public double LengthSquared()
		{
			return X * X + Y * Y;
		}
		public double Length { get => Math.Sqrt(LengthSquared()); }
		public double Dot(Vector3 b)
		{
			return X * b.X + Y * b.Z;
		}
		public double Dot(Vector2 b)
		{
			return X * b.X + Y * b.Y;
		}
		public Vector2 Normal()
		{
			return new Vector2(-Y, X);
		}
		public Vector2 Normalized()
		{
			return this / Length;
		}
	}
}
