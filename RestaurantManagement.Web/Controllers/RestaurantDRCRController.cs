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
    public class RestaurantDRCRController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RestaurantDRCR
        public async Task<ActionResult> Index(string startdate, string todate)
        {
            if (!string.IsNullOrEmpty(startdate) && !string.IsNullOrEmpty(todate))
            {
                var sd = startdate + " 12:00:00 AM";
                DateTime sd1 = Convert.ToDateTime(sd);

                var td = todate + " 11:59:59 PM";

                var td1 = Convert.ToDateTime(td);
                var result = await db.tblDRCR.Where(e => e.EntryDate >= sd1 && e.EntryDate <= td1 && e.Statuss == enumStatus.Active).ToListAsync();

                ViewBag.total = db.tblDRCR.Where(e => e.EntryDate >= sd1 && e.EntryDate <= td1 && e.Statuss == enumStatus.Active).Sum(x => (double?)x.Debit) ?? 0;
                return View(result);

            }
            else
            {

                ViewBag.total = db.tblDRCR.Sum(x => (double?)x.Debit) ?? 0;
                return View(await db.tblDRCR.Where(e => e.Statuss == enumStatus.Active).ToListAsync());

            }



        }

        // GET: RestaurantDRCR/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsDRCR clsDRCR = await db.tblDRCR.FindAsync(id);
            if (clsDRCR == null)
            {
                return HttpNotFound();
            }
            return View(clsDRCR);
        }

        // GET: RestaurantDRCR/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RestaurantDRCR/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DRCRID,ExpenseFor,Debit,Remarks")] clsDRCR clsDRCR)
        {
            if (ModelState.IsValid)
            {
                //,Credit,Balance,Statuss,EntryUserID,EntryDate
                clsDRCR.Credit = 0;
                clsDRCR.Balance = 0;
                clsDRCR.Statuss = enumStatus.Active;
                clsDRCR.EntryDate = DateTime.Now;
                clsDRCR.EntryUserID = 999;


                db.tblDRCR.Add(clsDRCR);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(clsDRCR);
        }

        // GET: RestaurantDRCR/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsDRCR clsDRCR = await db.tblDRCR.FindAsync(id);
            if (clsDRCR == null)
            {
                return HttpNotFound();
            }
            return View(clsDRCR);
        }

        // POST: RestaurantDRCR/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DRCRID,ExpenseFor,Debit,Credit,Balance,Remarks,Statuss,EntryUserID,EntryDate")] clsDRCR clsDRCR)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clsDRCR).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(clsDRCR);
        }

        // GET: RestaurantDRCR/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            clsDRCR clsDRCR = await db.tblDRCR.FindAsync(id);
            if (clsDRCR == null)
            {
                return HttpNotFound();
            }
            return View(clsDRCR);
        }

        // POST: RestaurantDRCR/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            clsDRCR clsDRCR = await db.tblDRCR.FindAsync(id);
            db.tblDRCR.Remove(clsDRCR);
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
