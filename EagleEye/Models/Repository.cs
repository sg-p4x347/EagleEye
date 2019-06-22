using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EagleEye.Models
{
	
	public class Repository<T> where T : class, IID
	{
		public static void Add(T model)
		{
			Models.Add(model.ID, model);
		}
		public static T Get(int id)
		{
			return Models[id];
		}
		public static void Delete(int id)
		{
			Models.Remove(id);
		}
		private static Dictionary<int, T> Models = new Dictionary<int, T>();

	}
}