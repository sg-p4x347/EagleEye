using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EagleEye.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
		[HttpPost]
		public ActionResult Login(User model)
		{
			if (!ModelState.IsValid) //Checks if input fields have the correct format
			{
				return View(model);
			}
			using (var db = new MainDbContext())
			{
				var user = db.Users.Where(u => u.UID == model.UID).FirstOrDefault();
				if (user != null)
				{
					var decryptedPassword = CustomDecrypt.Decrypt(user.Password);

					if (model.UID != null && model.Password == decryptedPassword)
					{

						var identity = new ClaimsIdentity(new[] {
						new Claim("Uid", user.UID),
						new Claim(ClaimTypes.Name, user.Name)
					}, "ApplicationCookie");

						var ctx = Request.GetOwinContext();
						var authManager = ctx.Authentication;
						authManager.SignIn(identity);

						return RedirectToAction("Index", "Home");
					}
					else
					{
						ModelState.AddModelError("", "Invalid Password");
					}
				}
				else
				{
					ModelState.AddModelError("", "Invalid UID");
				}
			}
			return View(model);
		}
	}
}