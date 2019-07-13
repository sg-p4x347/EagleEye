using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EagleEye.Models;
namespace EagleEye.Controllers
{

    public class CameraController : Controller
    {
		/// <summary>
		//Developer:
		//	Gage Coates

		//Purpose:
		//	Provides a common interface for requests that
		//	directly involve Camera models

		//Dependencies:
		//	Controller:
		//		Base MVC controller methods
		//	Camera:
		//		The subject of operations
		//	Repository:
		//		Used to retrieve and update Camera instances
		/// </summary>
		public CameraController()
		{
		}
		//--------------------------------------------------
		// Views

		/// <summary>
		//Purpose:
		//	Creates a view that lists all cameras

		//Returns:
		//	An html view
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View(Repository<Camera>.Models.Values.Select(m => new Views.Camera.Camera(m)).ToList());
        }
		/// <summary>
		//Purpose:
		//	Creates a partial view that defines a select list
		//	of all cameras

		//Returns:
		//	An html partial view
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult ListView()
		{
			return PartialView("List", Repository<Camera>.Models.Values.Select(c => new Views.Camera.Camera(c)).ToList());
		}
		/// <summary>
		//Purpose:
		//	Creates a view that allows for the setup of a
		//	camera instace by name

		//Returns:
		//	An html view
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Client()
        {
            return View();
        }
		//--------------------------------------------------
		// REST

		/// <summary>
		//Purpose:
		//	Creates a Json representation of a camera
		//	model

		//Returns:
		//	A json response body
		/// </summary>
		[HttpGet]
		public ActionResult Get(int id)
		{
			Camera camera;
			if (TryGetCamera(id, out camera))
			{
				return Json(new Views.Camera.Camera(camera), JsonRequestBehavior.AllowGet);
			}
			return new HttpNotFoundResult();
		}
		/// <summary>
		//Purpose:
		//	Creates a new Camera instance by name

		//Returns:
		//	An empty response
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Create(string name)
		{
			Repository<Camera>.Add(new Camera(Repository<Camera>.NextID, name));
			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
		/// <summary>
		//Purpose:
		//	Updates a Camera model

		//Returns:
		//	An empty response
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Administrator")]
		public ActionResult Update(Views.Camera.Camera camera)
		{
			Camera model = Repository<Camera>.Models.Values.FirstOrDefault(c => c.Name == camera.Name);
			if (model == null)
			{
				model = new Camera(Repository<Camera>.NextID, camera.Name);
				Repository<Camera>.Add(model);

				EagleEyeConfig.ExportDatabase();
			}
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
			{
				
				byte[] buffer = Convert.FromBase64String(camera.CurrentImage);
				stream.Write(buffer, 0, buffer.Length);
				model.CurrentImage = System.Drawing.Bitmap.FromStream(stream) as System.Drawing.Bitmap;
			}
			return new EmptyResult();
		}
		/// <summary>
		//Purpose:
		//	Deletes a Camera model by id

		//Returns:
		//	An empty response
		/// </summary>
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Delete(int id)
		{
			Repository<Camera>.Delete(id);

			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
		/// <summary>
		//Purpose:
		//	Tries to get a camera by id, returning a boolean
		//	value for success

		//Returns:
		//	If successful, the model is passed out through
		//	the camera parameter and true is returned, else
		//	false is returned
		/// </summary>
		private bool TryGetCamera(int id, out Camera camera)
		{
			if (Repository<Camera>.Contains(id))
			{
				camera = Repository<Camera>.Get(id);
				return true;
			}
			camera = null;
			return false;
		}
    }
}