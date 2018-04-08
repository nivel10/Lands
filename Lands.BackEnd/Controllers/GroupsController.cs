﻿namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Helpers;
    using Lands.BackEnd.Models;
    using Lands.Domain.Soccer;
    using System.Data.Entity;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class GroupsController : Controller
    {
        private DataContextLocal db = new DataContextLocal();

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
            return View(group);
        }

        // GET: Groups/Create
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
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
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
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
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
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #endregion Methods View

        #region Methods Other View

        // GET: Groups/AddTeam/5
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
            LoadViewBag(group, groupTeam);

            return View(groupTeam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddTeam(GroupTeam groupTeam)
        {
            if (ModelState.IsValid)
            {
                db.GroupTeams.Add(groupTeam);
                await db.SaveChangesAsync();
                return RedirectToAction(
                    string.Format("Details/{0}", groupTeam.GroupId));
            }

            var group = await db.Groups.FindAsync(groupTeam.GroupId);

            if (group == null)
            {
                return HttpNotFound();
            }

            //  CHEJ - Carga el ViewBag de Teams
            LoadViewBag(group, groupTeam);

            return View(groupTeam);
        }

        #endregion Methods Other View

        #region Methods

        private void LoadViewBag(Group group, GroupTeam groupTeam)
        {
            ViewBag.TeamId =
                new SelectList(CombosHelper.GetTeams(db, group), "TeamId", "Name", groupTeam.TeamId);
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