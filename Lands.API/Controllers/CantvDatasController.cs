namespace Lands.API.Controllers
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Lands.API.Models;
    using Lands.Domain.GetServices;

    [RoutePrefix("api/CantvDatas")]
    public class CantvDatasController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();

        [Authorize(Roles = "Admin")]
        // GET: api/CantvDatas
        public IQueryable<CantvData> GetCantvDatas()
        {
            return db.CantvDatas;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [Route("GetCantvDatasByUserId")]
        public async Task<IHttpActionResult> GetCantvDatasByUserId(int? id)
        {
            try
            {
                if (id == null)
                {
                    ModelState.AddModelError(string.Empty, "Error: Id is null...!!!");
                    return BadRequest(ModelState);
                }

                var cantvDatas = await db.CantvDatas
                    .Where(cd => cd.UserId == id)
                    .ToListAsync();

                return Ok(cantvDatas);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: api/CantvDatas/5
        [ResponseType(typeof(CantvData))]
        public async Task<IHttpActionResult> GetCantvData(int id)
        {
            var cantvData = await db.CantvDatas.FindAsync(id);
            if (cantvData == null)
            {
                return NotFound();
            }

            return Ok(cantvData);
        }

        [Authorize(Roles = "Admin, User")]
        // PUT: api/CantvDatas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCantvData(int id, CantvData cantvData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cantvData.CantvDataId)
            {
                return BadRequest();
            }

            db.Entry(cantvData).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CantvDataExists(id))
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
        // POST: api/CantvDatas
        [ResponseType(typeof(CantvData))]
        public async Task<IHttpActionResult> PostCantvData(CantvData cantvData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CantvDatas.Add(cantvData);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = cantvData.CantvDataId }, cantvData);
        }

        [Authorize(Roles = "Admin, User")]
        // DELETE: api/CantvDatas/5
        [ResponseType(typeof(CantvData))]
        public async Task<IHttpActionResult> DeleteCantvData(int id)
        {
            var cantvData = await db.CantvDatas.FindAsync(id);
            if (cantvData == null)
            {
                return NotFound();
            }

            db.CantvDatas.Remove(cantvData);
            await db.SaveChangesAsync();

            return Ok(cantvData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CantvDataExists(int id)
        {
            return db.CantvDatas.Count(e => e.CantvDataId == id) > 0;
        }
    }
}