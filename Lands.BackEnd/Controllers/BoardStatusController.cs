namespace Lands.BackEnd.Controllers
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web.Mvc;
    using Lands.BackEnd.Models;
    using Lands.Domain.Soccer;

    [Authorize(Roles = "Admin")]
    public class BoardStatusController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

        // GET: BoardStatus
        public async Task<ActionResult> Index()
        {
            return View(await db.BoardStatus.ToListAsync());
        }

        // GET: BoardStatus/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var boardStatus = await db.BoardStatus.FindAsync(id);
            if (boardStatus == null)
            {
                return HttpNotFound();
            }
            return View(boardStatus);
        }

        // GET: BoardStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BoardStatus boardStatus)
        {
            if (ModelState.IsValid)
            {
                db.BoardStatus.Add(boardStatus);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(boardStatus);
        }

        // GET: BoardStatus/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var boardStatus = await db.BoardStatus.FindAsync(id);
            if (boardStatus == null)
            {
                return HttpNotFound();
            }
            return View(boardStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BoardStatus boardStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(boardStatus).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(boardStatus);
        }

        // GET: BoardStatus/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var boardStatus = await db.BoardStatus.FindAsync(id);
            if (boardStatus == null)
            {
                return HttpNotFound();
            }
            return View(boardStatus);
        }

        // POST: BoardStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var boardStatus = await db.BoardStatus.FindAsync(id);
            db.BoardStatus.Remove(boardStatus);
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