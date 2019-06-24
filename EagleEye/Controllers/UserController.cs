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
            ViewBag.Message = "Your Home Page";
            return View();
        }
		public ActionResult Lot()
		{
			ViewBag.Message = "Your user Lot page.";

			return View();
		}
	}
}