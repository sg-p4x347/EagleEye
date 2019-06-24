using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EagleEye.Controllers
{
	public class AdminController : Controller
	{
        public ActionResult AdminHome()
        {
            ViewBag.Message = "This is the Admin Home.";
            return View();
        }
		public ActionResult AdminLot()
		{
			ViewBag.Message = "Your Lot page.";

			return View();
		}
        public ActionResult CreateLot()
        {
            ViewBag.Message = "Your Create a Lot page.";

            return View();
        }
        public ActionResult RemoveLot()
        {
            ViewBag.Message = "Your remove a lot page.";

            return View();
        }
        public ActionResult EditLot()
        {
            ViewBag.Message = "Your edit a lot page.";

            return View();
        }
    }
}