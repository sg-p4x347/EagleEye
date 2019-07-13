using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EagleEye.Controllers
{
	/// <summary>
	//Developer:
	//	Gage Coates

	//Purpose:
	//	Provides a place for non-model related actions

	//Dependencies:
	//	Controller:
	//		Base MVC controller methods
	/// </summary>
	[Authorize]
	public class HomeController : Controller
	{
		/// <summary>
		//Purpose:
		//	Creates a homepage view

		//Returns:
		//	An html view
		/// </summary>
		public ActionResult Index()
		{
			return View();
		}
	}
}