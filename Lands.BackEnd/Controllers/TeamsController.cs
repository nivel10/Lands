namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Helpers;
    using Lands.BackEnd.Models;
    using Lands.Domain.Others;
    using Lands.Domain.Soccer;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class TeamsController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        #region Methods View

        // GET: Teams
        public async Task<ActionResult> Index()
        {
            var teams = await db.Teams
                .OrderBy(t => t.Name)
                .ToListAsync();

            //  return View(await db.Teams.ToListAsync());
            return View(teams);
        }

        // GET: Teams/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //  public async Task<ActionResult> Create(Team team)
        public async Task<ActionResult> Create(TeamView view)
        {
            if (ModelState.IsValid)
            {
                //if (view.ImageFile != null)
                //{
                //    MethodsHelper.Image = view.ImagePath;

                //    //  CEHJ - Guarda la imagen en el FTP
                //    MethodsHelper.Image =
                //        FilesHelper.UploadPhoto(
                //            view.ImageFile,
                //            MethodsHelper.GetFolderSoccerFlag());

                //    //  CEHJ - Da formato a la imagen
                //    MethodsHelper.Image =
                //        string.Format(
                //            "{0}{1}",
                //            MethodsHelper.GetFolderSoccerFlag(),
                //            MethodsHelper.Image);
                //}

                //  CHEJ - Transforma el TeamView a Team
                var team = this.TeamViewToTeam(view);
                //  CHEJ - Actualiza el campo ImagePath
                //  team.ImagePath = MethodsHelper.Image;

                db.Teams.Add(team);
                response = await DbHelper.SaveChangeDB(db);

                if (response.IsSuccess)
                {
                    if (view.ImageFile != null)
                    {
                        MethodsHelper.Image = view.ImagePath;

                        //  CEHJ - Guarda la imagen en el FTP
                        MethodsHelper.Image =
                            FilesHelper.UploadPhoto(
                                view.ImageFile,
                                MethodsHelper.GetFolderSoccerFlag(),
                                Convert.ToString(team.TeamId).Trim());

                        //  CEHJ - Da formato a la imagen
                        MethodsHelper.Image =
                            string.Format(
                                "{0}{1}",
                                MethodsHelper.GetFolderSoccerFlag(),
                                MethodsHelper.Image);

                        team.ImagePath = MethodsHelper.Image;
                        db.Entry(team).State = EntityState.Modified;
                        response = await DbHelper.SaveChangeDB(db);

                        if (response.IsSuccess)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                            return View(view);
                        }
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }

            //  return View(team);
            return View(view);
        }

        // GET: Teams/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            var view = TeamToTeamView(team);

            //  return View(team);
            return View(view);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //  public async Task<ActionResult> Edit(Team team)
        public async Task<ActionResult> Edit(TeamView view)
        {
            if (ModelState.IsValid)
            {
                //if (view.ImageFile != null)
                //{
                //    MethodsHelper.Image = view.ImagePath;

                //    MethodsHelper.Image =
                //        FilesHelper.UploadPhoto(
                //            view.ImageFile,
                //            MethodsHelper.GetFolderSoccerFlag());

                //    MethodsHelper.Image =
                //        string.Format(
                //            "{0}{1}",
                //            MethodsHelper.GetFolderSoccerFlag(),
                //            MethodsHelper.Image);
                //}

                if (view.ImageFile != null)
                {
                    MethodsHelper.Image = view.ImagePath;

                    //  CEHJ - Guarda la imagen en el FTP
                    MethodsHelper.Image =
                        FilesHelper.UploadPhoto(
                            view.ImageFile,
                            MethodsHelper.GetFolderSoccerFlag(),
                            Convert.ToString(view.TeamId).Trim());

                    //  CEHJ - Da formato a la imagen
                    MethodsHelper.Image =
                        string.Format(
                            "{0}{1}",
                            MethodsHelper.GetFolderSoccerFlag(),
                            MethodsHelper.Image);
                }

                var team = TeamViewToTeam(view);
                team.ImagePath = MethodsHelper.Image;

                db.Entry(team).State = EntityState.Modified;
                response = await DbHelper.SaveChangeDB(db);

                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }

            //  return View(team);
            return View(view);
        }

        // GET: Teams/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var team = await db.Teams.FindAsync(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var team = await db.Teams.FindAsync(id);
            //  Captura la ruta del archivo
            var imagePath = Server.MapPath(Url.Content(team.ImagePath));
            db.Teams.Remove(team);
            response = await DbHelper.SaveChangeDB(db);

            if (response.IsSuccess)
            {

                //  Elimina el archivo
                if (FilesHelper.ExistFile(imagePath))
                {
                    var response = FilesHelper.DeleteFile(imagePath);
                }
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(team);
        } 

        #endregion Methods View

        #region Methods

        private Team TeamViewToTeam(TeamView view)
        {
            return new Team
            {
                ImagePath = view.ImagePath,
                Name = view.Name,
                TeamId = view.TeamId,
            };
        }

        private TeamView TeamToTeamView(Team team)
        {
            return new TeamView
            {
                ImagePath = team.ImagePath,
                Name = team.Name,
                TeamId = team.TeamId,
            };
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