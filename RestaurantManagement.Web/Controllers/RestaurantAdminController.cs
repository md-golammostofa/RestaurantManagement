using RestaurantManagement.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class RestaurantAdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: RestaurantAdmin
        public ActionResult Index()
        {
            string UserRole = User.Identity.GetRUserRole();

            List<clsLink> linkList = new List<clsLink>();

            if (UserRole=="Admin")
            {
                clsLink c1 = new clsLink { TextL = "Items", ActionL = "Index", ControllerL = "RestaurantItem" };
                clsLink c2 = new clsLink { TextL = "Discount", ActionL = "Index", ControllerL = "RestaurantDiscount" };
                clsLink c3 = new clsLink { TextL = "Table", ActionL = "Index", ControllerL = "RestaurantTable" };
                clsLink c4 = new clsLink { TextL = "Employee List", ActionL = "Index", ControllerL = "RestaurantEmployeesMain" };
                clsLink c5 = new clsLink { TextL = "Deleted Invoices", ActionL = "Index", ControllerL = "DeletedInvoices" };
                clsLink c6 = new clsLink { TextL = "Register", ActionL = "Register", ControllerL = "Account" };
               
                linkList.Add(c1);
                linkList.Add(c2);
                linkList.Add(c3);
                linkList.Add(c4);
                linkList.Add(c5);
                linkList.Add(c6);

            }
            else
            {
                clsLink c1 = new clsLink { TextL = "Home", ActionL = "Index", ControllerL = "Home" };
                
                linkList.Add(c1);

            }

            clsAdminIndex m = new clsAdminIndex();
            m.AdminOrNormal = UserRole;
            m.LinkList = linkList;

            return View(m);
        }
    }
}