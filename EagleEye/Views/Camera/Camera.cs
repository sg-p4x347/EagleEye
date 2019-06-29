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
			CurrentImage = "";
			if (camera.CurrentImage != null)
			{
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					camera.CurrentImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
					CurrentImage = System.Convert.ToBase64String(stream.GetBuffer());
				}
			}
		}
		public int ID { get; set; }
		public string Name { get; set; }
		public string CurrentImage { get; set; }
	}
}