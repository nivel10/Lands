namespace Lands.BackEnd.Helpers
{
    using System;
    using System.Linq;
    using System.Web.Configuration;
    using Lands.BackEnd.Models;
    using Lands.Domain.Soccer;

    public class MethodsHelper
    {
        public static string Image { get; set; }

        public static string GetFolderSoccerFlag()
        {
            return "~/Content/Soccer/Teams/Flags/";
        }

        public static string GetPathNoImage()
        {
            return "~/Content/Images/NoImage.png";
        }

        public static int GetStatusMatchIdByName(string statusMatchName, DataContextLocal dbLocal)
        {
            var statusMatch = dbLocal.StatusMatches
                .Where(sm => sm.Name == statusMatchName)
                .FirstOrDefault();
            if (statusMatch != null)
            {
                return statusMatch.StatusMatchId;
            }
            else
            {
                //  CHEJ - Invoca el metodo que crea el StatusMacth por nombre
                return CreateStatusMatchByName(statusMatchName, dbLocal);
            }
        }

        private static int CreateStatusMatchByName(string statusMatchName, DataContextLocal dbLocal)
        {
            try
            {
                var statusMacth = new StatusMatch
                {
                     Name = statusMatchName,
                };

                dbLocal.Entry(statusMatchName);
                dbLocal.SaveChanges();

                return statusMacth.StatusMatchId;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static string GetPathUserImages()
        {
            return "~/Content/Users/Images/";
        }

        public static string GetAppNameGetServices()
        {
            var appNameGetServices =
                WebConfigurationManager.AppSettings["AppNameGetServices"].ToString().Trim();
            return appNameGetServices;
        }
    }
}