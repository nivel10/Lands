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
    public class PredictionsController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        // GET: Predictions
        public async Task<ActionResult> Index()
        {
            var predictions = db.Predictions
                .Include(p => p.Board)
                .Include(p => p.Match)
                .Include(p => p.User);
            return View(await predictions.ToListAsync());
        }

        // GET: Predictions/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var prediction = await db.Predictions.FindAsync(id);
            if (prediction == null)
            {
                return HttpNotFound();
            }

            return View(prediction);
        }

        // GET: Predictions/Create
        public ActionResult Create()
        {
            var prediction = new Prediction { };
            
            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(prediction);

            return View(prediction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Prediction prediction)
        {
            if (ModelState.IsValid)
            {
                db.Predictions.Add(prediction);
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(prediction);

            return View(prediction);
        }

        // GET: Predictions/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var prediction = await db.Predictions.FindAsync(id);
            if (prediction == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(prediction);

            return View(prediction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Prediction prediction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prediction).State = EntityState.Modified;
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(prediction);

            return View(prediction);
        }

        // GET: Predictions/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var prediction = await db.Predictions.FindAsync(id);
            if (prediction == null)
            {
                return HttpNotFound();
            }
            return View(prediction);
        }

        // POST: Predictions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var prediction = await db.Predictions.FindAsync(id);
            db.Predictions.Remove(prediction);
            response = await DbHelper.SaveChangeDB(db);
            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBag(prediction);

            return View(prediction);
        }

        private void LoadViewBag(Prediction prediction)
        {
            ViewBag.BoardId =
                new SelectList(
                    CombosHelper.GetBorads(db),
                    "BoardId",
                    "ImagePath",
                    prediction == null ?
                        0 :
                            prediction.BoardId);

            ViewBag.MatchId =
                new SelectList(
                    CombosHelper.GetMatchs(db),
                    "MatchId",
                    "MatchId",
                    prediction == null ?
                        0 :
                            prediction.MatchId);

            ViewBag.UserId =
                new SelectList(
                    CombosHelper.GetUsers(db),
                    "UserId",
                    "FirstName",
                    prediction == null ?
                        0 :
                            prediction.UserId);
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