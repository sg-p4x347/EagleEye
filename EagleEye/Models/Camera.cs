using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Drawing;
using System.Collections.Concurrent;

namespace EagleEye.Models
{
	/// <summary>
	//Developer:
	//	Gage Coates

	//Purpose:
	//	Defines a unique camera instance that maintains
	//	a current image

	//Dependencies:
	//	Bitmap:
	//		Used to define the current image
	/// </summary>
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
				if (m_currentImage == null)
					return null;

				lock (m_currentImage) {
					return new Bitmap(m_currentImage);
				}
				
			}
			set
			{
				if (m_currentImage != null)
				{
					lock (m_currentImage)
					{
						m_currentImage = value;
						LastUpdate = DateTime.Now;
						Changed(this,new EventArgs());
					}
				} else
				{
					m_currentImage = value;
					LastUpdate = DateTime.Now;
					Changed(this, new EventArgs());
				}
			}
		}
		public event EventHandler Changed = delegate { };
		// The time CurrentImage was last updated
		public DateTime LastUpdate { get; set; }
	}
}