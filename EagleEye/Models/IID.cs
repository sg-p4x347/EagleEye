using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Models
{
	/// <summary>
	//Developer:
	//	Gage Coates

	//Purpose:
	//	Defines a common interface for Repository to
	//	interact with models that are uniquely identified

	//Notes:
	//	.NET naming convention calls for interfaces to
	//	begin with 'I' indicating that this is an
	//	interface
	/// </summary>
	public interface IID
	{
		int ID { get; }
	}
}
