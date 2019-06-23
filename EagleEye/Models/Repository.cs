using System;
using System.Collections;
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

		public static Dictionary<int, T> Models { get; private set; }  = new Dictionary<int, T>();

	}
}