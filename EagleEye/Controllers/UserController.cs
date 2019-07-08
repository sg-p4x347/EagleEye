using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
namespace EagleEye.Controllers
{
	[AllowAnonymous]
	public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View("TempIndex",new Views.User.User());
        }
		[HttpPost]
		public ActionResult Login(Views.User.User user)
		{
			var identity = new ClaimsIdentity(new[] {
				new Claim(ClaimTypes.Role, user.AccessLevel.ToString())
			}, "ApplicationCookie");

			var ctx = Request.GetOwinContext();
			var authManager = ctx.Authentication;
			authManager.SignIn(identity);

			return RedirectToAction("Index", "Home");
		}
		[HttpGet]
		public ActionResult Logout()
		{
			var ctx = Request.GetOwinContext();
			var authManager = ctx.Authentication;

			authManager.SignOut("ApplicationCookie");
			return RedirectToAction("Index", "User");
		}
	}
}