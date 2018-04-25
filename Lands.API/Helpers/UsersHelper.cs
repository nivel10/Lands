namespace Lands.API.Helpers
{
    using Lands.API.Models;
    using Lands.Domain;
    using Lands.Domain.Others;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Configuration;

    public class UsersHelper : IDisposable
    {
        //  Contexto de Datos (Tablas de autenticacion)
        private static ApplicationDbContext userContext = new ApplicationDbContext();
        private static Response response = new Response();

        #region Method UserASP

        /// <summary>
        /// Metodo que elimina el usuario ASP
        /// </summary>
        /// <param name="userName">String que almacena el email</param>
        /// <returns></returns>
        public static async Task<Response> DeleteUserASP(string userName, string rolName)
        {
            //  Crea el User Manager ASP
            var userManager =
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = await userManager.FindByEmailAsync(userName);
            if (userASP == null)
            {
                response.IsSuccess = false;
                response.Message = "UserASP == null";
                return response;
            }

            //  var responseASP = await userManager.DeleteAsync(userASP);
            //  CHEJ - Busca el rol del usuario
            var rolUser = GetRolUserASP(userName);
            if (string.IsNullOrEmpty(rolUser))
            {
                response.IsSuccess = true;
                response.Message = "El Usuario no está en el rol";
            }
            else
            {
                var responseASP = await userManager.RemoveFromRoleAsync(userASP.Id, rolName);
                response.IsSuccess = responseASP.Succeeded;
            }

            return response;
        }

        /// <summary>
        /// Metodo que hace el cambio de correo(User Name)
        /// </summary>
        /// <param name="currentUserName">String que almacena el correo</param>
        /// <param name="newUserName">String que almacena el nueco correo</param>
        /// <returns></returns>
        public static async Task<bool> UpdateUserName(string currentUserName, string newUserName)
        {
            //  Crea el objeto UserManager
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = await userManager.FindByEmailAsync(currentUserName);
            if (userASP == null)
            {
                return false;
            }
            else
            {
                var newUserASP = await userManager.FindByEmailAsync(newUserName);
                if (newUserASP != null)
                {
                    return false;
                }
                else
                {
                    userASP.Email = newUserName;
                    userASP.UserName = newUserName;
                    var resposnse = await userManager.UpdateAsync(userASP);
                    return resposnse.Succeeded;
                }
            }
        }

        /// <summary>
        /// Crea los parametros necesarios
        /// </summary>
        /// <param name="roleName">String que almacena el rol (Administrador / Usuario)</param>
        public static void CheckRole(string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));

            // Check to see if Role Exists, if not create it
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        /// <summary>
        /// Metodo que verifica el usuario Admin
        /// </summary>
        public static void CheckSuperUser()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var email = WebConfigurationManager.AppSettings["AdminUser"];
            var password = WebConfigurationManager.AppSettings["AdminPassWord"];
            var rolAdmin = WebConfigurationManager.AppSettings["RoleAdmin"];
            var userASP = userManager.FindByName(email);
            if (userASP == null)
            {
                //  CreateUserASP(email, "Admin", password);
                CreateUserASP(email, rolAdmin, password);
                return;
            }

            //  userManager.AddToRole(userASP.Id, "Admin");
            userManager.AddToRole(userASP.Id, rolAdmin);
        }

        /// <summary>
        /// Metodo que hace la creacion del usuario ASP
        /// </summary>
        /// <param name="email">String que almacena el email</param>
        /// <param name="roleName">String que almacena el role (Admin - User)</param>
        public static void CreateUserASP(string email, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            //  CHEJ - Vaida si ya existe el usuario
            var userASP = userManager.FindByEmail(email);
            //var userASP = new ApplicationUser
            //{
            //    Email = email,
            //    UserName = email,
            //};
            if (userASP == null)
            {
                userASP = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                };
                //  CHEJ - Crea el usuario ASP
                userManager.Create(userASP, email);
            }
            else
            {
                userManager.RemovePassword(userASP.Id);
                userManager.AddPassword(userASP.Id, userASP.Email);
            }

            //  CHEJ - Le asigna el rol ASP al usuario
            userManager.AddToRole(userASP.Id, roleName);
        }

        public static void CreateUserASP(string email, string roleName, string password)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };

            userManager.Create(userASP, password);
            userManager.AddToRole(userASP.Id, roleName);
        }

        //public static async Task PasswordRecovery(string email, DataContextLocal dbLocal)
        //{
        //    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
        //    var userASP = userManager.FindByEmail(email);
        //    if (userASP == null)
        //    {
        //        return;
        //    }

        //    //  var user = dbLocal.Users.Where(tp => tp.UserName == email).FirstOrDefault();
        //    var user = dbLocal.Users
        //        .Where(tp => tp.Email == email)
        //        .FirstOrDefault();
        //    if (user == null)
        //    {
        //        return;
        //    }

        //    var random = new Random();
        //    var newPassword = string.Format("{0}{1}{2:04}*",
        //        user.FirstName.Trim().ToUpper().Substring(0, 1),
        //        user.LastName.Trim().ToLower(),
        //        random.Next(10000));

        //    userManager.RemovePassword(userASP.Id);
        //    userManager.AddPassword(userASP.Id, newPassword);

        //    //  var subject = "Taxes Password Recovery";
        //    var body = string.Format(@"
        //        <h1>Taxes Password Recovery</h1>
        //        <p>Yor new password is: <strong>{0}</strong></p>
        //        <p>Please change it for one, that you remember easyly",
        //        newPassword);

        //    //  await MailHelper.SendMail(email, subject, body);
        //}

        /// <summary>
        /// Metodo que optiene el rol del usuario
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetRolUserASP(string email)
        {
            var userManager =
                new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = userManager.FindByEmail(email);
            if (userASP == null)
            {
                return "";
            }

            return userManager
                .GetRoles(userASP.Id)
                .FirstOrDefault();
        }

        /// <summary>
        /// Metodo que verifica los roles de usuario y super usuarios
        /// </summary>
        public static void CheckRoleAndSuperUser()
        {
            //  Hace las creacion de los roles
            var roleAdmin = WebConfigurationManager.AppSettings["RoleAdmin"];
            var roleUser = WebConfigurationManager.AppSettings["RoleUser"];
            //  CheckRole("Admin");
            //  CheckRole("User");
            CheckRole(roleAdmin);
            CheckRole(roleUser);

            //  CHEJ - Perfil de Clienntes
            //  CheckRole("Customer");
            //  CHEJ - Perfil de Supplier
            //  UsersHelper.CheckRole("Supplier");

            //  Hace la creacion del SuperUsuario
            CheckSuperUser();
        }

        #endregion Method UserASP

        #region Method User App

        /// <summary>
        /// Metodo asincrono que busca el usuario logueado
        /// </summary>
        /// <returns></returns>
        public static async Task<User> GetAsyncUserLogin(string userName, DataContextLocal dbLocal)
        {
            try
            {
                //return await dbLocal.Users
                //        .Where(u => u.UserName == userName)
                //        .FirstOrDefaultAsync();

                return await dbLocal.Users
                        .Where(u => u.Email == userName)
                        .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return new User { };
            }
        }

        /// <summary>
        /// Metodo que busca el usuario logueado
        /// </summary>
        /// <returns></returns>
        public static User GetUserLogin(string userName, DataContextLocal dbLocal)
        {
            try
            {
                //return dbLocal.Users
                //      .Where(u => u.UserName == userName)
                //      .FirstOrDefault();

                return dbLocal.Users
                      .Where(u => u.Email == userName)
                      .FirstOrDefault();
            }
            catch (Exception)
            {
                return new User { };
            }
        }

        #endregion Method User App

        #region Methods

        public void Dispose()
        {
            userContext.Dispose();
        }

        #endregion Methods
    }
}