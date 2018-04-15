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

    [Authorize(Roles = "Admin, User")]
    public class GroupsController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        #region Methods View

        // GET: Groups
        public async Task<ActionResult> Index()
        {
            return View(await db.Groups.ToListAsync());
        }

        // GET: Groups/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Asigna la Hora segun el pais
            SetLocalTaimeMatch(group);

            return View(group);
        }

        // GET: Groups/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
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

            return View(group);
        }

        // GET: Groups/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
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
            return View(group);
        }

        // GET: Groups/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var group = await db.Groups.FindAsync(id);

            db.Groups.Remove(group);
            response = await DbHelper.SaveChangeDB(db);

            if (response.IsSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }
            return View(group);
        }

        #endregion Methods View

        #region Methods Other View

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddTeam(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var group = await db.Groups.FindAsync(id);

            if (group == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Crea un objeto del GroupTeam
            var groupTeam = new GroupTeam
            {
                GroupId = group.GroupId,
            };

            //  CHEJ - Invoca el metodo de carga del ViewBag
            LoadViewBagTeams(group, groupTeam);

            return View(groupTeam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTeam(GroupTeam groupTeam)
        {
            if (ModelState.IsValid)
            {
                db.GroupTeams.Add(groupTeam);
                response = await DbHelper.SaveChangeDB(db);

                if (response.IsSuccess)
                {
                    return RedirectToAction(
                        string.Format("Details/{0}", groupTeam.GroupId));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }

            var group = await db.Groups.FindAsync(groupTeam.GroupId);

            if (group == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Carga el ViewBag de Teams
            LoadViewBagTeams(group, groupTeam);

            return View(groupTeam);
        }

        public async Task<ActionResult> DeleteTeam(int? teamId, int? groupId)
        {
            if (teamId != null)
            {
                var groupTeam = await db.GroupTeams
                    .Where(gt => gt.TeamId == teamId)
                    .FirstOrDefaultAsync();

                if (groupTeam != null)
                {
                    db.GroupTeams.Remove(groupTeam);
                    response = await DbHelper.SaveChangeDB(db);

                    if (response.IsSuccess)
                    {
                        return RedirectToAction(
                            string.Format("Details/{0}", groupId));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddMatch(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error: GroupId is null...!!!");
            }

            var group = await db.Groups.FindAsync(id);
            if (group == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error: Group is null...!!!");
            }

            var match = new Match
            {
                DateTime = DateTime.Today,
                GroupId = group.GroupId,
                StatusMatchId = MethodsHelper.GetStatusMatchIdByName("Not Started", db),
            };

            //  Carga los ViewBag
            LoadViewBagsMatchTeams(match);

            return View(match);
        }

        [HttpPost]
        public async Task<ActionResult> AddMatch(Match match)
        {
            if (ModelState.IsValid)
            {
                //  CHEJ - Valida que los equipos sean diferente
                if (match.LocalId != match.VisitorId)
                {
                    //  CHEJ - Actualiza la hora al Huso Horario Universal
                    match.DateTime = match.DateTime.ToUniversalTime();
                    db.Matches.Add(match);
                    response = await DbHelper.SaveChangeDB(db);
                    if (response.IsSuccess)
                    {
                        return RedirectToAction(string.Format("Details/{0}", match.GroupId));
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: the team local and the visitor must be different...!!!");
                }
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBagsMatchTeams(match);

            return View(match);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditMatch(int? localId, int? visitorId, int? matchId, int? groupId)
        {
            if (matchId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var match = await db.Matches.FindAsync(matchId);
            if (match == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Actualiza la fecha a formato local
            match.DateTime = match.DateTime.ToLocalTime();

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBagsMatchTeams(match);

            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditMatch(Match match)
        {
            if (ModelState.IsValid)
            {
                //  CHEJ - Valida que los equipos sean diferente
                if (match.LocalId != match.VisitorId)
                {
                    match.DateTime = match.DateTime.ToUniversalTime();
                    db.Entry(match).State = EntityState.Modified;
                    response = await DbHelper.SaveChangeDB(db);

                    if (response.IsSuccess)
                    {
                        return RedirectToAction(string.Format("Details/{0}", match.GroupId));
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error: the team local and the visitor must be different...!!!");
                } 
            }

            //  CHEJ - Carga los datos de los ViewBag
            LoadViewBagsMatchTeams(match);

            return View(match);
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMatch(int? localId, int? visitorId, int? matchId, int? groupId)
        {
            if (matchId != null)
            {
                var match = await db.Matches.FindAsync(matchId);
                if (match == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error: Match is null...!!!");
                }
                db.Matches.Remove(match);
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction(string.Format("Details/{0}", groupId));
                }
                ModelState.AddModelError(string.Empty, response.Message);
                return RedirectToAction(string.Format("Details/{0}", groupId));
            }
            else
            {
                if (groupId != null)
                {
                    ModelState.AddModelError(string.Empty, "Error: MatchId is null...!!!");
                    return RedirectToAction(string.Format("Details/{0}", groupId));
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Error: GroupId is null...!!!");
            }
        }

        #endregion Methods Other View

        #region Methods

        private void LoadViewBagTeams(Group group, GroupTeam groupTeam)
        {
            ViewBag.TeamId =
                new SelectList(CombosHelper.GetTeams(db, group), "TeamId", "Name", groupTeam.TeamId);
        }

        private void LoadViewBagsMatchTeams(Match match)
        {
            ViewBag.LocalId =
                new SelectList(CombosHelper.GetTeams(match.GroupId, match.LocalId, db), "TeamId", "Name", match.LocalId);

            ViewBag.VisitorId =
                new SelectList(CombosHelper.GetTeams(match.GroupId, match.VisitorId, db), "TeamId", "Name", match.VisitorId);
        }

        private void SetLocalTaimeMatch(Group group)
        {
            foreach (var match in group.Matches)
            {
                match.DateTime = match.DateTime.ToLocalTime();
            }
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