namespace Lands.API.Controllers
{
    using Lands.API.Helpers;
    using Lands.API.Models;
    using Lands.API.Models.ServicesVzLa;
    using Lands.Domain;
    using Lands.Domain.Others;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Validation;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.Http.Description;

    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();
        private string errorMessages = string.Empty;

        [HttpPost]
        [Route("LoginFacebook")]
        public async Task<IHttpActionResult> LoginFacebook(FacebookResponse _profile)
        {
            try
            {
                var user = await db.Users.Where(u => u.Email == _profile.Id).FirstOrDefaultAsync();
                if (user == null)
                {
                    user = new User
                    {
                        Email = _profile.Id,
                        FirstName = _profile.FirstName,
                        LastName = _profile.LastName,
                        ImagePath = _profile.Picture.Data.Url,
                        UserTypeId = 2,
                        Telephone = "...",
                    };

                    db.Users.Add(user);
                    UsersHelper.CreateUserASP(_profile.Id, "User", _profile.Id);
                }
                else
                {
                    user.FirstName = _profile.FirstName;
                    user.LastName = _profile.LastName;
                    user.ImagePath = _profile.Picture.Data.Url;
                    db.Entry(user).State = EntityState.Modified;
                }

                response = await DbHelper.SaveChangeDB(db);
                if (!response.IsSuccess)
                {
                    errorMessages = string.Format("Error: {0}", response.Message);
                    ModelState.AddModelError(string.Empty, errorMessages);
                    return BadRequest(errorMessages);
                }
                return Ok(true);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    errorMessages = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        errorMessages += string.Format("\n- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                return BadRequest(errorMessages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("PasswordRecovery")]
        public async Task<IHttpActionResult> PasswordRecovery(JObject form)
        {
            try
            {
                errorMessages = string.Empty;
                var email = string.Empty;
                dynamic jsonObject = form;

                email = jsonObject.Email.Value;

                var user = await db.Users
                    .Where(u => u.Email.ToLower() == email.ToLower())
                    .FirstOrDefaultAsync();
                if (user == null)
                {
                    //  return NotFound();
                    errorMessages = string.Format(
                        "Error: incorrect email (Users): {0} ...!!!",
                        email);
                    ModelState.AddModelError(string.Empty, errorMessages);
                    return BadRequest(errorMessages);
                }

                var userContext = new ApplicationDbContext();
                var userManager = 
                    new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(userContext));
                var userASP = userManager.FindByEmail(email);
                if (userASP == null)
                {
                    //  return NotFound();
                    errorMessages = string.Format(
                        "Error: incorrect email (ASP) {0} ...!!!",
                        email);
                    ModelState.AddModelError(string.Empty, errorMessages);
                    return BadRequest(errorMessages);
                }

                var random = new Random();
                var newPassword = string.Format("{0}", random.Next(100000, 999999));
                var response1 = userManager.RemovePassword(userASP.Id);
                var response2 = await userManager.AddPasswordAsync(userASP.Id, newPassword);
                if (response2.Succeeded)
                {
                    var subject = "GetServicesVzLa - Password Recovery...!!!";
                    var body = string.Format(@"
                        <h1>GetServices App - Password Recovery</h1>
                        <p>Your new password is: <strong>{0}</strong></p>
                        <p>Please, don't forget change it for one easy remember for you....!!!</p>
                        <p> </p>
                        <h1> CHEJ Consultor, C.A. </h1>
                        <h2> - Su Requerimiento es Nuestro Comrpomiso - </h2>",
                        newPassword);

                    await MailHelper.SendMail(email, subject, body);
                    return Ok(true);
                }

                return BadRequest("The password can't be changed.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        [Route("EditPassword")]
        public async Task<IHttpActionResult> EditPassword(ServicesVzLaUserEdit _user)
        {
            //  var errorMessages = string.Empty;
            errorMessages = string.Empty;
            try
            {
                var email = string.Empty;
                var currentPassword = string.Empty;
                var newPassword = string.Empty;
                //  dynamic jsonObject = _user;

                if (!ModelState.IsValid)
                {
                    //  Gets error of the ModelState
                    errorMessages = "Error: the UserEdit is not valid...!!!" + System.Char.ConvertFromUtf32(13);
                    errorMessages += MethodsHelper.GetErrorsModelState(ModelState);

                    ModelState.AddModelError(string.Empty, errorMessages);
                    return BadRequest(errorMessages);
                }

                //  email = jsonObject.Email.Value;
                //  currentPassword = jsonObject.CurrentPassword.Value;
                //  newPassword = jsonObject.NewPassword.Value;
                email = _user.Email;
                currentPassword = _user.Password;
                newPassword = _user.NewPassword;


                var userContext = new ApplicationDbContext();
                var userManager =
                    new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
                var userASP = userManager.FindByEmail(email);

                if (userASP == null)
                {
                    //  return NotFound();
                    ModelState.AddModelError(string.Empty, "Error: UserASP (By Email) is null...!!!");
                    return BadRequest("Error: UserASP (By Email) is null...!!!");
                }

                var response =
                    await userManager.ChangePasswordAsync(
                        userASP.Id,
                        currentPassword,
                        newPassword);
                if (!response.Succeeded)
                {
                    errorMessages = "Error: " + System.Char.ConvertFromUtf32(13);
                    errorMessages += response.Errors.FirstOrDefault();
                    ModelState.AddModelError(string.Empty, errorMessages);
                    return BadRequest(response.Errors.FirstOrDefault());
                }

                return Ok(true);
            }
            catch(Exception ex)
            {
                errorMessages = string.Format("Error: {0}", ex.Message);
                //  return BadRequest("Incorrect call");
                return BadRequest(errorMessages);
            }
        }

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