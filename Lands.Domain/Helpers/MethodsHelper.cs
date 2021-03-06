﻿namespace Lands.Domain.Helpers
{
    using Lands.Domain.Connection;
    using Lands.Domain.Soccer;
    using System;
    using System.Linq;

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

        public static int GetStatusMatchIdByName(string statusMatchName, DataContext dbLocal)
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

        private static int CreateStatusMatchByName(string statusMatchName, DataContext dbLocal)
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
    }
}