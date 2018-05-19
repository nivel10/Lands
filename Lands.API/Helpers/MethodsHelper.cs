namespace Lands.API.Helpers
{
    using System;
    using System.Linq;
    using System.Web.Http.ModelBinding;
    using Lands.API.Models;
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

        public static string GetErrorsModelState(ModelStateDictionary _modelState)
        {
            var errors = string.Empty;
            foreach (var modelState in _modelState)
            {
                if (modelState.Value.Errors.Count > 0)
                {
                    foreach (var item in modelState.Value.Errors)
                    {
                        //  errors += item.Exception.Message + System.Char.ConvertFromUtf32(13);
                        errors += 
                            (item.Exception == null ? 
                                item.ErrorMessage :
                                    item.Exception.Message) + System.Char.ConvertFromUtf32(13);
                    }
                }
            }
            return errors;
        }

        public static string GetPathUserImages()
        {
            return "~/Content/Users/Images/";
        }
    }
}