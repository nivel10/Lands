namespace Lands.API.Helpers
{
    using Lands.Domain.Others;
    using System;
    using System.IO;
    using System.Web;

    public class FilesHelper
    {
        public static string UploadPhoto(HttpPostedFileBase _httpPostedFileBase, string _folder)
        {
            string path = string.Empty;
            string pic = string.Empty;

            if (_httpPostedFileBase != null)
            {
                pic = Path.GetFileName(_httpPostedFileBase.FileName);
                path = Path.Combine(HttpContext.Current.Server.MapPath(_folder), pic);
                _httpPostedFileBase.SaveAs(path);
            }

            return pic;
        }

        public static string UploadPhoto(MemoryStream _memoryStream, string _folfer, string _nameFile)
        {
            string path = string.Empty;
            //  string pic = string.Empty;
            string name = string.Empty;

            if (_memoryStream != null)
            {
                _memoryStream.Position = 0;
                name = string.Format("{0}.jpg", _nameFile);
                path = Path.Combine(HttpContext.Current.Server.MapPath(_folfer), name);
                File.WriteAllBytes(path, _memoryStream.ToArray());
            }

            return name;
        }

        public static string UploadPhoto(HttpPostedFileBase _httpPostedFileBase, string _folder, string _nameFile)
        {
            string path = string.Empty;
            //  string pic = string.Empty;
            string name = string.Empty;

            if (_httpPostedFileBase != null)
            {
                //  pic = Path.GetFileName(file.FileName);
                name = string.Format("{0}{1}", _nameFile, Path.GetExtension(_httpPostedFileBase.FileName));
                path = Path.Combine(HttpContext.Current.Server.MapPath(_folder), name);
                _httpPostedFileBase.SaveAs(path);
            }

            return name;
            //  return pic;
        }

        //public static bool UploadPhoto(MemoryStream stream, string folder, string name)
        //{
        //    try
        //    {
        //        stream.Position = 0;
        //        var path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
        //        File.WriteAllBytes(path, stream.ToArray());
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        public static bool ExistFile(string _pahtFile)
        {
            return File.Exists(_pahtFile);
        }

        public static Response DeleteFile(string _pathFile)
        {
            try
            {
                if (ExistFile(_pathFile))
                {
                    File.Delete(_pathFile);
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