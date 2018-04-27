namespace Lands.BackEnd.Controllers
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using System.Net;
    using System.Web.Mvc;
    using Lands.BackEnd.Models;
    using Lands.Domain.GetServices;
    using Lands.Domain.Others;
    using System.Linq;
    using Lands.BackEnd.Helpers;

    [Authorize(Roles = "Admin")]
    public class CantvDatasController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        #region Methods View

        // GET: CantvDatas
        public async Task<ActionResult> Index()
        {
            var appName = MethodsHelper.GetAppNameGetServices();
            var cantvDatas = await db.CantvDatas
                .Include(c => c.User)
                .OrderBy(c => c.UserId)
                .ThenBy(c => c.CodePhone)
                .ThenBy(c => c.NumberPhone)
                .Where(c => c.User.AppName == appName)
                .ToListAsync();

            //  return View(await cantvDatas.ToListAsync());
            return View(cantvDatas);
        }

        // GET: CantvDatas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cantvData = await db.CantvDatas.FindAsync(id);
            if (cantvData == null)
            {
                return HttpNotFound();
            }
            return View(cantvData);
        }

        // GET: CantvDatas/Create
        public ActionResult Create()
        {
            var cantvData = new CantvData { };

            //  CHEJ - Load data in ViewBag
            LoadViewBag(cantvData);

            return View(cantvData);
        }

        // POST: CantvDatas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CantvData cantvData)
        {
            if (ModelState.IsValid)
            {
                db.CantvDatas.Add(cantvData);
                response = await DbHelper.SaveChangeDB(db);
                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return View(cantvData);
                }
                return RedirectToAction("Index");
            }

            //  CHEJ - Load data in ViewBag
            LoadViewBag(cantvData);

            return View(cantvData);
        }

        // GET: CantvDatas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cantvData = await db.CantvDatas.FindAsync(id);
            if (cantvData == null)
            {
                return HttpNotFound();
            }
            //  CHEJ - Load data in ViewBag
            LoadViewBag(cantvData);

            return View(cantvData);
        }

        // POST: CantvDatas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CantvData cantvData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cantvData).State = EntityState.Modified;

                response = await DbHelper.SaveChangeDB(db);
                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return View(cantvData);
                }
                return RedirectToAction("Index");
            }

            //  CHEJ - Load data in ViewBag
            LoadViewBag(cantvData);
            return View(cantvData);
        }

        // GET: CantvDatas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cantvData = await db.CantvDatas.FindAsync(id);
            if (cantvData == null)
            {
                return HttpNotFound();
            }
            return View(cantvData);
        }

        // POST: CantvDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var cantvData = await db.CantvDatas.FindAsync(id);
            db.CantvDatas.Remove(cantvData);
            response = await DbHelper.SaveChangeDB(db);
            if (!response.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(cantvData);
            }

            return RedirectToAction("Index");
        } 

        #endregion Methods View

        #region Methods

        private void LoadViewBag(CantvData cantvData)
        {
            ViewBag.UserId =
                new SelectList(CombosHelper.GetUsersGetServicesVzLa(db), "UserId", "FullName", cantvData.UserId);
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