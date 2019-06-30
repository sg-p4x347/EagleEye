using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace EagleEye.Models
{
	
	public class Repository<T> where T : class, IID
	{
		public static void Add(T model)
		{
			Models.TryAdd(model.ID, model);
		}
		public static T Get(int id)
		{
			T model;
			Models.TryGetValue(id, out model);
			return model;
		}
		public static bool Contains(int id)
		{
			return Models.ContainsKey(id);
		}
		public static void Delete(int id)
		{
			T model;
			Models.TryRemove(id, out model);
		}
		public static int NextID
		{
			get
			{
				return Models.Count > 0 ? Models.Keys.Max() + 1 : 0;
			}
		}

		public static ConcurrentDictionary<int, T> Models { get; private set; }  = new ConcurrentDictionary<int, T>();

	}
}