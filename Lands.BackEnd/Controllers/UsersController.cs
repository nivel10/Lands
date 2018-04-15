namespace Lands.BackEnd.Controllers
{
    using Lands.BackEnd.Helpers;
    using Lands.BackEnd.Models;
    using Lands.Domain;
    using Lands.Domain.Others;
    using System;
    using System.Data.Entity;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Configuration;
    using System.Web.Mvc;

    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private DataContextLocal db = new DataContextLocal();
        private Response response = new Response();
        
        // GET: Users
        public async Task<ActionResult> Index()
        {
            var users = db.Users.Include(u => u.UserType);
            return View(await users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            //  Crea un nuevo UserView
            var userView = new UserView();

            //  CHEJ - Carga el ViewBag tipo de usuario
            LoadViewBagUserType(null, userView.UserTypeId);

            return View(userView);
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //  public async Task<ActionResult> Create(User user)
        public async Task<ActionResult> Create(UserView userView)
        {
            if (ModelState.IsValid)
            {
                //  CHEJ - Transforma el objeto UserView a User
                var user = this.UserViewToUser(userView);

                db.Users.Add(user);
                response = await DbHelper.SaveChangeDB(db);

                if (response.IsSuccess)
                {
                    //  CHEJ - Crea el usuario en ASP.Net
                    var roleUser = WebConfigurationManager.AppSettings["RoleUser"].ToString();
                    UsersHelper.CheckRole(roleUser);
                    UsersHelper.CreateUserASP(user.Email, roleUser);

                    //  CHEJ - Guarda la imagen del usuario
                    if (userView.ImageFile != null)
                    {
                        MethodsHelper.Image = user.ImagePath;

                        //  CHEJ - Guarda la imagen en el FTP
                        MethodsHelper.Image = 
                            FilesHelper.UploadPhoto(
                                userView.ImageFile,
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
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, response.Message);
                            return View(userView);
                        }
                    }

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  CHEJ - Carga el ViewBag tipo de usuario
            LoadViewBagUserType(null, userView.UserTypeId);

            return View(userView);
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            
            //  CHEJ - Carga el ViewBag de los tipos de usuarios
            LoadViewBagUserType(user, 0);

            //  CHEJ - Transforma el objeto User en ViewUser
            var userView = UserToViewUser(user);

            return View(userView);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //  public async Task<ActionResult> Edit(User user)
        public async Task<ActionResult> Edit(UserView userView)
        {
            if (ModelState.IsValid)
            {
                //  CHEJ - Inicializa la variable de imagen
                MethodsHelper.Image = userView.ImagePath;

                //  CHEJ - Valida si el usuario cambio de imagen
                if (userView.ImageFile != null)
                {
                    //  CHEJ - Sube la imagen al FTP
                    MethodsHelper.Image =
                        FilesHelper.UploadPhoto(
                            userView.ImageFile,
                            MethodsHelper.GetPathUserImages(), 
                            Convert.ToString(userView.UserId).Trim());

                    //  CHEJ - Da formato a la imagen
                    MethodsHelper.Image =
                        string.Format(
                            "{0}{1}",
                            MethodsHelper.GetPathUserImages(),
                            MethodsHelper.Image);
                }

                //  CHEJ - Actualiza la ruta de la imagen
                userView.ImagePath = MethodsHelper.Image;

                //  CHEJ - Transforma el objeto de UserView a User
                var user = UserViewToUser(userView);

                db.Entry(user).State = EntityState.Modified;
                response = await DbHelper.SaveChangeDB(db);
                if (response.IsSuccess)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError(string.Empty, response.Message);
            }

            //  CHEJ - Hace la carga de datos de los ViewBag
            LoadViewBagUserType(null, userView.UserTypeId);

            return View(userView);
        }

        // GET: Users/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var user = await db.Users.FindAsync(id);
            //  CHEJ - Captura la ruta de la imagen
            var imagePath = Server.MapPath(Url.Content(user.ImagePath));

            db.Users.Remove(user);
            response = await DbHelper.SaveChangeDB(db);

            if (response.IsSuccess)
            {
                //  CHEJ - Elimina el archivo fisico
                if(FilesHelper.ExistFile(imagePath))
                {
                    response = FilesHelper.DeleteFile(imagePath);
                }
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(user);
        }

        private void LoadViewBagUserType(User user, int userTypeId)
        {
            ViewBag.UserTypeId =
                new SelectList(
                    CombosHelper.GetUserType(db), 
                    "UserTypeId", 
                    "Name", 
                    user == null ?
                        userTypeId : 
                            user.UserTypeId);
        }

        private User UserViewToUser(UserView view)
        {
            return new User
            {
                Email = view.Email,
                FirstName = view.FirstName,
                ImageArray = view.ImageArray,
                ImagePath = view.ImagePath,
                LastName = view.LastName,
                Password = view.Password,
                Telephone = view.Telephone,
                UserTypeId = view.UserTypeId,
                UserId = view.UserId,
            };
        }

        private UserView UserToViewUser(User user)
        {
            return new UserView
            {
                Email = user.Email,
                FirstName = user.FirstName,
                ImageArray = user.ImageArray,
                ImagePath = user.ImagePath,
                LastName = user.LastName,
                Password = user.Password,
                Telephone = user.Telephone,
                UserId = user.UserId,
                UserTypeId = user.UserTypeId,
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}