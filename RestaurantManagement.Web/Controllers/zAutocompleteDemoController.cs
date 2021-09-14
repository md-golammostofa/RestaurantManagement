using RestaurantManagement.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class zAutocompleteDemoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: zAutocompleteDemo
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string CustomerName, string CustomerId)
        {
            ViewBag.Message = "CustomerName: " + CustomerName + " CustomerId: " + CustomerId;
            return View();
        }

        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            var customers = (from i in db.tblItem
                             where i.ItemName.StartsWith(prefix)
                             select new
                             {
                                 label = i.ItemName + "_"+i.ItemCode+"_" +i.UnitPrice,
                                 val = i.ItemID
                             }).ToList();

            return Json(customers);
        }

    }
}