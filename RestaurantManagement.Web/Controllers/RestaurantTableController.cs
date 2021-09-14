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
using RestaurantManagement.Web.Extensions;

namespace RestaurantManagement.Web.Controllers
{
    [Authorize]
    public class RestaurantTableController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RestaurantTable
        public async Task<ActionResult> Index()
        {
            return View(await db.tblTable.ToListAsync());
        }

        // GET: RestaurantTable/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsTable clsTable = await db.tblTable.FindAsync(id);
            if (clsTable == null)
            {
                return HttpNotFound();
            }
            return View(clsTable);
        }

        // GET: RestaurantTable/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantTable/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TableID,TableName,TableCode,Remarks,Statuss")] clsTable clsTable)
        {
            if (ModelState.IsValid)
            {   
               
                 clsTable.EntryDate = DateTime.Now;
                clsTable.EntryUserID =999;

                db.tblTable.Add(clsTable);
                await db.SaveChangesAsync();

                this.AddNotification("Table Added", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(clsTable);
        }

        // GET: RestaurantTable/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsTable clsTable = await db.tblTable.FindAsync(id);
            if (clsTable == null)
            {
                return HttpNotFound();
            }
            return View(clsTable);
        }

        // POST: RestaurantTable/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "TableID,TableName,TableCode,Remarks,Statuss,EntryUserID,EntryDate")] clsTable clsTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clsTable).State = EntityState.Modified;
                await db.SaveChangesAsync();

                this.AddNotification("Table Info Updated", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(clsTable);
        }

        // GET: RestaurantTable/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsTable clsTable = await db.tblTable.FindAsync(id);
            if (clsTable == null)
            {
                return HttpNotFound();
            }
            return View(clsTable);
        }

        // POST: RestaurantTable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            clsTable clsTable = await db.tblTable.FindAsync(id);
            db.tblTable.Remove(clsTable);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult IsAlreadyExisted(string TableCode)
        {
            var ExistingTableCode =
                db.tblTable
                .Where(t => t.TableCode.Trim().ToUpper() == TableCode.Trim().ToUpper())
                .Select(t=>t.TableCode).FirstOrDefault();

            bool status = true;
            if (ExistingTableCode != null)
            { 
                status = false;
            }
            return Json(status);
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
