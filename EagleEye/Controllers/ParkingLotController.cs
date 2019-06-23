using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EagleEye.Models;
namespace EagleEye.Controllers
{
    public class ParkingLotController : Controller
    {
        // GET: ParkingLot
        public ActionResult Index()
        {
            return View(Repository<ParkingLot>.Models.Values.Select(lot => new Views.ParkingLot.ParkingLot(lot)).ToList());
        }
		[HttpGet]
		public ActionResult Edit(int id)
		{
			return View("Edit", new Views.ParkingLot.ParkingLot(Repository<ParkingLot>.Get(id)));
		}
    }
}