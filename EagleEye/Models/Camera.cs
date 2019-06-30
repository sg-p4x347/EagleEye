using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Drawing;
using System.Collections.Concurrent;

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
			CurrentImage = new Bitmap(400, 300);
			CurrentImage.Fill(Color.Black);
		}
		// A unique identifier
		public int ID { get; private set; } = -1;
		// A human readable identifier (not unique)
		public string Name { get; set; }
		// The most recent image from this camera
		private Bitmap m_currentImage;
		public Bitmap CurrentImage {
			get
			{
				
				return new Bitmap(m_currentImage);
			}
			set
			{

				lock (m_currentImage)
				{
					m_currentImage = value;
					LastUpdate = DateTime.Now;
				}
			}
		}
		// The time CurrentImage was last updated
		public DateTime LastUpdate { get; set; }
	}
}