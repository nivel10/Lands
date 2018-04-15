namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Models;
    using Lands.Domain.Soccer;
    using System.Data.Entity;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class StatusMatchesController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        // GET: StatusMatches
        public async Task<ActionResult> Index()
        {
            return View(await db.StatusMatches.ToListAsync());
        }

        // GET: StatusMatches/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var statusMatch = await db.StatusMatches.FindAsync(id);
            if (statusMatch == null)
            {
                return HttpNotFound();
            }
            return View(statusMatch);
        }

        // GET: StatusMatches/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StatusMatch statusMatch)
        {
            if (ModelState.IsValid)
            {
                db.StatusMatches.Add(statusMatch);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(statusMatch);
        }

        // GET: StatusMatches/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var statusMatch = await db.StatusMatches.FindAsync(id);
            if (statusMatch == null)
            {
                return HttpNotFound();
            }
            return View(statusMatch);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StatusMatch statusMatch)
        {
            if (ModelState.IsValid)
            {
                db.Entry(statusMatch).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(statusMatch);
        }

        // GET: StatusMatches/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var statusMatch = await db.StatusMatches.FindAsync(id);
            if (statusMatch == null)
            {
                return HttpNotFound();
            }
            return View(statusMatch);
        }

        // POST: StatusMatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var statusMatch = await db.StatusMatches.FindAsync(id);
            db.StatusMatches.Remove(statusMatch);
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