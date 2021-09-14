using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RestaurantManagement.Web.Models;

namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class RestaurantOrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RestaurantOrders
        public async Task<ActionResult> Index()
        {
            return View(await db.tblOrder.ToListAsync());
        }

        // GET: RestaurantOrders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsOrder clsOrder = await db.tblOrder.FindAsync(id);
            if (clsOrder == null)
            {
                return HttpNotFound();
            }
            return View(clsOrder);
        }

        // GET: RestaurantOrders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderID,OrderCode,CustomerID,CustomerMobileNo,CustomerName,CustomerAddress,TableID,TableName,TableCode,DiscountID,DiscountName,TotalDiscount,TotalAmount,NetTotal,Remarks,Statuss,EntryUserID,EntryDate")] clsOrder clsOrder)
        {
            if (ModelState.IsValid)
            {
                db.tblOrder.Add(clsOrder);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clsOrder);
        }

        // GET: RestaurantOrders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsOrder clsOrder = await db.tblOrder.FindAsync(id);
            if (clsOrder == null)
            {
                return HttpNotFound();
            }
            return View(clsOrder);
        }

        // POST: RestaurantOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderID,OrderCode,CustomerID,CustomerMobileNo,CustomerName,CustomerAddress,TableID,TableName,TableCode,DiscountID,DiscountName,TotalDiscount,TotalAmount,NetTotal,Remarks,Statuss,EntryUserID,EntryDate")] clsOrder clsOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clsOrder).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(clsOrder);
        }

        // GET: RestaurantOrders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsOrder clsOrder = await db.tblOrder.FindAsync(id);
            if (clsOrder == null)
            {
                return HttpNotFound();
            }
            return View(clsOrder);
        }

        // POST: RestaurantOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            clsOrder clsOrder = await db.tblOrder.FindAsync(id);
            db.tblOrder.Remove(clsOrder);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
