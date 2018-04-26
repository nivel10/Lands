namespace Lands.API.Controllers
{
    using Lands.API.Models;
    using Lands.Domain.GetServices;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    [RoutePrefix("api/CneIvssDatas")]
    public class CneIvssDatasController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();

        [Authorize(Roles = "Admin")]
        // GET: api/CneIvssDatas
        public IQueryable<CneIvssData> GetCneIvssDatas()
        {
            return db.CneIvssDatas;
        }

        [Authorize(Roles = "Admin")]
        // GET: api/CneIvssDatas/5
        [ResponseType(typeof(CneIvssData))]
        public async Task<IHttpActionResult> GetCneIvssData(int id)
        {
            var cneIvssData = await db.CneIvssDatas.FindAsync(id);
            if (cneIvssData == null)
            {
                return NotFound();
            }

            return Ok(cneIvssData);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [Route("GetCneIvssDatasByUserId")]
        public async Task<IHttpActionResult> GetCneIvssDatasByUserId(int? id)
        {
            try
            {
                if (id == null)
                {
                    ModelState.AddModelError(string.Empty, "Error: Id is null...!!!");
                    return BadRequest(ModelState);
                }
                var cneIvssDatas = await db.CneIvssDatas
                    .Where(cid => cid.UserId == id)
                    .ToListAsync();

                return Ok(cneIvssDatas);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }

        [Authorize(Roles = "Admin, User")]
        // PUT: api/CneIvssDatas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCneIvssData(int id, CneIvssData cneIvssData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cneIvssData.CneIvssDataId)
            {
                return BadRequest();
            }

            db.Entry(cneIvssData).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CneIvssDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "Admin, User")]
        // POST: api/CneIvssDatas
        [ResponseType(typeof(CneIvssData))]
        public async Task<IHttpActionResult> PostCneIvssData(CneIvssData cneIvssData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CneIvssDatas.Add(cneIvssData);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = cneIvssData.CneIvssDataId }, cneIvssData);
        }

        [Authorize(Roles = "Admin, User")]
        // DELETE: api/CneIvssDatas/5
        [ResponseType(typeof(CneIvssData))]
        public async Task<IHttpActionResult> DeleteCneIvssData(int id)
        {
            var cneIvssData = await db.CneIvssDatas.FindAsync(id);
            if (cneIvssData == null)
            {
                return NotFound();
            }

            db.CneIvssDatas.Remove(cneIvssData);
            await db.SaveChangesAsync();

            return Ok(cneIvssData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CneIvssDataExists(int id)
        {
            return db.CneIvssDatas.Count(e => e.CneIvssDataId == id) > 0;
        }
    }
}