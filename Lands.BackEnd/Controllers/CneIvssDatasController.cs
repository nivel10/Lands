namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Helpers;
    using Lands.BackEnd.Models;
    using Lands.Domain.GetServices;
    using Lands.Domain.Others;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class CneIvssDatasController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        #region Methods View

        // GET: CneIvssDatas
        public async Task<ActionResult> Index()
        {
            var cneDatas = await db.CneIvssDatas
                .Include(c => c.Nationality)
                .Include(c => c.User)
                .OrderBy(c => c.UserId)
                .ToListAsync();

            return View(cneDatas);
        }

        // GET: CneIvssDatas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cneIvssData = await db.CneIvssDatas.FindAsync(id);
            if (cneIvssData == null)
            {
                return HttpNotFound();
            }
            return View(cneIvssData);
        }

        // GET: CneIvssDatas/Create
        public ActionResult Create()
        {
            var cneIvssData = 
                new CneIvssData
                {
                    BirthDate = DateTime.Now.Date,
                };

            //   CHEJ - Load ViewBag
            LoadViewBag(cneIvssData);

            return View(cneIvssData);
        }

        // POST: CneIvssDatas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CneIvssData cneIvssData)
        {
            if (ModelState.IsValid)
            {
                db.CneIvssDatas.Add(cneIvssData);
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  Load ViewBag
            LoadViewBag(cneIvssData);

            return View(cneIvssData);
        }

        // GET: CneIvssDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cneIvssData = await db.CneIvssDatas.FindAsync(id);
            if (cneIvssData == null)
            {
                return HttpNotFound();
            }

            //  Load ViewBag
            LoadViewBag(cneIvssData);

            return View(cneIvssData);
        }

        // POST: CneIvssDatas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CneIvssData cneIvssData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cneIvssData).State = EntityState.Modified;
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  Load ViewBag
            LoadViewBag(cneIvssData);

            return View(cneIvssData);
        }

        // GET: CneIvssDatas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cneIvssData = await db.CneIvssDatas.FindAsync(id);
            if (cneIvssData == null)
            {
                return HttpNotFound();
            }
            return View(cneIvssData);
        }

        // POST: CneIvssDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var cneIvssData = await db.CneIvssDatas.FindAsync(id);
            db.CneIvssDatas.Remove(cneIvssData);
            response = await DbHelper.SaveChangeDB(db);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);

            //  Load ViewBag
            LoadViewBag(cneIvssData);

            return View(cneIvssData);
        }

        #endregion Methods View

        #region Methods

        private void LoadViewBag(CneIvssData cneIvss)
        {
            ViewBag.NationalityId =
                new SelectList(CombosHelper.GetNationalities(db), "NationalityId", "FullNationality");
            ViewBag.UserId =
                new SelectList(CombosHelper.GetUsersGetServicesVzLa(db), "UserId", "FullName", cneIvss.UserId);
        }

        protected override void Dispose(bool disposing)

        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        } 

        #endregion Methods
    }
}