using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Views.User
{
	public enum AccessLevel
	{
		User,
		Administrator
	}
	public class User
	{
		public User() { }
		public AccessLevel AccessLevel { get; set; }
	}
}