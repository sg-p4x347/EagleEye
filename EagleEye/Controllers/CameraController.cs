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
    }
}