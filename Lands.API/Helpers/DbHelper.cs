namespace Lands.API.Helpers
{
    using Lands.API.Models;
    using Lands.API.Models.Test001;
    using Lands.Domain.Others;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;
    using System.Data.SqlClient;
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

        public static async Task<Response> GetDataTableGeneric(
            string _fields,
            string _tables,
            string _where,
            string _dataTableName)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = $"SELECT {_fields} FROM {_tables} ";
                        if (!string.IsNullOrEmpty(_where))
                        {
                            sqlCommand.CommandText += $"WHERE {_where}";
                        }

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            await sqlConnection.OpenAsync();

                            using (DataTable dataTable = new DataTable(_dataTableName))
                            {
                                sqlDataAdapter.Fill(dataTable);
                                sqlConnection.Close();
                                sqlConnection.Dispose();

                                return new Response
                                {
                                    IsSuccess = true,
                                    Message = "Process is ok...!",
                                    Result = dataTable,
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public static async Task<Response> GetDataTableGeneric(
            string _password,
            decimal _prioridad,
            string _mapa,
            string _dataTableName)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = "DECLARE " + char.ConvertFromUtf32(13);
                        sqlCommand.CommandText += " @RETURN_VALUE INT, " + char.ConvertFromUtf32(13);
                        sqlCommand.CommandText += " @CADENAENCRIPT VARCHAR(500) " + char.ConvertFromUtf32(13);
                        sqlCommand.CommandText += "EXEC @RETURN_VALUE = ENCRIPT " + char.ConvertFromUtf32(13);
                        sqlCommand.CommandText += $"@CADENA = N'{_password.Trim().ToUpper()}{_prioridad.ToString().Trim().ToUpper()}{_mapa.Trim().ToUpper()}', " + char.ConvertFromUtf32(13);
                        sqlCommand.CommandText += "@CADENAENCRIPT = @CADENAENCRIPT OUTPUT " + char.ConvertFromUtf32(13);
                        sqlCommand.CommandText += "SELECT RTRIM(LTRIM(LEFT(@CADENAENCRIPT, 20))) AS N'CADENAENCRIPT'" + char.ConvertFromUtf32(13);

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            await sqlConnection.OpenAsync();

                            using (DataTable dataTable = new DataTable(_dataTableName))
                            {
                                sqlDataAdapter.Fill(dataTable);
                                sqlConnection.Close();
                                sqlConnection.Dispose();

                                return new Response
                                {
                                    IsSuccess = true,
                                    Message = "Process is ok...!",
                                    Result = dataTable,
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public static async Task<Response> GetDataTableGeneric(
            string _sqlQuery, 
            string _dataTableName)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand sqlCommand = new SqlCommand())
                    {
                        sqlCommand.Connection = sqlConnection;
                        sqlCommand.CommandType = CommandType.Text;
                        sqlCommand.CommandText = _sqlQuery;

                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            await sqlConnection.OpenAsync();

                            using (DataTable dataTable = new DataTable(_dataTableName))
                            {
                                sqlDataAdapter.Fill(dataTable);
                                sqlConnection.Close();
                                sqlConnection.Dispose();

                                return new Response
                                {
                                    IsSuccess = true,
                                    Message = "Process is ok...!",
                                    Result = dataTable,
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["TestsConnection"].ToString();
        }

        public static async Task<Response> GetListAccessCompanies(
            List<Empresa> _companyNoAccess)
        {
            try
            {
                var dbfPathFile = "D:\\Program Files (x86)\\Softech Consultores\\";
                dbfPathFile += "Profit Plus Administrativo\\Data\\bldatos.dbc";
                //  var dbfPathFile = "C:\\Data\\bldatos.dbc";

                var dbfConnectionString = "Provider=VFPOLEDB.1 ;Data Source=";
                dbfConnectionString += $"{dbfPathFile};";

                var dbfSelectCommand = "SELECT * FROM EMPRESAS ";
                if (_companyNoAccess != null)
                {
                    dbfSelectCommand += char.ConvertFromUtf32(13) + "WHERE COD_EMP NOT IN (" + char.ConvertFromUtf32(13);
                    foreach (var _company in _companyNoAccess)
                    {
                        dbfSelectCommand += $"'{_company.Cod_Emp.Trim()}', " + char.ConvertFromUtf32(13);
                    }
                    dbfSelectCommand += " 'NIKOLE')";
                }

                if (!FilesHelper.ExistFile(dbfPathFile))
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Can not be located the companies table...!!!",
                    };
                }

                using (OleDbConnection dbfConnection = new OleDbConnection(dbfConnectionString))
                {
                    await dbfConnection.OpenAsync();

                    using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(
                        dbfSelectCommand,
                        dbfConnection))
                    {
                        using (DataTable dataTable = new DataTable())
                        {
                            oleDbDataAdapter.Fill(dataTable);
                            dbfConnection.Close();
                            dbfConnection.Dispose();

                            var lisCompanies = new List<Empresa>();

                            foreach (DataRow row in dataTable.Rows)
                            {
                                lisCompanies.Add(new Empresa
                                {
                                    Cod_Emp = row["COD_EMP"].ToString().Trim(),
                                    Backup_dir = row["Backup_dir"].ToString().Trim(),
                                    Ciudad = row["Ciudad"].ToString().Trim(),
                                    Cpostal = row["Cpostal"].ToString().Trim(),
                                    Direc1 = row["Direc1"].ToString().Trim(),
                                    Email = row["Email"].ToString().Trim(),
                                    Emp_Des = row["Emp_Des"].ToString().Trim(),
                                    Estado = row["Estado"].ToString().Trim(),
                                    Fax = row["Fax"].ToString().Trim(),
                                    FedId = row["FedId"].ToString().Trim(),
                                    Nit = row["Nit"].ToString().Trim(),
                                    Ocul_emp = bool.Parse(row["Ocul_emp"].ToString().Trim()),
                                    Pais = row["Pais"].ToString().Trim(),
                                    Prioridad = int.Parse(row["Prioridad"].ToString().Trim()),
                                    Respons = row["Respons"].ToString().Trim(),
                                    Rif = row["Rif"].ToString().Trim(),
                                    StateId = row["StateId"].ToString().Trim(),
                                    StateUid = row["StateUid"].ToString().Trim(),
                                    Telefonos = row["Telefonos"].ToString().Trim(),
                                    TipoNeg = row["TipoNeg"].ToString().Trim(),
                                    WebSite = row["WebSite"].ToString().Trim(),
                                });
                            }

                            return new Response
                            {
                                IsSuccess = true,
                                Message = "Load companies is ok...!!!",
                                Result = lisCompanies,
                            };
                        }
                    }
                }

                //return new Response { };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}