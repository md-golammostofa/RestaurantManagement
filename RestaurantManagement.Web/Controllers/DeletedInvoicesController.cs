using CrystalDecisions.CrystalReports.Engine;
using RestaurantManagement.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagement.Web.Controllers
{
    public class DeletedInvoicesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: DeletedInvoices
        public ActionResult Index(string startdate, string todate, string iid, string printdownload)
        {
            
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
where o.OrderStatus='Invoiced' and o.Statuss='1' and CONVERT(date,o.EntryDate) BETWEEN CONVERT(date,'{0}') and CONVERT(date,'{1}')", sd1.ToString(), td1.ToString());
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
                var result = db.tblOrder.Where(o => o.EntryDate >= sd1 && o.EntryDate <= td1 && o.Statuss != 0).ToList();
                // var result = db.Database.SqlQuery<clsOrderDetails>("spGetInvoiceList @datestart = {0}, @dateend= {1}",sd, td).ToList();
                ViewBag.todaysdate = startdate;
                ViewBag.todayedate = todate;

                return View(result);
            }

            else if (!string.IsNullOrEmpty(iid))
            {
                if (int.TryParse(iid, out int invoice)) { }
                var result = db.tblOrder.Where(o => o.OrderID == invoice && o.Statuss != 0).ToList();

                return View(result);
            }
            else
            {


                var result = db.tblOrder.OrderByDescending(o => o.EntryDate).Where(o => o.Statuss != 0).ToList();
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
where o.OrderStatus='Invoiced' and CONVERT(date,o.EntryDate) BETWEEN CONVERT(date,'{0}') and CONVERT(date,'{1}')", startdate, todate);
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


        public ActionResult ReportInvoice(int OrderID)
        {

            // RestaurantReports  ReportInvoice
            string sqlQuery = string.Format(@"
select od.ProductName,od.Quantity,od.UnitPrice,TotalRowPrice=(od.Quantity*od.UnitPrice) 
,o.TotalAmount
,o.DiscountName,o.TotalDiscount
,o.NetTotal
,o.EntryDate,o.Remarks
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
        public ActionResult SearchInvoice(DateTime startdate, DateTime todate)
        {

            var result = db.tblOrder.Where(o => o.EntryDate >= startdate)
                                    .Where(o => o.EntryDate <= todate).ToList();

            return View(result);


        }






    }
}