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
    public class RestaurantItemController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RestaurantItem
        public async Task<ActionResult> Index()
        {
            return View(await db.tblItem.ToListAsync());
        }

        // GET: RestaurantItem/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsItem clsItem = await db.tblItem.FindAsync(id);
            if (clsItem == null)
            {
                return HttpNotFound();
            }
            return View(clsItem);
        }

        // GET: RestaurantItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ItemID,ItemName,ItemCode,Description,UnitPrice")] clsItem clsItem)
        {
            if (ModelState.IsValid)
            {
                //,Statuss,EntryUserID,EntryDate
                clsItem.Statuss = enumStatus.Active;
                clsItem.EntryDate = DateTime.Now;
                clsItem.EntryUserID = 999;
                clsItem.Remarks = "menu items";

                db.tblItem.Add(clsItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clsItem);
        }

        // GET: RestaurantItem/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsItem clsItem = await db.tblItem.FindAsync(id);
            if (clsItem == null)
            {
                return HttpNotFound();
            }
            return View(clsItem);
        }

        // POST: RestaurantItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ItemID,ItemName,ItemCode,Description,UnitPrice,Remarks,Statuss,EntryUserID,EntryDate")] clsItem clsItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clsItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(clsItem);
        }

        // GET: RestaurantItem/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsItem clsItem = await db.tblItem.FindAsync(id);
            if (clsItem == null)
            {
                return HttpNotFound();
            }
            return View(clsItem);
        }

        // POST: RestaurantItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            clsItem clsItem = await db.tblItem.FindAsync(id);
            db.tblItem.Remove(clsItem);
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
