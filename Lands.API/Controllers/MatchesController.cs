namespace Lands.API.Controllers
{
    using Lands.API.Models.Soccer;
    using Lands.Domain.Connection;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;

    //  En el API el Authorize solicita Token<>
    [Authorize]
    public class MatchesController : ApiController
    {
        private DataContext db = new DataContext();

        //// GET: api/Matches
        //public IQueryable<Match> GetMatches()
        //{
        //    return db.Matches;
        //}

        public async Task<IHttpActionResult> GetMatches()
        {
            //  Se crea la lista a partir del objeto MatchResponse
            var response = new List<MatchResponse>();
            //  Se optiene la lista de los Matches
            var matches = await db.Matches.ToListAsync();

            foreach (var match in matches)
            {
                response.Add(new MatchResponse
                {
                    DateTime = match.DateTime,
                    Group = match.Group,
                    GroupId = match.GroupId,
                    Local = match.Local,
                    LocalGoals = match.LocalGoals,
                    LocalId = match.LocalId,
                    MatchId = match.MatchId,
                    Predictions = match.Predictions.ToList(),
                    StatusMatch = match.StatusMatch,
                    StatusMatchId = match.StatusMatchId,
                    Visitor = match.Visitor,
                    VisitorGoals = match.VisitorGoals,
                    VisitorId = match.VisitorId,
                });
            }

            return Ok(response);
        }

        //// GET: api/Matches/5
        //[ResponseType(typeof(Match))]
        //public async Task<IHttpActionResult> GetMatch(int id)
        //{
        //    Match match = await db.Matches.FindAsync(id);
        //    if (match == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(match);
        //}

        //// PUT: api/Matches/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutMatch(int id, Match match)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != match.MatchId)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(match).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MatchExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //// POST: api/Matches
        //[ResponseType(typeof(Match))]
        //public async Task<IHttpActionResult> PostMatch(Match match)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.Matches.Add(match);
        //    await db.SaveChangesAsync();

        //    return CreatedAtRoute("DefaultApi", new { id = match.MatchId }, match);
        //}

        //// DELETE: api/Matches/5
        //[ResponseType(typeof(Match))]
        //public async Task<IHttpActionResult> DeleteMatch(int id)
        //{
        //    Match match = await db.Matches.FindAsync(id);
        //    if (match == null)
        //    {
        //        return NotFound();
        //    }

        //    db.Matches.Remove(match);
        //    await db.SaveChangesAsync();

        //    return Ok(match);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MatchExists(int id)
        {
            return db.Matches.Count(e => e.MatchId == id) > 0;
        }
    }
}