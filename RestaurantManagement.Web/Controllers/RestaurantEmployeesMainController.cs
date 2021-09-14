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
    public class RestaurantEmployeesMainController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RestaurantEmployeesMain
        public async Task<ActionResult> Index()
        {
            return View(await db.tblEmployee.ToListAsync());
        }

        // GET: RestaurantEmployeesMain/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsEmployee clsEmployee = await db.tblEmployee.FindAsync(id);
            if (clsEmployee == null)
            {
                return HttpNotFound();
            }
            return View(clsEmployee);
        }

        // GET: RestaurantEmployeesMain/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantEmployeesMain/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EID,EName,EType,ENumber,Remarks,Statuss,EntryUserID,EntryDate")] clsEmployee clsEmployee)
        {
            if (ModelState.IsValid)
            {
                db.tblEmployee.Add(clsEmployee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clsEmployee);
        }

        // GET: RestaurantEmployeesMain/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsEmployee clsEmployee = await db.tblEmployee.FindAsync(id);
            if (clsEmployee == null)
            {
                return HttpNotFound();
            }
            return View(clsEmployee);
        }

        // POST: RestaurantEmployeesMain/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EID,EName,EType,ENumber,Remarks,Statuss,EntryUserID,EntryDate")] clsEmployee clsEmployee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clsEmployee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(clsEmployee);
        }

        // GET: RestaurantEmployeesMain/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsEmployee clsEmployee = await db.tblEmployee.FindAsync(id);
            if (clsEmployee == null)
            {
                return HttpNotFound();
            }
            return View(clsEmployee);
        }

        // POST: RestaurantEmployeesMain/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            clsEmployee clsEmployee = await db.tblEmployee.FindAsync(id);
            db.tblEmployee.Remove(clsEmployee);
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
