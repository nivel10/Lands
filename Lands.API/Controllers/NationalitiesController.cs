namespace Lands.API.Controllers
{
    using Lands.API.Models;
    using Lands.API.Models.ServicesVzLa;
    using Lands.Domain.GetServices;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    public class NationalitiesController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();

        [Authorize(Roles = "Admin, User")]
        // GET: api/Nationalities
        //public IQueryable<Nationality> GetNationalities()
        //{
        //    return db.Nationalities;
        //}
        public async Task<IHttpActionResult> GetNationalities()
        {
            var nationalities = await db.Nationalities
                .OrderBy(n => n.NationalityId)
                .ToListAsync();

            var nationalitiesResponse = 
                new List<ServicesVzLaNationalityResponse>();
            foreach (var nationality in nationalities)
            {
                nationalitiesResponse.Add(new ServicesVzLaNationalityResponse
                {
                    Abbreviation = nationality.Abbreviation,
                    Name = nationality.Name,
                    NationalityId = nationality.NationalityId,
                });
            }

            return Ok(nationalitiesResponse);
        }

        [Authorize(Roles = "Admin, User")]
        // GET: api/Nationalities/5
        [ResponseType(typeof(Nationality))]
        public async Task<IHttpActionResult> GetNationality(int id)
        {
            var nationality = await db.Nationalities.FindAsync(id);
            if (nationality == null)
            {
                return NotFound();
            }

            return Ok(nationality);
        }

        [Authorize(Roles = "Admin")]
        // PUT: api/Nationalities/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNationality(int id, Nationality nationality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != nationality.NationalityId)
            {
                return BadRequest();
            }

            db.Entry(nationality).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NationalityExists(id))
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

        [Authorize(Roles = "Admin")]
        // POST: api/Nationalities
        [ResponseType(typeof(Nationality))]
        public async Task<IHttpActionResult> PostNationality(Nationality nationality)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Nationalities.Add(nationality);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = nationality.NationalityId }, nationality);
        }

        [Authorize(Roles = "Admin")]
        // DELETE: api/Nationalities/5
        [ResponseType(typeof(Nationality))]
        public async Task<IHttpActionResult> DeleteNationality(int id)
        {
            Nationality nationality = await db.Nationalities.FindAsync(id);
            if (nationality == null)
            {
                return NotFound();
            }

            db.Nationalities.Remove(nationality);
            await db.SaveChangesAsync();

            return Ok(nationality);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NationalityExists(int id)
        {
            return db.Nationalities.Count(e => e.NationalityId == id) > 0;
        }
    }
}