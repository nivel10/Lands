namespace Lands.BackEnd
{
    using System.Data.Entity;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Lands.BackEnd.Helpers;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //  CHEJ - Habilita las migraciones automaticas
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<
                    Models.DataContextLocal, 
                    Migrations.Configuration>());

            // CHEJ - Invoca el metodo que hace la verificacion de datos
            CheckDataInitialize();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Metodo de inicializacion de datos
        /// </summary>
        private void CheckDataInitialize()
        {
            //  CHEJ - Metodo que crea los roles y usuario
            UsersHelper.CheckRoleAndSuperUser();
            ////  CHEJ - Metodo que crea los DcumentType (Tipos de Documentos)
            //ConfigHelper.CheckDocumentsType();
            ////  CHEJ - Metodo que crea los Departament (Estados)
            //ConfigHelper.CheckDepartments();
            ////  CHEJ - Metodo que crea las City (Ciudades)
            //ConfigHelper.CheckCities();
            ////  CHEJ - Metodo que crea los State (Estados de los Documentos)
            //ConfigHelper.CheckStates();
            ////  CHEJ - Metodo que crea los AccountTypes (Tipos de Cuentas)
            //ConfigHelper.CheckAccountTypes();
            ////  CHEJ - Metodo que crea los PaymentMethods (Metodos de Pago)
            //ConfigHelper.CheckPaymentMethods();
            ////  CHEJ - Metodo que crea los MovementTypes (Tipos de Movimientos)
            //ConfigHelper.CheckMovementTypes();
            ////  CHEJ - Metodo que crea los BankMovementTypes (Tipos de Movimientos Bancos)
            //ConfigHelper.ChecBankMovementTypes();
        }

    }
}
