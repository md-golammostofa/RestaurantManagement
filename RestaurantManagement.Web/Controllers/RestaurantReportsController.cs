using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class RestaurantReportsController : Controller
    {
        // GET: RestaurantReports
        public ActionResult Index()
        {
            return View();
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
            rd.Load(Path.Combine(Server.MapPath("~/Reporting/ReportCrystal/rptStudentList.rpt")));
            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "StudentList.pdf");

        }

    }
}