using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.Camera
{
	public class Camera
	{
		public Camera() { }
		public Camera(Models.Camera camera)
		{
			ID = camera.ID;
			Name = camera.Name;
		}
		public int ID { get; set; }
		public string Name { get; set; }
	}
}