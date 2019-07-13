using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EagleEye.Models;
using System.Threading;
namespace EagleEye.Controllers
{
    public class ParkingLotController : Controller
    {
		/*--------------------------------------------------
		Developer:
			Gage Coates

		Purpose:
			Provides a common interface for requests that
			directly involve ParkingLot and Annotation models

		Dependencies:
			Controller:
				Base MVC controller methods
			ParkingLot:
				The subject of operations
			Annotation:
				Used to hydrate ParkingLot models
				with their child Annotation models
		--------------------------------------------------*/
		public ParkingLotController()
		{
		}
		//--------------------------------------------------
		// Views

		/*--------------------------------------------------
		Purpose:
			Creates a view that lists all parking lot instances

		Returns:
			An html view
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Index()
        {
            return View(Repository<ParkingLot>.Models.Values.Select(lot => new Views.ParkingLot.ParkingLot(lot)).ToList());
        }
		/*--------------------------------------------------
		Purpose:
			Creates a view of the parking lot that allows
			editing of annotations and the name
		Returns:
			An html view
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
		/*--------------------------------------------------
		Purpose:
			Creates a view for users to monitor a parking lot
		Returns:
			An html view
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Monitor(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				return View("Monitor", new Views.ParkingLot.ParkingLot(lot));
			}
			return new HttpNotFoundResult();
		}
		/*--------------------------------------------------
		Purpose:
			Creates a form view for instantiation of a new
			parking lot
		Returns:
			An html view
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult New()
		{
			return View("New");
		}



		/*--------------------------------------------------
		Purpose:
			Creates a base64 encoded image in the response body
			for the parking lot baseline
		Returns:
			A base64 encoded image in the response body
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Baseline(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot) && lot.Baseline != null)
			{
				// Create a memory stream for the bitmap to write to
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

		/*--------------------------------------------------
		Purpose:
			Updates a parking lot baseline to the current
			image held in the associated Camera instance
		Returns:
			An empty result
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult UpdateBaseline(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				lot.Baseline = lot.Camera.CurrentImage;
				EagleEyeConfig.ExportDatabase();
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}
		//--------------------------------------------------
		// REST

		/*--------------------------------------------------
		Purpose:
			Creates a new parking lot from a name and
			camera ID
		Returns:
			An empty result
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Create(string name, int cameraID)
		{
			if (Repository<Camera>.Contains(cameraID))
			{
				Camera camera = Repository<Camera>.Get(cameraID);
				ParkingLot newLot = new ParkingLot(Repository<ParkingLot>.NextID, name, camera);
				Repository<ParkingLot>.Add(newLot);

				EagleEyeConfig.ExportDatabase();
			}
			return new EmptyResult();
		}
		/*--------------------------------------------------
		Purpose:
			Retrieves a parking lot instance by ID
		Returns:
			A json encoded response of the parking lot model
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Get(int id)
		{
			ParkingLot lot;
			if (TryGetLot(id, out lot))
			{
				lock (lot)
				{
					return Json(new Views.ParkingLot.ParkingLot(lot), JsonRequestBehavior.AllowGet);
				}
			}
			return new HttpNotFoundResult();
		}
		/*--------------------------------------------------
		Purpose:
			Updates a parking lot model to mirror the provided
			ParkingLot viewmodel
		Returns:
			An empty result
		--------------------------------------------------*/
		[HttpPost]
		public ActionResult Update(Views.ParkingLot.ParkingLot lot)
		{
			ParkingLot model;
			if (TryGetLot(lot.ID, out model))
			{
				lock (model)
				{
					model.Annotations.Clear();
					foreach (var annotation in lot.Annotations)
					{
						var modelAnnotation = new Annotation(model.Annotations.Count > 0 ? model.Annotations.Max(a => a.ID) + 1 : 0, (Annotation.AnnotationType)Enum.Parse(typeof(Annotation.AnnotationType), annotation.Type));
						model.Annotations.Add(modelAnnotation);
						foreach (var point in annotation.Points)
						{
							modelAnnotation.Add(new Models.Geometry.Vector2(point.X, point.Y));
						}
					}
				}
				EagleEyeConfig.ExportDatabase();
				return new EmptyResult();
			}
			return new HttpNotFoundResult();
		}
		/*--------------------------------------------------
		Purpose:
			Deletes a parking lot by ID
		Returns:
			An empty result
		--------------------------------------------------*/
		[HttpGet]
		public ActionResult Delete(int id)
		{
			Repository<ParkingLot>.Delete(id);

			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
		/*--------------------------------------------------
		Purpose:
			Tries to get a parking lot by id, returning a boolean
			value for success

		Returns:
			If successful, the model is passed out through
			the lot parameter and true is returned, else
			false is returned
		--------------------------------------------------*/
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