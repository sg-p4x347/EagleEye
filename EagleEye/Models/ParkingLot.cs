using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
namespace EagleEye.Models
{
	public class ParkingLot
	{
		/*--------------------------------------------------
		Developer:
			Gage Coates

		Purpose:
			Purpose

		Dependencies:
			Bitmap:
				Defines the baseline image
			Camera:
				A parking lot must have an associated camera
				to compare against the baseline
			Annotation:
				Defines regions in the baseline that should be
				included in calculations
		--------------------------------------------------*/
		public ParkingLot(int id, Camera camera, Bitmap baseline)
		{
			ID = id;
			Camera = camera;
			Baseline = baseline;
		}
		// A unique identifier
		public int ID { get; private set; } = -1;
		// A human readable identifier (not unique)
		public string Name { get; set; }
		// The assocative camera that monitors this parking lot
		public Camera Camera { get; private set; }
		// An image that defines the empty state of the parking lot
		public Bitmap Baseline { get; private set; }
		// A list of annotations that define regions of Parking or Isle space
		public List<Annotation> Annotations { get; private set; } = new List<Annotation>();
		// Only annotations of Parking type
		public IEnumerable<Annotation> ParkingSpaces {
			get
			{
				return Annotations.Where(a => a.Type == Annotation.AnnotationType.Parking);
			}
		}
		// Only annotations of Isle type
		public IEnumerable<Annotation> Isles {
			get
			{
				return Annotations.Where(a => a.Type == Annotation.AnnotationType.Isle);
			}
		}
	}
}