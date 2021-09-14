using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using RestaurantManagement.Web.Extensions;
using RestaurantManagement.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class RestaurantTableWithOrderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        PrinterSettings settings = new PrinterSettings();

        // GET: RestaurantTableWithOrder
        [HttpGet]
        public ActionResult Index(string flag)
        {
            List<clsTableVM> tblvm = new List<clsTableVM>();
            List<clsTable> tableList = db.tblTable.ToList();
            foreach (var t in db.tblTable.ToList())
            {
                //  clsTableVM
                clsTableVM B = new clsTableVM();
                B.TableID = t.TableID;
                B.TableCode = t.TableCode;
                B.TableName = t.TableName;

                int orderCount = db.tblOrder.Where(o => o.TableID == t.TableID && o.OrderStatus == "Ordered").Count();
                int BillCount = db.tblOrder.Where(o => o.TableID == t.TableID && o.OrderStatus == "Billed").Count();
                if (orderCount > 0)
                {
                    B.TblBackColor = "#FFA500";
                    B.TblBooked = "Booked";
                }
                else if(BillCount > 0)
                {
                    B.TblBackColor = "#00D555";
                    B.TblBooked = "Billed";

                }
                else
                {
                    B.TblBackColor = "#00FCFE";
                    B.TblBooked = "Available";
                }

                tblvm.Add(B);
            }

            if (flag == "Order")
            {
                this.AddNotification("Order Saved", NotificationType.SUCCESS);
            }
            else if (flag == "Invoice")
            {
                this.AddNotification("Invoice Saved", NotificationType.SUCCESS);
            }
            else if (flag == "Billed")
            {

                this.AddNotification("Billed Saved", NotificationType.SUCCESS);
            }

            return View(tblvm.ToList());
        }


        public ActionResult AddOrder(int? tableid)
        {

         //   PrinterSettings settings = new PrinterSettings();
          //  ViewBag.Installedprinter = PrinterSettings.InstalledPrinters;
            ViewBag.printername = settings.PrinterName;

            int rowcount = db.tblOrder.Where(o => o.TableID == tableid && (o.OrderStatus == "Ordered" || o.OrderStatus == "Billed")).Select(o => o.OrderID).Count();

            var ExistingOrderID = db.tblOrder.Where(o => o.TableID == tableid &&( o.OrderStatus == "Ordered" || o.OrderStatus == "Billed")).Select(o => o.OrderID).FirstOrDefault();
            ViewBag.ExistingOrderID = ExistingOrderID;

            var ExistingEmployee  = db.tblOrder.Where(o => o.TableID == tableid && (o.OrderStatus == "Ordered" || o.OrderStatus == "Billed")).Select(o => o.OrderPostedBy ).FirstOrDefault();
            ViewBag.ExistingEmployee = ExistingEmployee;

           var ExistingDiscount = db.tblOrder.Where(o => o.TableID == tableid && (o.OrderStatus == "Ordered" || o.OrderStatus == "Billed")).Select(o => o.DiscountID).FirstOrDefault();
            ViewBag.ExistingDiscount = ExistingDiscount;

            int orderid = db.tblOrder.Where(o => o.TableID == tableid && (o.OrderStatus == "Ordered" || o.OrderStatus == "Billed")).Select(o => o.OrderID).FirstOrDefault();
            ViewBag.Ostatus = db.tblOrder.Where(o => o.OrderID == orderid).Select(o => o.OrderStatus).FirstOrDefault();
            ViewBag.Tname = db.tblTable.Where(t => t.TableID == tableid).Select(t => t.TableName).FirstOrDefault();
            ViewBag.TID = db.tblTable.Where(t => t.TableID == tableid).Select(t => t.TableID).FirstOrDefault();

            //ddl
            ViewBag.DiscountID = new SelectList(db.tblDiscount.Select(d => new { d.DiscountID, DiscountName = d.DiscountName + "_" + d.DiscountPercentage }), "DiscountID", "DiscountName", ExistingDiscount);
            ViewBag.TableID = new SelectList(db.tblTable.Where(t => t.TableID == tableid).Select(t => new { t.TableID, t.TableName }), "TableID", "TableName");
            ViewBag.EName = new SelectList(db.tblEmployee.Select(d => new { d.EName }), "EName", "EName", ExistingEmployee);


            ViewBag.OrderDetails = (
                from n in db.tblOrderDetails
                join m in db.tblOrder on n.OrderID equals m.OrderID
                where n.OrderID == orderid && m.TableID==tableid
                                    select n ).ToList();

            //from n in db.Products
            //join c in db.Categories on n.CategoryId equals c.CategoryId
            //orderby n.ProductId descending
            //select new { n.ProductName, n.ProductId, c.CategoryId, c.CategoryName };


            if (tableid == 1)
            {
                ViewBag.ShowHideOrderBtn = "none";
                ViewBag.ShowHideInvoiceBtn = "block";
                ViewBag.ShowHideBillBtn = "block";
                ViewBag.ShowHideKitchenCopy = "none";
                ViewBag.FormTitle = "Invoice Form";
            }
            else if (tableid > 1)
            {
                if (ExistingOrderID == 0)
                {
                    ViewBag.ShowHideOrderBtn = "block";
                    ViewBag.ShowHideInvoiceBtn = "none";
                    ViewBag.ShowHideBillBtn = "none";
                    ViewBag.ShowHideKitchenCopy = "none";
                    ViewBag.FormTitle = "Order Form";
                }
                else
                {
                    ViewBag.ShowHideOrderBtn = "block";
                    ViewBag.ShowHideInvoiceBtn = "block";
                    ViewBag.ShowHideBillBtn = "block";
                    ViewBag.ShowHideKitchenCopy = "block";
                    ViewBag.FormTitle = "Order / Invoice Form";
                }
            }
           

            ViewBag.Elist = db.tblEmployee.Where(e => e.Statuss == 0).ToList();
           ViewBag.Dlist = db.tblDiscount.Where(e => e.Statuss == 0).ToList();
            return View();

        }

        // to save order
        [HttpPost]
        public ActionResult SaveOrder(clsOrder o)
        {
            if (o.OrderID > 0)
            {
                db.tblOrderDetails.RemoveRange(db.tblOrderDetails.Where(c => c.OrderID == o.OrderID));
                db.tblOrder.RemoveRange(db.tblOrder.Where(pr => pr.OrderID == o.OrderID));
            }
            o.OrderStatus = "Ordered";

            //db.SaveChanges();
            if (o.tblOrderDetails != null)
            {
                db.tblOrder.Add(o);
                foreach (var od in o.tblOrderDetails)
                {
                    db.tblOrderDetails.Add(od);
                }
            }

            db.SaveChanges();
            // printing instant kitchen copy
           string sqlQuery = string.Format(@"select od.ProductName,od.Quantity
,isnull(o.TableName,'None') as TableName
,isnull(o.OrderPostedBy,'None') as OrderPostedBy
,o.EntryDate,o.OrderID
from tblOrderDetails od
inner join tblOrder o on o.OrderID=od.OrderID
where o.OrderID={0}
", o.OrderID);
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
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/Crystals/rptKitchenCopy.rpt")));


            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            rd.PrintOptions.PrinterName = settings.PrinterName;

            rd.PrintToPrinter(1, false, 1, 1);

          


            this.AddNotification("Order Saved", NotificationType.SUCCESS);
           // File(stream, "application/pdf", "KitchenCopy.pdf");
            return RedirectToAction("Index", "RestaurantTableWithOrder");
        }

// save invoice
        [HttpPost]
        public ActionResult SaveInvoice(clsOrder o)
        {
            if (o.TableID == 1)
            {
                //make invoice parcel
                o.OrderStatus = "Invoiced";
                o.OrderID = 0;
                o.TableName = db.tblTable.Where(t => t.TableID == 1).Select(t => t.TableName).FirstOrDefault(); ;
                o.TableCode = db.tblTable.Where(t => t.TableID == 1).Select(t => t.TableCode).FirstOrDefault(); ;
                o.DiscountName = db.tblDiscount.Where(d => d.DiscountID == o.DiscountID).Select(d => d.DiscountName).FirstOrDefault();


                if (o.tblOrderDetails != null)
                {
                    db.tblOrder.Add(o);
                    foreach (var od in o.tblOrderDetails)
                    {
                        db.tblOrderDetails.Add(od);
                    }
                }
                db.SaveChanges();
            }
            else if (o.TableID > 1)
            {
                //del old from details
                if (o.OrderID > 0)
                {
                    //remove old
                    db.tblOrderDetails.RemoveRange(db.tblOrderDetails.Where(c => c.OrderID == o.OrderID));
                    db.SaveChanges();
                }


                //make invoice from order
                var OrderObj = db.tblOrder.Where(or => or.OrderID == o.OrderID).FirstOrDefault();   // o;
                if (OrderObj != null)
                {
                    OrderObj.OrderStatus = "Invoiced";

                    OrderObj.TotalAmount = o.TotalAmount;
                    OrderObj.DiscountID = o.DiscountID;
                    OrderObj.DiscountName = db.tblDiscount.Where(d => d.DiscountID == o.DiscountID).Select(d => d.DiscountName).FirstOrDefault();
                    OrderObj.TotalDiscount = o.TotalDiscount;

                    OrderObj.TableID = o.TableID;
                    OrderObj.TableName = db.tblTable.Where(t => t.TableID == o.TableID).Select(t => t.TableName).FirstOrDefault();
                    OrderObj.TableCode = db.tblTable.Where(t => t.TableID == o.TableID).Select(t => t.TableCode).FirstOrDefault();

                    OrderObj.NetTotal = o.NetTotal;
                    OrderObj.ReceivedAmount = o.ReceivedAmount;
                    OrderObj.ReturnedAmount = o.ReturnedAmount;
                    OrderObj.TableName = o.TableName;
                    OrderObj.OrderPostedBy = o.OrderPostedBy;

                    OrderObj.Remarks = o.Remarks;


                    db.Entry(OrderObj).State = EntityState.Modified;


                    //add new
                    if (o.tblOrderDetails != null)
                    {
                        foreach (var od in o.tblOrderDetails)
                        {
                            od.OrderID = o.OrderID;
                            db.tblOrderDetails.Add(od);
                        }
                    }

                    db.SaveChanges();
                }

            }
            //  ReportInvoice(o.OrderID);

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
", o.OrderID);
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
            rd.PrintOptions.PrinterName = settings.PrinterName;
            rd.PrintToPrinter(1, false, 1, 1);
           // File(stream, "application/pdf", "InvoiceBillinstant.pdf");
            
            this.AddNotification("Invoiced Successfull", NotificationType.SUCCESS);
            return RedirectToAction("Index", "RestaurantTableWithOrder");
            //   RedirectToAction("Index", "RestaurantTableWithOrder");

        }

        // end save invoice


        // start Bill genaration
        [HttpPost]
        public ActionResult SaveBill(clsOrder o)
        {

           



            if (o.TableID == 1)
            {
                //make invoice parcel
                o.OrderStatus = "Billed";
                o.OrderID = 0;
                o.TableName = db.tblTable.Where(t => t.TableID == 1).Select(t => t.TableName).FirstOrDefault(); ;
                o.TableCode = db.tblTable.Where(t => t.TableID == 1).Select(t => t.TableCode).FirstOrDefault(); ;
                o.DiscountName = db.tblDiscount.Where(d => d.DiscountID == o.DiscountID).Select(d => d.DiscountName).FirstOrDefault();


                if (o.tblOrderDetails != null)
                {
                    db.tblOrder.Add(o);
                    foreach (var od in o.tblOrderDetails)
                    {
                        db.tblOrderDetails.Add(od);
                    }
                }
                db.SaveChanges();
            }
            else if (o.TableID > 1)
            {
                //del old from details
                if (o.OrderID > 0)
                {
                    //remove old
                    db.tblOrderDetails.RemoveRange(db.tblOrderDetails.Where(c => c.OrderID == o.OrderID));
                    db.SaveChanges();
                }


                //make invoice from order
                var OrderObj = db.tblOrder.Where(or => or.OrderID == o.OrderID).FirstOrDefault();   // o;
                if (OrderObj != null)
                {
                    OrderObj.OrderStatus = "Billed";

                    OrderObj.TotalAmount = o.TotalAmount;
                    OrderObj.DiscountID = o.DiscountID;
                    OrderObj.DiscountName = db.tblDiscount.Where(d => d.DiscountID == o.DiscountID).Select(d => d.DiscountName).FirstOrDefault();
                    OrderObj.TotalDiscount = o.TotalDiscount;

                    OrderObj.TableID = o.TableID;
                    OrderObj.TableName = db.tblTable.Where(t => t.TableID == o.TableID).Select(t => t.TableName).FirstOrDefault();
                    OrderObj.TableCode = db.tblTable.Where(t => t.TableID == o.TableID).Select(t => t.TableCode).FirstOrDefault();

                    OrderObj.NetTotal = o.NetTotal;
                    OrderObj.TableName = o.TableName;
                    OrderObj.OrderPostedBy = o.OrderPostedBy;

                    OrderObj.Remarks = o.Remarks;


                    db.Entry(OrderObj).State = EntityState.Modified;


                    //add new
                    if (o.tblOrderDetails != null)
                    {
                        foreach (var od in o.tblOrderDetails)
                        {
                            od.OrderID = o.OrderID;
                            db.tblOrderDetails.Add(od);
                        }
                    }

                    db.SaveChanges();
                }

            }
            //  ReportInvoice(o.OrderID);

            string sqlQuery = string.Format(@"select od.ProductName,od.Quantity,od.UnitPrice,TotalRowPrice=(od.Quantity*od.UnitPrice) 
,o.TotalAmount
,o.DiscountName,o.TotalDiscount
,o.NetTotal,o.OrderID
,o.EntryDate,o.Remarks
from tblOrderDetails od
inner join tblOrder o on o.OrderID=od.OrderID
where o.OrderID={0}
", o.OrderID);
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
            PrinterSettings settings = new PrinterSettings();
       
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/Crystals/rptBill.rpt")));


            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);





            //rd.ExportToDisk(ExportFormatType.PortableDocFormat, Server.MapPath("Files/Billinstant.pdf"));
            // rd.Load(@"d:\Reports\Report1.rpt");

            //ClientScript.RegisterStartupScript(this.Page.GetType(), "popupOpener", "var popup=window.open('Files/BiWeeklyReport.pdf');popup.focus();", true);
            //  ScriptManager.RegisterStartupScript(this.GetType(), "ShowStatus", "var popup=window.open('Files/BiWeeklyReport.pdf');popup.focus();", true);

            // billed method  
            //  var pname = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
            // var dpn = settings.PrinterName;
            // rd.PrintOptions.PrinterName = dpn;
         //   PrinterSettings settings = new PrinterSettings();
            rd.PrintOptions.PrinterName = settings.PrinterName;


            rd.PrintToPrinter(1, false, 1, 1);


            this.AddNotification("Billed Successfull", NotificationType.SUCCESS);


          
           // return File(stream, "application/pdf", "Billinstant-" + o.OrderID + ".pdf");
            return RedirectToAction("Index", "RestaurantTableWithOrder");

        }









        // end bill genaration

        // Print kitchen Copy

        public ActionResult PrintKitchenCopy(int OrderID)
        {

            // RestaurantReports  ReportInvoice
            string sqlQuery = string.Format(@"select od.ProductName,od.Quantity
,isnull(o.TableName,'None') as TableName
,isnull(o.OrderPostedBy,'None') as OrderPostedBy
,o.EntryDate,o.OrderID
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
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/Crystals/rptKitchenCopy.rpt")));


            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);


            /*  System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();

              CrystalDecisions.Shared.PrintLayoutSettings PrintLayout = new CrystalDecisions.Shared.PrintLayoutSettings();

              System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
              var dpn = settings.PrinterName;
              printerSettings.PrinterName = dpn;

              System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
              */
            // CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions popt = new CrystalDecisions.ReportAppServer.Controllers.PrintReportOptions();
            //    popt.PrinterName = printerSettings.PrinterName;
            // rd.ReportClientDocument.PrintOutputController.PrintReport(popt);


            //   ViewBag.Installedprinter = PrinterSettings.InstalledPrinters;
            // ViewBag.printername = settings.PrinterName;
            // rd.PrintToPrinter(printerSettings, pSettings, false, PrintLayout);
          
            rd.PrintOptions.PrinterName = settings.PrinterName;
            
            rd.PrintToPrinter(1, true, 1, 1);

            return File(stream, "application/pdf", "KitchenCopy.pdf");

        }

        public ActionResult PrintBillCopy(int OrderID)
        {

            // RestaurantReports  ReportInvoice
            string sqlQuery = string.Format(@"select od.ProductName,od.Quantity,od.UnitPrice,TotalRowPrice=(od.Quantity*od.UnitPrice) 
,o.TotalAmount
,o.DiscountName,o.TotalDiscount
,o.NetTotal,o.OrderID
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
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/Crystals/rptBill.rpt")));


            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            // rd.PrintToPrinter(1, false, 1, 1);
            return File(stream, "application/pdf", "Billinstant-" + OrderID + ".pdf");

        }

        public ActionResult PrintInvoiceCopy(int OrderID)
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
            // rd.PrintToPrinter(1, false, 1, 1);
           return File(stream, "application/pdf", "Invoiceinstant-" + OrderID + ".pdf");

        }














        // invoice print and download
        public void ReportInvoice(int OrderID)
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
            rd.PrintToPrinter(1, false, 1, 1);
            File(stream, "application/pdf", "InvoiceBillinstant.pdf");

        }



        //
        public ActionResult CrReport(int OrderID)
        {








            return View();
        }






        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var customers = (from i in db.tblItem
                                 where (i.ItemCode + ":" + i.ItemName).Contains(prefix)
                                 select new
                                 {
                                     label = i.ItemCode + ":" + i.ItemName,
                                     valID = i.ItemID,
                                     valItemName = i.ItemName,
                                     valUnitPrice = i.UnitPrice
                                 }).ToList();

                return Json(customers);
            }

        }
    }
}