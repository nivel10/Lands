namespace Lands.API.Controllers
{
    using Lands.API.Models;
    using Lands.Domain.GetServices;
    using Lands.Domain.Others;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using Lands.API.Helpers;

    [RoutePrefix("api/CneIvssDatas")]
    public class CneIvssDatasController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

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
                ModelState.AddModelError(string.Empty, "Error: Id is not equal to CneIvssDataId...!!!");
                //  return BadRequest(ModelState.First().Value.Errors[0].ErrorMessage.Trim());
                return BadRequest("Error: Id is not equal to CneIvssDataId...!!!");
            }

            db.Entry(cneIvssData).State = EntityState.Modified;

            try
            {
                //  await db.SaveChangesAsync();
                response = await DbHelper.SaveChangeDB(db);
                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    //  return BadRequest(ModelState.First().Value.Errors[0].ErrorMessage.Trim());
                    return BadRequest(response.Message);
                }
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

        [Authorize(Roles = "Admin")]
        // POST: api/CneIvssDatas
        [ResponseType(typeof(CneIvssData))]
        public async Task<IHttpActionResult> PostCneIvssData(CneIvssData cneIvssData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //  Find the record if exist
            var oldCneIvssData = await db.CneIvssDatas
                .Where(cid => cid.IsCne == true && 
                       cid.IdentificationCard == cneIvssData.IdentificationCard &&
                       cid.NationalityId == cneIvssData.NationalityId &&
                       cid.UserId == cneIvssData.UserId)
                .FirstOrDefaultAsync();

            if (oldCneIvssData != null)
            {
                ModelState.AddModelError(string.Empty, "There is already a record with the same record...!!!");
                return BadRequest(ModelState.First().Value.Errors[0].ErrorMessage.Trim());
            }

            db.CneIvssDatas.Add(cneIvssData);
            response = await DbHelper.SaveChangeDB(db);

            if (response.IsSuccess)
            {
                return CreatedAtRoute("DefaultApi", new { id = cneIvssData.CneIvssDataId }, cneIvssData);
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return BadRequest(ModelState);
        }

        [Authorize(Roles = "Admin, User")]
        // POST: api/CneIvssDatas
        [Route("PostCneIvssDataInsertByOption")]
        [ResponseType(typeof(CneIvssData))]
        public async Task<IHttpActionResult> PostCneIvssDataInsertByOption(string _option, CneIvssData _cneIvssData)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Error: CneIvssData is not valid...!!!");
                //  return BadRequest(ModelState);
                return BadRequest("Error: CneIvssData is not valid...!!!");
            }

            try
            {
                //  Find the record if exist
                //var oldCneIvssData = await db.CneIvssDatas
                //    .Where(cid => cid.CneIvssDataId == _cneIvssData.CneIvssDataId)
                //    .FirstOrDefaultAsync();
                //  Find the record if exist
                var oldCneIvssData = await db.CneIvssDatas
                   .Where(cid => cid.UserId == _cneIvssData.UserId &&
                                 cid.NationalityId == _cneIvssData.NationalityId &&
                                 cid.IdentificationCard == _cneIvssData.IdentificationCard)
                   .FirstOrDefaultAsync();

                if (oldCneIvssData != null)
                {
                    switch (_option)
                    {
                        case "cne":
                            if (oldCneIvssData.IsCne == false)
                            {
                                oldCneIvssData.IsCne = true;
                                _cneIvssData.IsCne = true;
                                db.Entry(oldCneIvssData).State = EntityState.Modified;
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "There is already a record with the same record...!!!");
                                //  return BadRequest(ModelState.First().Value.Errors[0].ErrorMessage.Trim());
                                return BadRequest("There is already a record with the same record...!!!");
                            }
                                break;
                        case "ivss":
                            break;
                    }
                }
                else
                {
                    db.CneIvssDatas.Add(_cneIvssData);
                }
                
                response = await DbHelper.SaveChangeDB(db);

                if (response.IsSuccess)
                {
                    return Ok(_cneIvssData);
                    //  return CreatedAtRoute("DefaultApi", new { id = _cneIvssData.CneIvssDataId }, _cneIvssData);
                }

                ModelState.AddModelError(string.Empty, response.Message);
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        // DELETE: api/CneIvssDatas/5
        [ResponseType(typeof(CneIvssData))]
        public async Task<IHttpActionResult> DeleteCneIvssData(int id)
        {
            var cneIvssData = await db.CneIvssDatas.FindAsync(id);
            if (cneIvssData == null)
            {
                ModelState.AddModelError(string.Empty, "Error: CneIvssData is null...!!!");
                //  return NotFound();
                return BadRequest("Error: CneIvssData is null...!!!");
            }

            try
            {
                db.CneIvssDatas.Remove(cneIvssData);
                response = await DbHelper.SaveChangeDB(db);
                if(!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ex.Message);
            }
            
            return Ok(cneIvssData);
        }

        [Authorize(Roles = "Admin, User")]
        // DELETE: api/CneIvssDatas/5
        [Route("PostCneIvssDataByOption")]
        [ResponseType(typeof(CneIvssData))]
        public async Task<IHttpActionResult> PostCneIvssDataByOption(string _option, CneIvssData cneIvssData)
        {
            try
            {
                if(cneIvssData == null)
                {
                    ModelState.AddModelError(string.Empty, "Error: CneIvssData is null...!!!");
                    return BadRequest("Error: CneIvssData is null...!!!");
                }

                var findCneIvssData = await db.CneIvssDatas.FindAsync(cneIvssData.CneIvssDataId);
                if (findCneIvssData == null)
                {
                    ModelState.AddModelError(string.Empty, "Error: FindCneIvssData is null...!!!");
                    return BadRequest("Error: FindCneIvssData is null...!!!");
                }

                switch (_option)
                {
                    case "cne":
                        if (findCneIvssData.IsIvss == false)
                        {
                            db.CneIvssDatas.Remove(findCneIvssData);
                        }
                        else
                        {
                            findCneIvssData.IsCne = false;
                            db.Entry(findCneIvssData).State = EntityState.Modified;
                        }
                        break;
                }

                response = await DbHelper.SaveChangeDB(db);
                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return BadRequest(response.Message);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ex.Message);
            }

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