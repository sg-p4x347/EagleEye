using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EagleEye.Controllers
{
	public class UserController : Controller
	{
        public ActionResult UserHome()
        {
            ViewBag.Message = "Welcome! To the user portal. Click on lot to find a parking spot.";
            return View();
        }
		public ActionResult Lot()
		{
			ViewBag.Message = "Please select a lot to view";

			return View();
		}
	}
}