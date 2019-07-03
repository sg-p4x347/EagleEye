using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
namespace EagleEye.Models
{
	static class ExtensionMethods
	{
		public static bool SameSize(this Bitmap a, Bitmap b)
		{
			return a.Width == b.Width && a.Height == b.Height;
		}
		static public List<Point> FindIslands(this Bitmap source, int count, int thresholdArea)
		{
			int gridSize = (int)Math.Floor(Math.Sqrt((double)thresholdArea));
			List<HashSet<Point>> islands = new List<HashSet<Point>>();
			for (int level = 255; level >= 0; level--)
			{
				islands.Clear();
				for (int x = 0; x < source.Width; x += gridSize)
				{
					for (int y = 0; y < source.Height; y += gridSize)
					{
						Point point = new Point(x, y);
						if (!islands.Any(island => island.Contains(point)))
						{
							int value = source.GetPixel(x, y).R;
							if (value >= level)
							{
								HashSet<Point> island = new HashSet<Point>();
								Queue<Point> newPoints = new Queue<Point>();
								newPoints.Enqueue(point);
								while (newPoints.Count > 0)
								{
									var seed = newPoints.Dequeue();
									if (seed.X >= 0 && seed.X < source.Width && seed.Y >= 0 && seed.Y < source.Height && source.GetPixel(seed.X, seed.Y).R >= level)
									{
										if (!island.Contains(seed))
										{
											island.Add(seed);
											newPoints.Enqueue(new Point(seed.X, seed.Y - 1));
											newPoints.Enqueue(new Point(seed.X - 1, seed.Y));
											newPoints.Enqueue(new Point(seed.X + 1, seed.Y));
											newPoints.Enqueue(new Point(seed.X, seed.Y + 1));
										}
										else
										{
											var neighbor = islands.Find(i => i.Contains(seed));
											if (neighbor != null)
											{
												// Union islands
												islands.Remove(neighbor);
												island.UnionWith(neighbor);
											}
										}
									}
								}
								islands.Add(island);
								if (islands.Count(i => i.Count >= thresholdArea) >= count)
									goto done;
							}
						}
					}
				}

			}
			done:;
			List<Point> points = new List<Point>();
			foreach (var island in islands.Where(i => i.Count >= thresholdArea))
			{
				Point average = new Point(0, 0);
				foreach (var point in island)
				{
					average.X += point.X;
					average.Y += point.Y;
				}
				average.X /= island.Count;
				average.Y /= island.Count;
				points.Add(average);
			}
			return points;
		}
		// Normalizes the green channel as a percent of the total
		static public Bitmap GreenShift(this Bitmap source)
		{
			Bitmap result = new Bitmap(source.Width, source.Height);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					Color color = source.GetPixel(x, y);
					int total = (color.R + color.B + color.G);
					int g = total != 0 ? (int)Math.Min(255, (double)(color.G + color.B) * 255 / total) : 0;
					result.SetPixel(x, y, Color.FromArgb(g, g, g));
				}
			}
			return result;
		}
		static public Bitmap Posterize(this Bitmap source, int threshold = 382)
		{
			Bitmap result = new Bitmap(source.Width, source.Height);
			for (int x = 0; x < source.Width; x++)
			{
				for (int y = 0; y < source.Height; y++)
				{
					var pixel = source.GetPixel(x, y);
					if (pixel.R + pixel.G + pixel.B > threshold)
					{
						result.SetPixel(x, y, Color.White);
					}
					else
					{
						result.SetPixel(x, y, Color.Black);
					}
				}
			}
			return result;
		}
		static public Bitmap Difference(this Bitmap a, Bitmap b)
		{
			Bitmap result = new Bitmap(a.Width, a.Height);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					Color colorA = a.GetPixel(x, y);
					Color colorB = b.GetPixel(x, y);
					result.SetPixel(x, y, Color.FromArgb(Math.Abs(colorA.R - colorB.R), Math.Abs(colorA.G - colorB.G), Math.Abs(colorA.B - colorB.B)));

				}
			}
			return result;
		}
		static public Tuple<int,int,int> AverageDifference(this Bitmap a, Bitmap b)
		{
			int red = 0, green = 0, blue = 0;
			
			for (int x = 0; x < a.Width; x++)
			{
				for (int y = 0; y < a.Height; y++)
				{
					Color colorA = a.GetPixel(x, y);
					Color colorB = b.GetPixel(x, y);
					red += colorA.R - colorB.R;
					green += colorA.G - colorB.G;
					blue += colorA.B - colorB.B;
				}
			}
			int pixelCount = a.Width * a.Height;
			return new Tuple<int,int,int>(red / pixelCount, green / pixelCount, blue / pixelCount);
		}
		static public Bitmap Add(this Bitmap source, Tuple<int,int,int> offset)
		{
			Bitmap result = new Bitmap(source);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					Color baseColor = source.GetPixel(x, y);
					result.SetPixel(x, y, Color.FromArgb(
						Math.Max(0, Math.Min(255, baseColor.R + offset.Item1)),
						Math.Max(0, Math.Min(255, baseColor.G + offset.Item2)),
						Math.Max(0, Math.Min(255, baseColor.B + offset.Item3))
					));
				}
			}
			return result;
		}
		static public void Fill(this Bitmap source, Color color)
		{
			for (int x = 0; x < source.Width; x++)
			{
				for (int y = 0; y < source.Height; y++)
				{
					source.SetPixel(x, y, color);
				}
			}
		}
		static public int Overlap(this Bitmap a, Bitmap b)
		{
			int overlap = 0;
			for (int x = 0; x < a.Width; x++)
			{
				for (int y = 0; y < a.Height; y++)
				{
					var pixelA = a.GetPixel(x, y);
					var pixelB = b.GetPixel(x, y);
					overlap += Math.Min(pixelA.R, pixelB.R) + Math.Min(pixelA.G, pixelB.G) + Math.Min(pixelA.B, pixelB.B);
				}
			}
			return overlap;
		}
		static public Bitmap Crop(this Bitmap source, double percent)
		{
			int width = (int)(source.Width * percent);
			int height = (int)(source.Height * percent);
			return source.Clone(new Rectangle((source.Width - width) / 2, (source.Height - height) / 2, width, height), source.PixelFormat);
		}
		static public Bitmap Scale(this Bitmap source, double scale)
		{
			Bitmap result = new Bitmap((int)(source.Width * scale), (int)(source.Height * scale));
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					result.SetPixel(x, y, source.GetPixel((int)Math.Max(0, Math.Min(source.Width, x / scale)), (int)Math.Max(0, Math.Min(source.Height, y / scale))));
				}
			}
			return result;
		}
		static public Bitmap Scale(this Bitmap source, int width)
		{
			double scale = (double)width / (double)source.Width;
			return Scale(source, scale);
		}
		static public int Sum(this Bitmap source)
		{
			int sum = 0;
			for (int x = 0; x < source.Width; x++)
			{
				for (int y = 0; y < source.Height; y++)
				{
					var pixel = source.GetPixel(x, y);
					sum += pixel.R + pixel.G + pixel.B;
				}
			}
			return sum;
		}
		static public Bitmap Transform(this Bitmap source, Geometry.Matrix matrix)
		{
			Bitmap result = new Bitmap(source.Width, source.Height);
			for (int x = 0; x < result.Width; x++)
			{
				for (int y = 0; y < result.Height; y++)
				{
					// Inverse transform. e.g. take the destination pixel and find the corresponding source pixel
					Geometry.Vector3 sourcePoint = matrix * new Geometry.Vector3((double)x, (double)y, 1);
					int sx = (int)Math.Round(sourcePoint.X / sourcePoint.Z);
					int sy = (int)Math.Round(sourcePoint.Y / sourcePoint.Z);
					if (sx >= 0 && sx < source.Width && sy >= 0 && sy < source.Height)
						result.SetPixel(x, y, source.GetPixel(sx, sy));
				}
			}
			return result;
		}
		static public double Value(this Color color)
		{
			// 765.0 is 255 * 3, e.g. the maximum value
			return (color.R + color.G + color.B) / 765.0;
		}
	}
}
