using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Models
{
	public class ImageStream : HttpRequestBase
	{
		public ImageStream()
		{
		}
		System.IO.Stream Stream { get => InputStream; }
	}
}