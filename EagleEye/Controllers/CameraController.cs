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
        // GET: Camera
        public ActionResult Index()
        {
            return View(Repository<Camera>.Models.Values.Select(m => new Views.Camera.Camera(m)).ToList());
        }
		public ActionResult Update(Views.Camera.Camera camera)
		{
			return new EmptyResult();
		}
		public ActionResult ListView()
		{
			return PartialView("List", Repository<Camera>.Models.Values.Select(c => new Views.Camera.Camera(c)).ToList());
		}
		[HttpGet]
		public ActionResult Delete(int id)
		{
			Repository<Camera>.Delete(id);
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