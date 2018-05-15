namespace Lands.API.Controllers
{
    using Lands.API.Models;
    using Lands.API.Helpers;
    using Lands.Domain.GetServices;
    using Lands.Domain.Others;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;
    using System;

    public class ZoomDatasController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        [Authorize(Roles = "Admin")]
        // GET: api/ZoomDatas
        public IQueryable<ZoomData> GetZoomDatas()
        {
            return db.ZoomDatas;
        }

        [Authorize(Roles = "Admin")]
        // GET: api/ZoomDatas/5
        [ResponseType(typeof(ZoomData))]
        public async Task<IHttpActionResult> GetZoomData(int id)
        {
            ZoomData zoomData = await db.ZoomDatas.FindAsync(id);
            if (zoomData == null)
            {
                return NotFound();
            }

            return Ok(zoomData);
        }

        [Authorize(Roles = "Admin, User")]
        // PUT: api/ZoomDatas/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutZoomData(int id, ZoomData zoomData)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Error: ZoomData is null...!!!");
                //  return BadRequest(ModelState);
                return BadRequest("Error: ZoomData is null...!!!");
            }

            if (id != zoomData.ZoomDataId)
            {
                ModelState.AddModelError(string.Empty, "Error: Id != ZoomData.ZoomDataId...!!!");
                //  return BadRequest(ModelState);
                return BadRequest("Error: Id != ZoomData.ZoomDataId...!!!");
            }

            db.Entry(zoomData).State = EntityState.Modified;

            try
            {
                response = await DbHelper.SaveChangeDB(db);
                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return BadRequest(response.Message);
                }

                return Ok(zoomData);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZoomDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //  return StatusCode(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "Admin, User")]
        // POST: api/ZoomDatas
        [ResponseType(typeof(ZoomData))]
        public async Task<IHttpActionResult> PostZoomData(ZoomData zoomData)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Error: ZoomData is not valid...!!!");
                //  return BadRequest(ModelState);
                return BadRequest("Error: ZoomData is not valid...!!!");
            }

            try
            {
                db.ZoomDatas.Add(zoomData);
                response = await DbHelper.SaveChangeDB(db);

                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    //  return BadRequest(ModelState);
                    return BadRequest(response.Message);
                }

                return Ok(zoomData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                //  return BadRequest(ModelState);
                return BadRequest(ex.Message);
            }
            
            //  return CreatedAtRoute("DefaultApi", new { id = zoomData.ZoomDataId }, zoomData);
        }

        [Authorize(Roles = "Admin, User")]
        // DELETE: api/ZoomDatas/5
        [ResponseType(typeof(ZoomData))]
        public async Task<IHttpActionResult> DeleteZoomData(int id)
        {
            try
            {
                var zoomData = await db.ZoomDatas.FindAsync(id);
                if (zoomData == null)
                {
                    //  return NotFound();
                    ModelState.AddModelError(string.Empty, "Error: ZoomData is null...!!!");
                    return BadRequest("Error: ZoomData is null...!!!");
                }

                db.ZoomDatas.Remove(zoomData);
                response = await DbHelper.SaveChangeDB(db);
                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return BadRequest(response.Message);
                }
                
                return Ok(zoomData);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ex.Message);
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

        private bool ZoomDataExists(int id)
        {
            return db.ZoomDatas.Count(e => e.ZoomDataId == id) > 0;
        }
    }
}