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
    public class RestaurantDiscountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RestaurantDiscount
        public async Task<ActionResult> Index()
        {
            return View(await db.tblDiscount.ToListAsync());
        }

        // GET: RestaurantDiscount/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsDiscount clsDiscount = await db.tblDiscount.FindAsync(id);
            if (clsDiscount == null)
            {
                return HttpNotFound();
            }
            return View(clsDiscount);
        }

        // GET: RestaurantDiscount/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantDiscount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DiscountID,DiscountName,DiscountPercentage,Remarks")] clsDiscount clsDiscount)
        {
            if (ModelState.IsValid)
            {
                clsDiscount.Statuss = enumStatus.Active;
                clsDiscount.EntryDate = DateTime.Now;
                clsDiscount.EntryUserID = 999;

                db.tblDiscount.Add(clsDiscount);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clsDiscount);
        }

        // GET: RestaurantDiscount/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsDiscount clsDiscount = await db.tblDiscount.FindAsync(id);
            if (clsDiscount == null)
            {
                return HttpNotFound();
            }
            return View(clsDiscount);
        }

        // POST: RestaurantDiscount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DiscountID,DiscountName,DiscountPercentage,Remarks,Statuss,EntryUserID,EntryDate")] clsDiscount clsDiscount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clsDiscount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(clsDiscount);
        }

        // GET: RestaurantDiscount/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsDiscount clsDiscount = await db.tblDiscount.FindAsync(id);
            if (clsDiscount == null)
            {
                return HttpNotFound();
            }
            return View(clsDiscount);
        }

        // POST: RestaurantDiscount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            clsDiscount clsDiscount = await db.tblDiscount.FindAsync(id);
            db.tblDiscount.Remove(clsDiscount);
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
