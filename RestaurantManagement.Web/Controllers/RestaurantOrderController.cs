using CrystalDecisions.CrystalReports.Engine;
using RestaurantManagement.Web.Models;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class RestaurantOrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: RestaurantOrder
        public ActionResult Index(string startdate, string todate, string iid, string printdownload)
        {
            //
            string shopcode = "";
            string usercode = "";

            //var result ;
            DateTime sDate = DateTime.Today;
            DateTime tDate = DateTime.Now;
         

            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate))
            {

         

                if (printdownload == "print")
                {

                    var sd = startdate + " 12:00:00 AM";
                    DateTime sd1 = Convert.ToDateTime(sd);
                    var td = todate + " 11:59:59 PM";

                    var td1 = Convert.ToDateTime(td);


                    string sqlQuery = string.Format(@"select od.ProductName,od.Quantity,od.UnitPrice,od.OrderID
,TotalRowPrice=convert(decimal(18,2),(od.Quantity*od.UnitPrice)) 
,o.TotalAmount
,o.DiscountName,o.TotalDiscount
,o.NetTotal
,o.EntryDate,o.Remarks
,TotalSales=(select  sum(tdd.NetTotal) from tblOrder tdd where tdd.OrderStatus='Invoiced' and CONVERT(date,tdd.EntryDate) BETWEEN CONVERT(date,'{0}') and CONVERT(date,'{1}') )
from tblOrderDetails od
inner join tblOrder o on o.OrderID=od.OrderID
where o.OrderStatus='Invoiced' and o.Statuss='0' and CONVERT(date,o.EntryDate) BETWEEN CONVERT(date,'{0}') and CONVERT(date,'{1}')", sd1.ToString(), td1.ToString());
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

                    ReportDocument rd = new ReportDocument();
                    rd.Load(Path.Combine(Server.MapPath("~/Reporting/Crystals/rptInvoices.rpt")));


                    rd.SetDataSource(dt);
                    Response.Buffer = false;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    // File(stream, "application/pdf", "InvoiceBills.pdf");
                    //  rd.PrintToPrinter(1, false, 1, 1);
                    return File(stream, "application/pdf", "InvoiceBills.pdf");


                    //ReportInvoiceDateWise(sd1.ToString(),td1.ToString());
                }
               
            }











            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && string.IsNullOrEmpty(iid))
            {
                
                var sd = startdate + " 12:00:00 AM";
                DateTime sd1 = Convert.ToDateTime(sd);

                var td = todate + " 11:59:59 PM";

                var td1 = Convert.ToDateTime(td);
                // var result = db.tblOrder.Where(o => o.EntryDate >= sd && o.EntryDate <= td).ToList();
                  var result = db.tblOrder.Where(o => o.EntryDate >= sd1 && o.EntryDate <= td1 && o.Statuss==0).OrderByDescending(o => o.OrderID).ToList();
                // var result = db.Database.SqlQuery<clsOrderDetails>("spGetInvoiceList @datestart = {0}, @dateend= {1}",sd, td).ToList();
                ViewBag.todaysdate = startdate;
                ViewBag.todayedate = todate;

                return View(result);
            }
            //else if(!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate) && !string.IsNullOrEmpty(iid))
            //{       
            //    var sd = startdate + " 12:00:00 AM";
            //    DateTime sd1 = Convert.ToDateTime(sd);
            //    var td = todate + " 11:59:59 PM";

            //    var td1 = Convert.ToDateTime(td);
            //    if (int.TryParse(iid,out int invoice)) { }

                
            //    // var result = db.tblOrder.Where(o => o.EntryDate >= sd && o.EntryDate <= td).ToList();
            //    var result = db.tblOrder.Where(o => o.EntryDate >= sd1 && o.EntryDate <= td1 && o.OrderID == invoice ).ToList();
            //    // var result = db.Database.SqlQuery<clsOrderDetails>("spGetInvoiceList @datestart = {0}, @dateend= {1}",sd, td).ToList();

            //    ViewBag.todaysdate = startdate;
            //    ViewBag.todayedate = todate;

            //    return View(result);


            //}
            else if (!string.IsNullOrEmpty(iid))
            {
                if (int.TryParse(iid, out int invoice)) { }
                var result = db.tblOrder.Where(o => o.OrderID == invoice && o.Statuss == 0 && o.OrderStatus=="Invoiced").OrderByDescending(o => o.OrderID).ToList();
               
                return View(result);
            }
            else
            {
                

                var result = db.tblOrder.Where(o => o.EntryDate >= sDate && o.Statuss == 0 && o.OrderStatus == "Invoiced").OrderByDescending(o=>o.OrderID).ToList();
                ViewBag.todaysdate = sDate.Date.ToString("MM/dd/yyyy");
                ViewBag.todayedate = tDate.Date.ToString("MM/dd/yyyy");
                return View(result);
            }
        }




        // report print  date wise 

        public ActionResult ReportInvoiceDateWise(String startdate, String todate)
        {

           
            string sqlQuery = string.Format(@"select od.ProductName,od.Quantity,od.UnitPrice,od.OrderID
,TotalRowPrice=convert(decimal(18,2),(od.Quantity*od.UnitPrice)) 
,o.TotalAmount
,o.DiscountName,o.TotalDiscount
,o.NetTotal
,o.EntryDate,o.Remarks
,TotalSales=(select  sum(tdd.NetTotal) from tblOrder tdd where tdd.OrderStatus='Invoiced' and CONVERT(date,tdd.EntryDate) BETWEEN CONVERT(date,'{0}') and CONVERT(date,'{1}') )
from tblOrderDetails od
inner join tblOrder o on o.OrderID=od.OrderID
where o.OrderStatus='Invoiced' and CONVERT(date,o.EntryDate) BETWEEN CONVERT(date,'{0}') and CONVERT(date,'{1}')", startdate,todate);
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

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/Crystals/rptInvoices.rpt")));


            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            File(stream, "application/pdf", "InvoiceBills.pdf");
            rd.PrintToPrinter(1, false, 1, 1);
            return File(stream, "application/pdf", "InvoiceBills.pdf");

        }











        //


        // inactive invoice

        public ActionResult InactiveInvoice(int OrderID)
        {
            clsOrder f = db.tblOrder.FirstOrDefault(o => o.OrderID == OrderID);
            f.Statuss = enumStatus.Inactive;
            db.SaveChanges();
            DateTime sDate = DateTime.Today;
            DateTime tDate = DateTime.Now;
            var result = db.tblOrder.Where(o => o.EntryDate >= sDate).ToList();
            ViewBag.todaysdate = sDate.Date.ToString("MM/dd/yyyy");
            ViewBag.todayedate = tDate.Date.ToString("MM/dd/yyyy");
            // return View(result);
           return RedirectToAction("Index", "RestaurantOrder");
        }





        //








        public ActionResult ReportInvoice(int OrderID)
        {

            // RestaurantReports  ReportInvoice
            string sqlQuery = string.Format(@"
select od.ProductName,od.Quantity,od.UnitPrice,TotalRowPrice=(od.Quantity*od.UnitPrice) 
,o.TotalAmount
,o.DiscountName,o.TotalDiscount
,o.NetTotal
,isnull(o.ReceivedAmount,0) as ReceivedAmount
,isnull(o.ReturnedAmount,0) as ReturnedAmount
,o.EntryDate,o.OrderID,o.Remarks
from tblOrderDetails od
inner join tblOrder o on o.OrderID=od.OrderID
where o.OrderID={0}
", OrderID);
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

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/Crystals/rptInvoice.rpt")));
           
            
            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
          //  rd.PrintToPrinter(1, false, 1, 1);
            return File(stream, "application/pdf", "InvoiceBill.pdf");

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

        [HttpPost]
        public ActionResult SearchInvoice (DateTime startdate , DateTime todate)
        {

            var result = db.tblOrder.Where(o => o.EntryDate >= startdate)
                                    .Where(o => o.EntryDate <= todate).ToList();

            return View(result);
           

        }





        public ActionResult SaveOrder(clsOrder od)
        {
            //
            string result = "Error! Order Is Not Complete!";
            if (!string.IsNullOrEmpty(od.CustomerMobileNo) && od.tblOrderDetails.Count > 0)
            {
                clsOrder o = new clsOrder();
                o.CustomerName = od.CustomerName;
                o.CustomerAddress = od.CustomerAddress;
                o.EntryDate = DateTime.Now;
                o.TotalAmount = od.tblOrderDetails.Sum(odst => odst.TotalAmount);
                o.NetTotal = o.TotalAmount - o.TotalDiscount;
                o.OrderCode = "ORD-" + (db.tblOrder.Count() + 1).ToString();
                db.tblOrder.Add(o);

                foreach (var item in od.tblOrderDetails)
                {
                    clsOrderDetails ods = new clsOrderDetails();
                    od.OrderID = o.OrderID;
                    ods.ProductName = item.ProductName;
                    ods.Quantity = item.Quantity;
                    ods.UnitPrice = item.UnitPrice;
                    ods.TotalAmount = item.TotalAmount;

                    db.tblOrderDetails.Add(ods);
                }
                db.SaveChanges();
                result = "Success! Order Is Complete!";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: zInvoice/Details/5
        public ActionResult DetailsReport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsOrder order = db.tblOrder.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        //report
        public ActionResult ReportOrder(int? id)
        {


            string shopcode = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsOrder order = db.tblOrder.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            //show
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/ReportCrystal/rptOrder.rpt")));
            rd.SetDataSource(
               (from od in db.tblOrderDetails
                join o in db.tblOrder on od.OrderID equals o.OrderID
                select new
                {
                    OrderCode = "INV-" + o.OrderID.ToString().PadLeft(4, '0'),
                    o.CustomerMobileNo,
                    o.CustomerName,
                    o.CustomerAddress,
                    o.EntryDate,
                    od.ProductName,
                    od.UnitPrice,
                    od.Quantity,
                    TotalPrice = od.UnitPrice * od.Quantity
                }).ToList()
                );
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "rptOrder.pdf");
        }



    }
}