namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Helpers;
    using Lands.BackEnd.Models;
    using Lands.Domain.GetServices;
    using Lands.Domain.Others;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class ZoomDatasController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        #region Methods View

        // GET: ZoomDatas
        public async Task<ActionResult> Index()
        {
            var zoomDatas = await db.ZoomDatas
                .Include(z => z.User)
                .OrderBy(z => z.UserId)
                .ToListAsync();

            return View(zoomDatas);
        }

        // GET: ZoomDatas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var zoomData = await db.ZoomDatas.FindAsync(id);
            if (zoomData == null)
            {
                return HttpNotFound();
            }
            return View(zoomData);
        }

        // GET: ZoomDatas/Create
        public ActionResult Create()
        {
            var zoomData = new ZoomData { };

            //  CHEJ - Load ViewBag
            LoadViewBag(zoomData);

            return View();
        }

        // POST: ZoomDatas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ZoomData zoomData)
        {
            if (ModelState.IsValid)
            {
                db.ZoomDatas.Add(zoomData);
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  CHEJ - Load ViewBag
            LoadViewBag(zoomData);

            return View(zoomData);
        }

        // GET: ZoomDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var zoomData = await db.ZoomDatas.FindAsync(id);
            if (zoomData == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Load ViewBag
            LoadViewBag(zoomData);

            return View(zoomData);
        }

        // POST: ZoomDatas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ZoomData zoomData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zoomData).State = EntityState.Modified;
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError(string.Empty, response.Message);

            //  CHEJ - Load ViewBag
            LoadViewBag(zoomData);
            return View(zoomData);
        }

        // GET: ZoomDatas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var zoomData = await db.ZoomDatas.FindAsync(id);
            if (zoomData == null)
            {
                return HttpNotFound();
            }
            return View(zoomData);
        }

        // POST: ZoomDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var zoomData = await db.ZoomDatas.FindAsync(id);
            db.ZoomDatas.Remove(zoomData);
            response = await DbHelper.SaveChangeDB(db);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, response.Message);

            //  CHEJ - Load ViewBag
            LoadViewBag(zoomData);
            return View(zoomData);
        } 

        #endregion Methods View

        #region Methods

        private void LoadViewBag(ZoomData zoomData)
        {
            ViewBag.UserId =
                new SelectList(CombosHelper.GetUsersGetServicesVzLa(db), "UserId", "FullName", zoomData.UserId);
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