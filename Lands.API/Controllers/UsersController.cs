namespace Lands.API.Controllers
{
    using Lands.API.Helpers;
    using Lands.API.Models;
    using Lands.Domain;
    using Lands.Domain.Others;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.Http.Description;

    public class UsersController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        //  [Authorize(Roles = "Admin")]
        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        [Authorize(Roles = "Admin, User")]
        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize(Roles = "Admin, User")]
        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                try
                {
                    //  CHEJ - Crea el usuario en la App
                    db.Users.Add(user);
                    response = await DbHelper.SaveChangeDB(db);
                    if (!response.IsSuccess)
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                        return BadRequest(ModelState);
                    }

                    //  CHEJ - Sube las imagenes
                    if (user.ImageArray != null && 
                        user.ImageArray.Length > 0)
                    {
                        var stream = new MemoryStream(user.ImageArray);
                        MethodsHelper.Image = user.ImagePath;

                        //  CHEJ - Guarda la imagen en el FTP
                        MethodsHelper.Image =
                            FilesHelper.UploadPhoto(
                                stream,
                                MethodsHelper.GetPathUserImages(),
                                Convert.ToString(user.UserId).Trim());

                        //  CHEJ - Da formato a la imagen
                        MethodsHelper.Image =
                            string.Format(
                                "{0}{1}",
                                MethodsHelper.GetPathUserImages(),
                                MethodsHelper.Image);

                        //  Actualiza la informacion del usuario
                        user.ImagePath = MethodsHelper.Image;
                        db.Entry(user).State = EntityState.Modified;
                        response = await DbHelper.SaveChangeDB(db);
                        if (response.IsSuccess)
                        {
                            return Ok();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                            return BadRequest(ModelState);
                        }
                    }
                    
                    //  CHEJ - Crea el usuario ASP
                    UsersHelper.CreateUserASP(
                        user.Email,
                        WebConfigurationManager.AppSettings["RoleUser"].ToString().Trim(), 
                        user.Password);

                    //  return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ModelState);
                }
            }
        }

        [Authorize(Roles = "Admin")]
        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserId == id) > 0;
        }
    }
}