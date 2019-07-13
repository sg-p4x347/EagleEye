using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
namespace EagleEye.Controllers
{
	/*--------------------------------------------------
	Developer:
		Gage Coates

	Purpose:
		Provides authentication actions for unauthenticated
		users to login to the system as either an administrator
		or standard user

	Dependencies:
		Controller:
			Base MVC controller methods
	--------------------------------------------------*/
	[AllowAnonymous]
	public class UserController : Controller
    {
		/*--------------------------------------------------
		Purpose:
			Creates a login view

		Returns:
			An html view
		--------------------------------------------------*/
		[HttpGet]
        public ActionResult Index()
        {
            return View("Login",new Views.User.User());
        }
		/*--------------------------------------------------
		Purpose:
			Logs a user in and provisions them with the
			appropriate role. Subsiquent requests are
			authenticated with this role

		Returns:
			A redirect to /Home/Index
		--------------------------------------------------*/
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
		/*--------------------------------------------------
		Purpose:
			Logs a user out and unauthenticates requests

		Returns:
			A redirect to /User/Index
		--------------------------------------------------*/
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