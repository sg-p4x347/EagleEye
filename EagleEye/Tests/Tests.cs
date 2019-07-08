﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EagleEye.Models;
namespace EagleEye.Tests
{
	public static class Tests
	{
		public static void RunTests() {
			EagleEye.Models.Geometry.Vector2 vector = new Models.Geometry.Vector2(1, 0);
			//JsonImport();
			Navigation();
			Intersection();
		}
		static ParkingLot TestLot()
		{
			ParkingLot lot = new ParkingLot(0, "Test Lot", new Camera(0,"Test Camera"));
			Annotation isle = new Annotation(0, Annotation.AnnotationType.Isle);
			isle.Points.Add(new Models.Geometry.Vector2(0, 0));
			isle.Points.Add(new Models.Geometry.Vector2(0.5, 0));
			isle.Points.Add(new Models.Geometry.Vector2(0.5, 0.5));
			isle.Points.Add(new Models.Geometry.Vector2(0, 0.5));
			lot.Annotations.Add(isle);
			Annotation isle2 = new Annotation(0, Annotation.AnnotationType.Isle);
			isle2.Points.Add(new Models.Geometry.Vector2(0, 0.5));
			isle2.Points.Add(new Models.Geometry.Vector2(1, 0.5));
			isle2.Points.Add(new Models.Geometry.Vector2(1, 1));
			isle2.Points.Add(new Models.Geometry.Vector2(0, 1));
			lot.Annotations.Add(isle2);
			Annotation space = new Annotation(1, Annotation.AnnotationType.Parking);
			space.Points.Add(new Models.Geometry.Vector2(0.5, 0));
			space.Points.Add(new Models.Geometry.Vector2(1, 0));
			space.Points.Add(new Models.Geometry.Vector2(1, 0.25));
			space.Points.Add(new Models.Geometry.Vector2(0.5, 0.25));
			lot.Annotations.Add(space);
			Annotation space2 = new Annotation(1, Annotation.AnnotationType.Parking);
			space2.Points.Add(new Models.Geometry.Vector2(0.75, 0));
			space2.Points.Add(new Models.Geometry.Vector2(1, 0));
			space2.Points.Add(new Models.Geometry.Vector2(1, 0.25));
			space2.Points.Add(new Models.Geometry.Vector2(0.75, 0.25));
			lot.Annotations.Add(space2);
			return lot;
		}
		static void JsonImport()
		{
			Json json = Json.Import(new System.IO.StreamReader(HttpRuntime.AppDomainAppPath + "/App_Data/test.json"));
			if (json.Type != Models.Json.JsonType.Object)
			{
				throw new Exception("Json did not parse root as Object type");
			} else
			{
				if (!json.ContainsKey("test"))
				{
					throw new Exception("Json did not contain the correct key");
				} else
				{
					Json value = json["test"];
					if (value == null)
					{
						throw new Exception();
					} else
					{
						if (value.Count != 5)
						{
							throw new Exception();
						}
					}
				}
			}
			using (System.IO.StreamWriter stream = new System.IO.StreamWriter(HttpRuntime.AppDomainAppPath + "/App_Data/test_export.json")) {
				json.Export(stream);
			}
		}
		static void Intersection()
		{
			Annotation isle = new Annotation(0, Annotation.AnnotationType.Isle);
			isle.Points.Add(new Models.Geometry.Vector2(0, 0));
			isle.Points.Add(new Models.Geometry.Vector2(0.5, 0));
			isle.Points.Add(new Models.Geometry.Vector2(0.5, 0.5));
			isle.Points.Add(new Models.Geometry.Vector2(0, 0.5));
			bool test = isle.Intersects(new Models.Geometry.Vector2(0.5, 1), new Models.Geometry.Vector2(1, -.5));
		}
		static void Navigation()
		{
			ParkingLot lot = TestLot();
			var context = new NavigationContext(lot.Annotations);
			//var path = context.AStar(new Models.Geometry.Vector2(0, 0), new Models.Geometry.Vector2(0.875, 0.5));
		}
	}
}