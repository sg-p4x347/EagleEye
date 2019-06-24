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
		// GET: ParkingLot
		public ActionResult Index()
        {
            return View(Repository<ParkingLot>.Models.Values.Select(lot => new Views.ParkingLot.ParkingLot(lot)).ToList());
        }
		[HttpGet]
		public ActionResult Edit(int id)
		{
			return View("Edit", new Views.ParkingLot.ParkingLot(Repository<ParkingLot>.Get(id)));
		}
		[HttpGet]
		public ActionResult Baseline(int id)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				Repository<ParkingLot>.Get(id).Baseline.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
				Response.ContentType = "image/png";
				Response.Write(System.Convert.ToBase64String(stream.GetBuffer()));
			}
			return new EmptyResult();
		}
		[HttpGet]
		public JsonResult Get(int id)
		{
			return Json(new Views.ParkingLot.ParkingLot(Repository<ParkingLot>.Get(id)), JsonRequestBehavior.AllowGet);
		}
		[HttpPost]
		public ActionResult Update(Views.ParkingLot.ParkingLot lot)
		{
			ParkingLot model = Repository<ParkingLot>.Get(lot.ID);
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
    }
}