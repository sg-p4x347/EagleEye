using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;

namespace EagleEye.Models
{
	/*--------------------------------------------------
	Developer:
		Gage Coates

	Purpose:
		Creates a standard interface for CRUD operations
		on classes that implement the IID interface

	Dependencies:
		IID:
			Allows for uniquely identifying objects
		T:
			The subject class that implements IID

	Notes:
		All methods are static and state is static
	--------------------------------------------------*/
	public class Repository<T> where T : class, IID
	{
		/*---------------------------------------------------
		Purpose:
			Adds an object to the internal collection
		---------------------------------------------------*/
		public static void Add(T model)
		{
			Models.TryAdd(model.ID, model);
		}
		/*---------------------------------------------------
		Purpose:
			Gets an object by id

		Returns:
			An object T associated with the given id
		---------------------------------------------------*/
		public static T Get(int id)
		{
			T model;
			Models.TryGetValue(id, out model);
			return model;
		}
		/*---------------------------------------------------
		Purpose:
			Tests if an object is contained with the given id

		Returns:
			true if contained, else false
		---------------------------------------------------*/
		public static bool Contains(int id)
		{
			return Models.ContainsKey(id);
		}
		/*---------------------------------------------------
		Purpose:
			Removes an object with the given id, if it exists
		---------------------------------------------------*/
		public static void Delete(int id)
		{
			T model;
			Models.TryRemove(id, out model);
		}
		/*---------------------------------------------------
		Purpose:
			Provides a unique ID relative to the current
			set of known ids

		Returns:
			A unique integer id
		---------------------------------------------------*/
		public static int NextID
		{
			get
			{
				return Models.Count > 0 ? Models.Keys.Max() + 1 : 0;
			}
		}
		// The static thread-safe mapping of objects to their id
		// Operations are made in constant time due to hashing
		public static ConcurrentDictionary<int, T> Models { get; private set; }  = new ConcurrentDictionary<int, T>();

	}
}