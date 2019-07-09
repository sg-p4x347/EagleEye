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
		public CameraController()
		{
		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
		[HttpGet]
		[Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View(Repository<Camera>.Models.Values.Select(m => new Views.Camera.Camera(m)).ToList());
        }
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult ListView()
		{
			return PartialView("List", Repository<Camera>.Models.Values.Select(c => new Views.Camera.Camera(c)).ToList());
		}
        [HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Client()
        {
            return View();
        }
		//--------------------------------------------------
		// REST
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
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Create(string name)
		{
			Repository<Camera>.Add(new Camera(Repository<Camera>.NextID, name));
			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
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
		[HttpGet]
		[Authorize(Roles = "Administrator")]
		public ActionResult Delete(int id)
		{
			Repository<Camera>.Delete(id);

			EagleEyeConfig.ExportDatabase();
			return new EmptyResult();
		}
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