namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Helpers;
    using Lands.BackEnd.Models;
    using Lands.Domain.Others;
    using Lands.Domain.Soccer;
    using System.Data.Entity;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class BoardsController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        // GET: Boards
        public async Task<ActionResult> Index()
        {
            var boards = db.Boards.Include(b => b.BoardStatus).Include(b => b.User);
            return View(await boards.ToListAsync());
        }

        // GET: Boards/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }
            return View(board);
        }

        // GET: Boards/Create
        public ActionResult Create()
        {
            var board = new Board { };

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(board);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Board board)
        {
            if (ModelState.IsValid)
            {
                db.Boards.Add(board);
                response = await DbHelper.SaveChangeDB(db);

                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(board);

            return View(board);
        }

        // GET: Boards/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(board);

            return View(board);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Board board)
        {
            if (ModelState.IsValid)
            {
                db.Entry(board).State = EntityState.Modified;
                response = await DbHelper.SaveChangeDB(db);

                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(board);

            return View(board);
        }

        // GET: Boards/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var board = await db.Boards.FindAsync(id);
            if (board == null)
            {
                return HttpNotFound();
            }

            return View(board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var board = await db.Boards.FindAsync(id);
            db.Boards.Remove(board);
            response = await DbHelper.SaveChangeDB(db);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);

            return View(board);
        }

        private void LoadViewBag(Board board)
        {
            ViewBag.BoardStatusId =
                new SelectList(
                    CombosHelper.GetBoradStatus(db),
                    "BoardStatusId",
                    "Name",
                    board == null ?
                        0 :
                            board.BoardStatusId);

            ViewBag.UserId =
                new SelectList(
                    CombosHelper.GetUsers(db),
                    "UserId",
                    "FirstName",
                    board == null ?
                        0 :
                            board.UserId);
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