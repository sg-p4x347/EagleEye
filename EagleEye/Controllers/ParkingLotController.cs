using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EagleEye.Models;
namespace EagleEye.Controllers
{
    public class ParkingLotController : Controller
    {
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			EagleEyeConfig.ExportDatabase();
		}
		//--------------------------------------------------
		// View Actions

		[HttpGet]
		public ActionResult Index()
        {
            return View(Repository<ParkingLot>.Models.Values.Select(lot => new Views.ParkingLot.ParkingLot(lot)).ToList());
        }
		/*--------------------------------------------------
		Purpose:
			Return a view of the parking lot that allows
			editing of annotations and the name
		Returns:
			The Edit view
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Edit(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				return View("Edit", new Views.ParkingLot.ParkingLot(lot));
			}
			return new HttpNotFoundResult();
		}
		[HttpGet]
		public ActionResult New()
		{
			return View("New");
		}


		
		/*--------------------------------------------------
		Purpose:
			Returns a base64 encoded image in the response body
			

		Returns:
			Returns
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Baseline(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot) && lot.Baseline != null)
			{
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
				{
					
					lot.Baseline.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
					Response.ContentType = "image/png";
					Response.Write(System.Convert.ToBase64String(stream.GetBuffer()));
				}
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}
		[HttpGet]
		public ActionResult UpdateBaseline(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				lot.Baseline = lot.Camera.CurrentImage;
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}
		//--------------------------------------------------
		// REST
		[HttpGet]
		public ActionResult Create(string name, int cameraID)
		{
			if (Repository<Camera>.Contains(cameraID))
			{
				Camera camera = Repository<Camera>.Get(cameraID);
				ParkingLot newLot = new ParkingLot(Repository<ParkingLot>.NextID, name, camera);
				Repository<ParkingLot>.Add(newLot);
			}
			return new EmptyResult();
		}
		[HttpGet]
		public ActionResult Get(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				return Json(new Views.ParkingLot.ParkingLot(lot), JsonRequestBehavior.AllowGet);
			}
			return new HttpNotFoundResult();
		}
		[HttpPost]
		public ActionResult Update(Views.ParkingLot.ParkingLot lot)
		{
			ParkingLot model;
			if (TryGetLot(lot.ID, out model))
			{
				model.Annotations.Clear();
				foreach (var annotation in lot.Annotations)
				{
					var modelAnnotation = new Annotation(model.Annotations.Count > 0 ? model.Annotations.Max(a => a.ID) + 1 : 0, (Annotation.AnnotationType)Enum.Parse(typeof(Annotation.AnnotationType), annotation.Type));
					model.Annotations.Add(modelAnnotation);
					foreach (var point in annotation.Points)
					{
						modelAnnotation.Add(new Vector2(point.X, point.Y));
					}
				}
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}
		[HttpGet]
		public ActionResult Delete(int id)
		{
			Repository<ParkingLot>.Delete(id);
			return new EmptyResult();
		}
		private bool TryGetLot(int id, out ParkingLot lot)
		{
			if (Repository<ParkingLot>.Contains(id))
			{
				lot = Repository<ParkingLot>.Get(id);
				return true;
			}
			lot = null;
			return false;
		}
    }
}