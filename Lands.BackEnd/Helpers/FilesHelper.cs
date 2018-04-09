namespace Lands.BackEnd.Helpers
{
    using Lands.Domain.Others;
    using System;
    using System.IO;
    using System.Web;

    public class FilesHelper
    {
        public static string UploadPhoto(HttpPostedFileBase file, string folder)
        {
            string path = string.Empty;
            string pic = string.Empty;

            if (file != null)
            {
                pic = Path.GetFileName(file.FileName);
                path = Path.Combine(HttpContext.Current.Server.MapPath(folder), pic);
                file.SaveAs(path);
            }

            return pic;
        }

        public static string UploadPhoto(HttpPostedFileBase file, string folder, string nameFile)
        {
            string path = string.Empty;
            string pic = string.Empty;
            string name = string.Empty;

            if (file != null)
            {
                pic = Path.GetFileName(file.FileName);
                name = string.Format("{0}{1}", nameFile, Path.GetExtension(file.FileName));
                path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
                file.SaveAs(path);
            }

            return name;
            //  return pic;
        }

        public static bool UploadPhoto(MemoryStream stream, string folder, string name)
        {
            try
            {
                stream.Position = 0;
                var path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
                File.WriteAllBytes(path, stream.ToArray());
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool ExistFile(string pahtFile)
        {
            return File.Exists(pahtFile);
        }

        public static Response DeleteFile(string pathFile)
        {
            try
            {
                if (ExistFile(pathFile))
                {
                    File.Delete(pathFile);
                    return new Response { IsSuccess = true, Message = "File is deleted Ok...!!!", Result = null, };
                }
                return new Response { IsSuccess = true, Message = "File is not found...!!!", Result = null, };
            }
            catch (Exception ex)
            {
                return new Response { IsSuccess = false, Message = ex.Message, Result = null, };
            }
        }
    }
}