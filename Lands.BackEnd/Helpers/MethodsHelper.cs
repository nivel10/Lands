namespace Lands.BackEnd.Helpers
{
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
    }
}