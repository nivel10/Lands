namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Helpers;
    using Lands.BackEnd.Models;
    using Lands.Domain.GetServices;
    using Lands.Domain.Others;
    using System.Data.Entity;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class NationalitiesController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response respose = new Response();

        #region Methos View

        // GET: Nationalities
        public async Task<ActionResult> Index()
        {
            return View(await db.Nationalities.ToListAsync());
        }

        // GET: Nationalities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nationality = await db.Nationalities.FindAsync(id);
            if (nationality == null)
            {
                return HttpNotFound();
            }
            return View(nationality);
        }

        // GET: Nationalities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Nationalities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Nationality nationality)
        {
            if (ModelState.IsValid)
            {
                db.Nationalities.Add(nationality);
                respose = await DbHelper.SaveChangeDB(db);
                if (respose.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respose.Message);
            }

            return View(nationality);
        }

        // GET: Nationalities/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nationality = await db.Nationalities.FindAsync(id);
            if (nationality == null)
            {
                return HttpNotFound();
            }
            return View(nationality);
        }

        // POST: Nationalities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Nationality nationality)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nationality).State = EntityState.Modified;
                respose = await DbHelper.SaveChangeDB(db);
                if (respose.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, respose.Message);
            }
            return View(nationality);
        }

        // GET: Nationalities/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nationality = await db.Nationalities.FindAsync(id);
            if (nationality == null)
            {
                return HttpNotFound();
            }
            return View(nationality);
        }

        // POST: Nationalities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var nationality = await db.Nationalities.FindAsync(id);
            db.Nationalities.Remove(nationality);
            respose = await DbHelper.SaveChangeDB(db);
            if (respose.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, respose.Message);
            return View(nationality);
        }

        #endregion Methods View

        #region Methods

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