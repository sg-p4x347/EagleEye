using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
namespace EagleEye
{
	public static class HtmlHelpers
	{
		public static bool IsAdmin(this HttpRequestBase request)
		{
			return request.GetOwinContext().Authentication.User.IsInRole(EagleEye.Views.User.AccessLevel.Administrator.ToString());
		}
	}
}