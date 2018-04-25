namespace Lands.API.Helpers
{
    using Lands.API.Models;
    using Lands.Domain.Others;
    using System;
    using System.Threading.Tasks;

    public class DbHelper
    {
        public static async Task<Response> SaveChangeDB(DataContextLocal dbLocal)
        {
            try
            {
                await dbLocal.SaveChangesAsync();
                return new Response
                {
                    IsSuccess = true,
                    Message = "Save is ok...!!!",
                    Result = null,
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && 
                    ex.InnerException.InnerException != null && 
                    !string.IsNullOrEmpty(ex.InnerException.InnerException.Message))
                {
                    if (ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = "The record can't be deleted, because it has related records...!!!",
                            Result = null,
                        };
                    }
                    else if (ex.InnerException.InnerException.Message.ToUpper().Contains("INDEX"))
                    {
                        //  CHEJ - Validacion para el email
                        if (ex.InnerException.InnerException.Message.Contains("User_Email_Index"))
                        {
                            return new Response
                            {
                                IsSuccess = false,
                                Message = "The email you are using is already registered...!!!",
                                Result = null,
                            };
                        }

                        return new Response
                        {
                            IsSuccess = false,
                            Message = "There is already a record with the same name...!!!",
                            Result = null,
                        };
                    }
                    else
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message = ex.Message,
                            Result = null,
                        };
                    }
                }
                else
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = ex.Message,
                        Result = null,
                    };
                }
            }
        }
    }
}