namespace Lands.API.Controllers
{
    using Lands.API.Helpers;
    using Lands.API.Models;
    using Lands.API.Models.ServicesVzLa;
    using Lands.Domain;
    using Lands.Domain.Others;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.Http.Description;

    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();

        [Authorize(Roles = "Admin")]
        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        [Authorize(Roles = "Admin, User")]
        [Route("GetServicesVzLaUSerByEmail")]
        public async Task<IHttpActionResult> GetServicesVzLaUSerByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError(string.Empty, "Error: email is null...!!!");
                    return BadRequest(ModelState);
                }
                
                //  Gets data of the UsetByEmail
                var userByEmail = await db.Users
                     .Include(u => u.CantvDatas)
                     .Include(u => u.CneIvssDatas)
                     .Include(u => u.ZoomDatas)
                     .Where(u => u.Email == email)
                     .FirstOrDefaultAsync();

                //  Gets data of the Nationality
                var nationalities = await db.Nationalities.ToListAsync();
                var servicesNationalities = new List<ServicesVzLaNationalityResponse>();

                if (nationalities != null)
                {
                    foreach (var nationality in nationalities)
                    {
                        servicesNationalities.Add(new ServicesVzLaNationalityResponse
                        {
                            Abbreviation = nationality.Abbreviation,
                            Name = nationality.Name,
                            NationalityId = nationality.NationalityId,
                        });
                    }
                }

                if (userByEmail == null)
                {
                    ModelState.AddModelError(string.Empty, "Error: user is null...!!!");
                    return BadRequest(ModelState);
                }

                //  Create a new object of the UserResponse
                var userResponse = new ServicesVzLaUserResponse();

                //  Load values of CantvData in the List<ServicesVzLaCantvDataResponse>
                var cantvDatas = new List<ServicesVzLaCantvDataResponse>();
                foreach (var cantvdata in userByEmail
                    .CantvDatas
                    .OrderBy(cd => cd.CodePhone)
                    .ThenBy(cd => cd.NumberPhone))
                {
                    cantvDatas.Add(new ServicesVzLaCantvDataResponse
                    {
                        CantvDataId = cantvdata.CantvDataId,
                        CodePhone = cantvdata.CodePhone,
                            NumberPhone = cantvdata.NumberPhone,
                    });
                }

                //  Load values of CneIvssData in the List<ServicesVzLaCantvDataResponse>
                var cneIvssDatas = new List<ServicesVzLaCneIvssDataResponse>();
                foreach (var cneIvssData in userByEmail
                    .CneIvssDatas
                    .OrderBy(cid => cid.Nationality.Abbreviation)
                    .ThenBy(cid => cid.IdentificationCard))
                {
                    cneIvssDatas.Add(new ServicesVzLaCneIvssDataResponse
                    {
                        BirthDate = cneIvssData.BirthDate,
                        CneIvssDataId = cneIvssData.CneIvssDataId,
                        IdentificationCard = cneIvssData.IdentificationCard,
                        IsCne = cneIvssData.IsCne,
                        IsIvss = cneIvssData.IsIvss,
                        NationalityId = cneIvssData.NationalityId,
                        NationalityDatas = servicesNationalities.Where(n => n.NationalityId == cneIvssData.NationalityId).ToList(),
                    });
                }

                //  Load values of ZoomDatas in the List<ServicesVzLaZoomDataResponse>
                var zoomDatas = new List<ServicesVzLaZoomDataResponse>();
                foreach (var zoomData in userByEmail
                    .ZoomDatas
                    .OrderBy(zd => zd.Tracking))
                {
                    zoomDatas.Add(new ServicesVzLaZoomDataResponse
                    {
                        Tracking = zoomData.Tracking,
                        ZoomDataId = zoomData.ZoomDataId,
                    });
                }
                
                userResponse.AppName = userByEmail.AppName;
                userResponse.CantvDatas = cantvDatas;
                userResponse.CneIvssDatas = cneIvssDatas;
                userResponse.Email = userByEmail.Email;
                userResponse.FirstName = userByEmail.FirstName;
                userResponse.ImageArray = userByEmail.ImageArray;
                userResponse.ImagePath = userByEmail.ImagePath;
                userResponse.LastName = userByEmail.LastName;
                userResponse.Password = userByEmail.Password;
                userResponse.Telephone = userByEmail.Telephone;
                userResponse.UserId = userByEmail.UserId;
                userResponse.UserTypeId = userByEmail.UserTypeId;
                userResponse.ZoomDatas = zoomDatas;

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
        //  [ResponseType(typeof(void))]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PutUser(int id, User _user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //  return BadRequest(ModelState);
                    ModelState.AddModelError(string.Empty, "Error: User is not valid...!!!");
                    return BadRequest("Error: User is not valid...!!!");
                }

                if (id != _user.UserId)
                {
                    //  return BadRequest();
                    ModelState.AddModelError(string.Empty, "Error: Id is not equal to UserId...!!!");
                    return BadRequest("Error: Id is not equal to UserId...!!!");
                }

                db.Entry(_user).State = EntityState.Modified;
                response = await DbHelper.SaveChangeDB(db);

                if (!response.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                    return BadRequest(response.Message);
                }

                //  CHEJ - Sube las imagenes
                if (_user.ImageArray != null &&
                    _user.ImageArray.Length > 0)
                {
                    var stream = new MemoryStream(_user.ImageArray);
                    MethodsHelper.Image = _user.ImagePath;

                    //  CHEJ - Guarda la imagen en el FTP
                    MethodsHelper.Image =
                        FilesHelper.UploadPhoto(
                            stream,
                            MethodsHelper.GetPathUserImages(),
                            Convert.ToString(_user.UserId).Trim());

                    //  CHEJ - Da formato a la imagen
                    MethodsHelper.Image =
                        string.Format(
                            "{0}{1}",
                            MethodsHelper.GetPathUserImages(),
                            MethodsHelper.Image);

                    //  Actualiza la informacion del usuario
                    _user.ImagePath = MethodsHelper.Image;
                    db.Entry(_user).State = EntityState.Modified;
                    response = await DbHelper.SaveChangeDB(db);
                    if (response.IsSuccess)
                    {
                        return Ok(_user);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                        return BadRequest(response.Message);
                    }
                }
                //  return StatusCode(HttpStatusCode.NoContent);
                return Ok(_user);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, User")]
        [ResponseType(typeof(void))]
        [Route("PutUserEdit")]
        public async Task<IHttpActionResult> PutUserEdit(int id, ServicesVzLaUserEdit _userEdit)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Error: UserEdit is not valid...!!!");
                    return BadRequest("Error: UserEdit is not valid...!!!");
                }

                if (id != _userEdit.UserId)
                {
                    ModelState.AddModelError(string.Empty, "Error: id is no equal to UserEditId, is not valid...!!!");
                    return BadRequest("Error: id is no equal to UserEditId, is not valid...!!!");
                }

                //  Find if exist the user by email
                var userEdit = await db.Users
                    .Where(us => us.Email == _userEdit.NewEmail &&
                           us.UserId != id)
                     .FirstOrDefaultAsync();
                if (userEdit != null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        string.Format(
                            "This email: {0} is already registered, you must try another ... !!!",
                            _userEdit.NewEmail));
                    return BadRequest(string.Format(
                            "This email: {0} is already registered, you must try another ... !!!",
                            _userEdit.NewEmail));
                }

                userEdit = await db.Users.FindAsync(id);
                if (userEdit != null)
                {
                    userEdit.Email = _userEdit.NewEmail;
                    db.Entry(userEdit).State = EntityState.Modified;
                    response = await DbHelper.SaveChangeDB(db);

                    if (response.IsSuccess)
                    {
                        if (await UsersHelper.UpdateUserName(_userEdit.Email, _userEdit.NewEmail))
                        {
                            return Ok("Information: Successfully updated registration...!!!");
                        }
                        else
                        {
                            ModelState.AddModelError(
                                string.Empty, 
                                string.Format(
                                    "Error Updating the email: {0} to the email {1}...!!!",
                                    _userEdit.Email,
                                    _userEdit.NewEmail));
                            return BadRequest(string.Format(
                                "Error Updating the email: {0} to the email {1}...!!!",
                                _userEdit.Email,
                                _userEdit.NewEmail));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                        return BadRequest(response.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        string.Format(
                            "This email: {0} is not registered... !!!",
                            _userEdit.Email));
                    return BadRequest(string.Format(
                        "This email: {0} is not registered... !!!",
                        _userEdit.Email));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, User")]
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
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return BadRequest(ex.Message);
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