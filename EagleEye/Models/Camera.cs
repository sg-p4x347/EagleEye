using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

namespace EagleEye.Models
{
	/*--------------------------------------------------
	Developer:
		Gage Coates

	Purpose:
		Defines a unique camera instance

	Dependencies:
		Bitmap:
			Used to define the current image
	--------------------------------------------------*/
	public class Camera : IID
	{

		public Camera(int id,string name)
		{
			ID = id;
			Name = name;
		}
		// A unique identifier
		public int ID { get; private set; } = -1;
		// A human readable identifier (not unique)
		public string Name { get; set; }
		// The most recent image from this camera
		public Bitmap CurrentImage { get; set; }
	}
}