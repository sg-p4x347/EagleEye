using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EagleEye.Controllers
{
	/*--------------------------------------------------
	Developer:
		Gage Coates

	Purpose:
		Provides a place for non-model related actions

	Dependencies:
		Controller:
			Base MVC controller methods
	--------------------------------------------------*/
	[Authorize]
	public class HomeController : Controller
	{
		/*--------------------------------------------------
		Purpose:
			Creates a homepage view

		Returns:
			An html view
		--------------------------------------------------*/
		public ActionResult Index()
		{
			return View();
		}
	}
}