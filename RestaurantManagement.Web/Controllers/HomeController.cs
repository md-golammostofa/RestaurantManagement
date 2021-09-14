using RestaurantManagement.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {   // table status
            int TotalTable = db.tblTable.Where(t => t.Statuss == enumStatus.Active).Count() - 1;
            int TotalTableOrdered = db.tblOrder.Where(o => o.OrderStatus == "Ordered").Count();
            ViewBag.LatestExpences = (from t in db.tblDRCR
                                      where t.Statuss == 0 
                                      orderby t.EntryDate descending
                                      select t).Take(10);
            string userRole = User.Identity.GetRUserRole();

            // top moving
            string sqlQuery = string.Format(@"
            select top 6 i.ItemID,i.ItemName,i.ItemCode,i.UnitPrice, topMoving = sum(od.Quantity) 
            from tblOrderDetails od 
            inner join  tblOrder o on o.OrderID = od.OrderID 
            inner join tblItem i on i.ItemID = od.ProductID 
            group by i.ItemID,i.ItemName,i.UnitPrice,i.ItemCode 
            order by sum(od.Quantity) desc ");

            DataTable dt = new DataTable();
            string constr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
            }

            List<clsTableTM> movingList = new List<clsTableTM>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                clsTableTM t = new clsTableTM();
                t.ItemName = dt.Rows[i]["ItemName"].ToString();
                t.UnitPrice = dt.Rows[i]["UnitPrice"].ToString();
                movingList.Add(t);
            }
            ViewBag.movingList = movingList;


            // recently ordered / invoiced item
            string sqlQuery3 = string.Format(@"select t.OrderID,od.ProductName,od.Quantity,Price=convert(decimal(18,2),(od.Quantity* od.UnitPrice)) from (
select top 1 o.OrderID from tblOrder o
 order by OrderID desc
 
  ) t inner join tblOrderDetails od on od.OrderID =t.OrderID ");

            DataTable dt3 = new DataTable();
            string constr3 = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con3 = new SqlConnection(constr3))
            {
                using (SqlCommand cmd3 = new SqlCommand(sqlQuery3))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd3.Connection = con3;
                        sda.SelectCommand = cmd3;
                        sda.Fill(dt3);
                    }
                }
            }

            List<clsTableRL> RecentList = new List<clsTableRL>();
            for (int i = 0; i < dt3.Rows.Count; i++)
            {
                clsTableRL t = new clsTableRL();
                t.ProductName = dt3.Rows[i]["ProductName"].ToString();
                t.Quantity = dt3.Rows[i]["Quantity"].ToString();
                t.Price = dt3.Rows[i]["Price"].ToString();
                RecentList.Add(t);
            }
            ViewBag.RecentList = RecentList;


            //














            ViewBag.AvgDaily = 0; /* AvgDailyOrder();*/



            //




            ViewBag.TotalTableOrdered = TotalTableOrdered;
            ViewBag.FreeTableNum = TotalTable - TotalTableOrdered;

            return View();




        }

        public String AvgDailyOrder()
        {

            // avg daily order 
            DataTable dt2 = new DataTable();
            string sqlQuery2 = string.Format(@"
                select  
                CONVERT(DECIMAL(16,2),t.OrderNumber/ CONVERT(DECIMAL(16,2), (DATEDIFF(DAY, t.startdate, t.enddate) ))) as avgr
  
                from (
                select 
  
                top 1 

                startdate = (SELECT Top 1 
                EntryDate
                FROM tblOrder where Statuss ='0' and OrderStatus= 'Invoiced' order by  EntryDate asc),

                enddate =( SELECT Top 1 
                EntryDate
                FROM tblOrder where Statuss ='0' and OrderStatus= 'Invoiced' order by  EntryDate desc)  ,
                OrderNumber = (Select Count(*) from tblOrder where Statuss ='0' and OrderStatus= 'Invoiced')
                from tblOrder) t  ");
            string constr2 = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection con2 = new SqlConnection(constr2))
            {
                using (SqlCommand cmd2 = new SqlCommand(sqlQuery2))
                {
                    using (SqlDataAdapter sda2 = new SqlDataAdapter())
                    {
                        cmd2.Connection = con2;
                        sda2.SelectCommand = cmd2;
                        sda2.Fill(dt2);
                    }
                }
            }


            return dt2.Rows[0]["avgr"].ToString();

        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var customers = (from i in db.tblItem
                                 where i.ItemName.StartsWith(prefix)
                                 select new
                                 {
                                     label = i.ItemName,
                                     val = i.ItemID,
                                     valUnitPrice = i.UnitPrice
                                 }).ToList();

                return Json(customers);
            }

        }

    }
}