namespace Lands.API.Controllers.Test001
{
    using Lands.API.Helpers;
    using Lands.API.Models.Test001;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using System.Web.Http;

    [RoutePrefix("api/Test")]
    public class TestsController : ApiController
    {
        [Route("GetEmployees")]
        public async Task<IHttpActionResult> GetEmployess()
        {
            try
            {
                var list = new List<Employee>();

                var response = await DbHelper.GetDataTableGeneric(
                    "*",
                    "EMPLOYEE",
                    "",
                    "Employees");

                if (response.IsSuccess)
                {
                    //  var sqlDataTable = (DataTable)response.Result;
                    list = LoadDataListEmployee((DataTable)response.Result);
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAuthenticateEmployee")]
        public async Task<IHttpActionResult> GetAuthenticateEmployee(
            JObject _form)
        {
            var employee = new Employee();
            try
            {
                dynamic jsonObject = _form;
                try
                {
                    string userId = jsonObject.UserId.Value;
                    string userPassword = jsonObject.UserPassword.Value;

                    var response = await DbHelper.GetDataTableGeneric(
                        "*",
                        "EMPLOYEE",
                        $"EMPLOYEE_I = '{userId.Replace("'", "''")}'",
                        "Employee");
                    if (!response.IsSuccess)
                    {
                        employee.Message = response.Message;
                        //  return Ok(employee);
                        return BadRequest(employee.Message);
                    }

                    employee = this.LoadDataEmployee((DataTable)response.Result);
                    if (string.IsNullOrEmpty(employee.Employee_i))
                    {
                        employee.Message = "User or password wrong, please check...!!!";
                        return BadRequest(employee.Message);
                        //  return Ok(employee);
                    }

                    response = await DbHelper.GetDataTableGeneric(
                        userPassword,
                        employee.Prioridad,
                        employee.Mapa,
                        "Employee");

                    if (!response.IsSuccess)
                    {
                        employee.Message = "User or password wrong, please check...!!!";
                        return BadRequest(employee.Message);
                        //  return Ok(employee);
                    }

                    var isAuthenticated = (DataTable)response.Result;
                    var userPasswordAuthenticate = isAuthenticated.Rows[0]["CADENAENCRIPT"].ToString().Trim();
                    var userPasswordObject = employee.Password.ToString().Trim().Replace("\u001d", "");
                    if (userPasswordAuthenticate != userPasswordObject)
                    {
                        employee.Message = "User or password wrong, please check...!!!";
                        return BadRequest(employee.Message);
                        //  return Ok(employee);
                    }

                    response = await DbHelper.GetListAccessCompanies(employee.EmpresasSinAcceso);
                    if (!response.IsSuccess)
                    {
                        employee.Message = response.Message;
                        return BadRequest(employee.Message);
                        //  return Ok(employee);
                    }

                    employee.EmpresaConAcceso = (List<Empresa>)response.Result;
                    employee.Employee_p = userPassword.Trim();
                    employee.Message = "The user has successfully authenticated...!!!";
                    return Ok(employee);
                }
                catch (Exception ex)
                {
                    employee.Message = ex.Message;
                    return BadRequest(employee.Message);
                    //  return Ok(employee);
                }
            }
            catch (Exception ex)
            {
                employee.Message = ex.Message;
                return BadRequest(employee.Message);
                //  return Ok(employee);
            }
        }

        [HttpPost]
        [Route("GetCompanyConnection")]
        public async Task<IHttpActionResult> GetCompanyConnection(
            object _form)
        {
            try
            {
                dynamic objectJason = _form;

                string companyId = objectJason.CompanyId.Value;
                var response = await DbHelper.GetDataTableGeneric(
                    "*",
                    "SYS.DATABASES", $"NAME = '{companyId}' AND STATE = 0",
                    "CompanyConnection");

                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }

                var companyCoonection = (DataTable)response.Result;
                if (companyCoonection.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(companyCoonection.Rows[0][0].ToString().Trim()))
                    {
                        return BadRequest();
                    }

                    return Ok(companyCoonection.Rows[0][0].ToString().Trim());
                }
                else
                {
                    return BadRequest("Error: The database is not exist...!!!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCustomerStatementAccount")]
        public async Task<IHttpActionResult> GetCustomerStatementAccount(
            object _form)
        {
            var ListCustomerStatementAccount =
                new List<CustomerStatementAccount>();
            try
            {
                #region Data Json

                dynamic JsonForm = _form;
                string fromCustomerId = JsonForm.FromCustomerId.Value;
                string toCustomerId = JsonForm.ToCustomerId.Value;
                DateTime fromDate = DateTime.Parse(JsonForm.FromDate.Value);
                DateTime toDate = DateTime.Parse(JsonForm.ToDate.Value);
                string documentState = JsonForm.DocumentState.Value;
                string dataBaseName = JsonForm.DataBaseName.Value;

                #endregion Data Jason

                var response = await DbHelper.GetDataTableGeneric(
                    this.GetQueryCustomerStatement(
                        fromCustomerId,
                        toCustomerId,
                        fromDate,
                        toDate,
                        documentState,
                        dataBaseName),
                    "CustomerStatementAccount");
                if (!response.IsSuccess)
                {
                    return BadRequest(response.Message);
                }

                var dataTableCustomerStatementAccount = (DataTable)response.Result;
                if (dataTableCustomerStatementAccount.Rows.Count <= 0)
                {
                    return Ok("Data not found...!!!");
                }

                //  Load data Customer Statement Account
                ListCustomerStatementAccount =
                    this.LoadDataCustomerStatementAccount(dataTableCustomerStatementAccount);

                return Ok(ListCustomerStatementAccount);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Methods

        private List<Employee> LoadDataListEmployee(DataTable _sqlDataTable)
        {
            var list = new List<Employee>();

            foreach (DataRow row in _sqlDataTable.Rows)
            {
                list.Add(new Employee
                {
                    Activo = bool.Parse(row["Activo"].ToString()),
                    AdLogin = row["AdLogin"].ToString().Trim(),
                    Camb_sucu = bool.Parse(row["Camb_sucu"].ToString()),
                    Employee_i = row["Employee_i"].ToString().Trim(),
                    Empresas = row["Empresas"].ToString().Trim(),
                    EmpresasSinAcceso = string.IsNullOrEmpty(row["Empresas"].ToString().Trim()) ?
                        null
                            : this.GetListEmpresas(row["Empresas"].ToString().Trim()),
                    Estado = row["Estado"].ToString().Trim(),
                    Fec_prox = DateTime.Parse(
                        string.IsNullOrEmpty(row["Fec_prox"].ToString().Trim()) ?
                            "01/01/2000" :
                                row["Fec_prox"].ToString().Trim()),
                    Fec_ult = DateTime.Parse(
                        string.IsNullOrEmpty(row["Fec_ult"].ToString().Trim()) ?
                            "01/01/2000" :
                                row["Fec_ult"].ToString().Trim()),
                    Fec_Ult_FA = DateTime.Parse(
                        string.IsNullOrEmpty(row["Fec_Ult_FA"].ToString().Trim()) ?
                            "01/01/2000" :
                                row["Fec_Ult_FA"].ToString().Trim()),
                    Group_id = row["Group_id"].ToString().Trim(),
                    Idioma = row["Idioma"].ToString().Trim(),
                    Last_name = row["Last_name"].ToString().Trim(),
                    Mapa = row["Mapa"].ToString().Trim(),
                    Password = row["Password"].ToString().Trim(),
                    Pcontrol_1 = bool.Parse(row["Pcontrol_1"].ToString()),
                    Pcontrol_2 = bool.Parse(row["Pcontrol_2"].ToString()),
                    Pide_sucu = bool.Parse(row["Pide_sucu"].ToString()),
                    Prioridad = decimal.Parse(row["Prioridad"].ToString()),
                    Serie = int.Parse(row["Serie"].ToString()),
                    Sucursal = row["Sucursal"].ToString().Trim(),
                    User_nodo = row["User_nodo"].ToString().Trim(),
                    Veces = int.Parse(row["Veces"].ToString()),
                });
            }

            return list;
        }

        private Employee LoadDataEmployee(DataTable sqlDataTable)
        {
            var employee = new Employee();

            foreach (DataRow row in sqlDataTable.Rows)
            {
                employee.Activo = bool.Parse(row["Activo"].ToString());
                employee.AdLogin = row["AdLogin"].ToString().Trim();
                employee.Camb_sucu = bool.Parse(row["Camb_sucu"].ToString());
                employee.Employee_i = row["Employee_i"].ToString().Trim();
                employee.Empresas = row["Empresas"].ToString().Trim();
                employee.EmpresasSinAcceso = string.IsNullOrEmpty(row["Empresas"].ToString().Trim()) ?
                    null :
                        this.GetListEmpresas(row["Empresas"].ToString().Trim());
                employee.Estado = row["Estado"].ToString().Trim();
                employee.Fec_prox = DateTime.Parse(
                    string.IsNullOrEmpty(row["Fec_prox"].ToString().Trim()) ?
                        "01/01/2000" :
                            row["Fec_prox"].ToString().Trim());
                employee.Fec_ult = DateTime.Parse(
                    string.IsNullOrEmpty(row["Fec_ult"].ToString().Trim()) ?
                        "01/01/2000" :
                            row["Fec_ult"].ToString().Trim());
                employee.Fec_Ult_FA = DateTime.Parse(
                    string.IsNullOrEmpty(row["Fec_Ult_FA"].ToString().Trim()) ?
                        "01/01/2000" :
                            row["Fec_Ult_FA"].ToString().Trim());
                employee.Group_id = row["Group_id"].ToString().Trim();
                employee.Idioma = row["Idioma"].ToString().Trim();
                employee.Last_name = row["Last_name"].ToString().Trim();
                employee.Mapa = row["Mapa"].ToString().Trim();
                employee.Password = row["Password"].ToString().Trim();
                employee.Pcontrol_1 = bool.Parse(row["Pcontrol_1"].ToString());
                employee.Pcontrol_2 = bool.Parse(row["Pcontrol_2"].ToString());
                employee.Pide_sucu = bool.Parse(row["Pide_sucu"].ToString());
                employee.Prioridad = decimal.Parse(row["Prioridad"].ToString());
                employee.Serie = int.Parse(row["Serie"].ToString());
                employee.Sucursal = row["Sucursal"].ToString().Trim();
                employee.User_nodo = row["User_nodo"].ToString().Trim();
                employee.Veces = int.Parse(row["Veces"].ToString());
            }

            return employee;
        }

        private List<Empresa> GetListEmpresas(string _empresas)
        {
            try
            {
                var listEmpresas = new List<Empresa>();
                var empresa = string.Empty;

                for (int i = 0; i < _empresas.Length; i++)
                {
                    if (_empresas.Substring(i, 1) == "]")
                    {
                        listEmpresas.Add(new Empresa
                        {
                            Cod_Emp = empresa,
                        });
                        empresa = string.Empty;
                    }
                    else if (_empresas.Substring(i, 1) != "[" &&
                        !string.IsNullOrEmpty(_empresas.Substring(i, 1)))
                    {
                        empresa += _empresas.Substring(i, 1);
                    }
                }
                return listEmpresas;
            }
            catch
            {
                return new List<Empresa>();
            }
        }

        private string GetQueryCustomerStatement(
            string _fromCustomerId,
            string _toCustmerId,
            DateTime _fromDate,
            DateTime _toDate,
            string _documentSate,
            string _dataBaseName)
        {
            var sqlQuery = string.Empty;
            var _fromDateLocal = _fromDate.ToShortDateString().Trim();
            var _toDateLocal = _toDate.ToShortDateString().Trim();

            sqlQuery = $"USE [{_dataBaseName}]" + char.ConvertFromUtf32(13);
            sqlQuery += " " + Char.ConvertFromUtf32(13);

            sqlQuery += "Declare" + Char.ConvertFromUtf32(13);
            sqlQuery += " @rifEmpresa nVarchar(20), " + Char.ConvertFromUtf32(13);
            sqlQuery += " @codigoEmpresa nVarChar(10), " + Char.ConvertFromUtf32(13);
            sqlQuery += " @nombreEmpresa nVarChar(100) " + Char.ConvertFromUtf32(13);
            sqlQuery += " " + Char.ConvertFromUtf32(13);
            sqlQuery += "Set @rifEmpresa = (SELECT TEMP_CHAR1 FROM PAR_EMP) " + Char.ConvertFromUtf32(13);
            sqlQuery += "Set @codigoEmpresa = (SELECT TEMP_CHAR2 FROM PAR_EMP) " + Char.ConvertFromUtf32(13);
            sqlQuery += "Set @nombreEmpresa = (SELECT TEMP_CHAR3 + '' + TEMP_CHAR4 FROM PAR_EMP) " + Char.ConvertFromUtf32(13);
            sqlQuery += " " + Char.ConvertFromUtf32(13);
            sqlQuery += "SELECT @codigoEmpresa AS cCodEmpresa, " + Char.ConvertFromUtf32(13);
            sqlQuery += "@rifEmpresa AS cRifEmpresa, " + Char.ConvertFromUtf32(13);
            sqlQuery += "@nombreEmpresa AS cNomEmpresa, " + Char.ConvertFromUtf32(13);
            sqlQuery += "A.TIPO_DOC, A.NRO_DOC, A.FEC_EMIS, A.FEC_VENC, " + Char.ConvertFromUtf32(13);

            sqlQuery += "CASE " + Char.ConvertFromUtf32(13);
            sqlQuery += "    WHEN DATEDIFF(DD, A.FEC_VENC, GETDATE()) < 0 THEN " + Char.ConvertFromUtf32(13);
            sqlQuery += "        (DATEDIFF(DD, A.FEC_VENC, GETDATE()) * -1) " + Char.ConvertFromUtf32(13);
            sqlQuery += "    WHEN DATEDIFF(DD, A.FEC_VENC, GETDATE()) = 0 THEN " + Char.ConvertFromUtf32(13);
            sqlQuery += "        DATEDIFF(DD, A.FEC_VENC, GETDATE()) " + Char.ConvertFromUtf32(13);
            sqlQuery += "    ELSE " + Char.ConvertFromUtf32(13);
            sqlQuery += "        0 " + Char.ConvertFromUtf32(13);
            sqlQuery += "END AS nDiaPorVencer, " + Char.ConvertFromUtf32(13);

            sqlQuery += "CASE " + Char.ConvertFromUtf32(13);
            sqlQuery += "    WHEN DATEDIFF(DD, A.FEC_VENC, GETDATE()) > 0 THEN " + Char.ConvertFromUtf32(13);
            sqlQuery += "        DATEDIFF(DD, A.FEC_VENC, GETDATE()) " + Char.ConvertFromUtf32(13);
            sqlQuery += "    ELSE " + Char.ConvertFromUtf32(13);
            sqlQuery += "        0 " + Char.ConvertFromUtf32(13);
            sqlQuery += "END AS nDiaVencido, A.OBSERVA, A.CO_VEN, B.VEN_DES, " + Char.ConvertFromUtf32(13);

            sqlQuery += "CASE " + Char.ConvertFromUtf32(13);
            sqlQuery += "    WHEN A.TIPO_DOC = 'ADEL' OR A.TIPO_DOC LIKE '%AJN%' THEN " + Char.ConvertFromUtf32(13);
            sqlQuery += "        A.MONTO_NET * -1 " + Char.ConvertFromUtf32(13);
            sqlQuery += "    ELSE " + Char.ConvertFromUtf32(13);
            sqlQuery += "        A.MONTO_NET " + Char.ConvertFromUtf32(13);
            sqlQuery += "    END AS MONTO_NET, " + Char.ConvertFromUtf32(13);

            sqlQuery += "CASE " + Char.ConvertFromUtf32(13);
            sqlQuery += "    WHEN A.TIPO_DOC = 'ADEL' OR A.TIPO_DOC LIKE '%AJN%' THEN " + Char.ConvertFromUtf32(13);
            sqlQuery += "        A.SALDO * -1 " + Char.ConvertFromUtf32(13);
            sqlQuery += "    ELSE " + Char.ConvertFromUtf32(13);
            sqlQuery += "        A.SALDO " + Char.ConvertFromUtf32(13);
            sqlQuery += "END AS SALDO, A.CO_CLI, C.CLI_DES, C.RESPONS, C.EMAIL, C.CONTRIBU_E " + Char.ConvertFromUtf32(13);

            sqlQuery += "FROM DOCUM_CC AS A " + Char.ConvertFromUtf32(13);
            sqlQuery += "INNER JOIN VENDEDOR AS B ON A.CO_VEN = B.CO_VEN " + Char.ConvertFromUtf32(13);
            sqlQuery += "INNER JOIN CLIENTES AS C ON A.CO_CLI = C.CO_CLI " + Char.ConvertFromUtf32(13);
            sqlQuery += "WHERE A.SALDO > 0.00 " + Char.ConvertFromUtf32(13);

            if (!string.IsNullOrEmpty(_fromCustomerId) && !string.IsNullOrEmpty(_toCustmerId))
            {
                sqlQuery += $"AND A.CO_CLI BETWEEN '{_fromCustomerId.Replace("'", "''")}' AND ";
                sqlQuery += $"'{_toCustmerId.Replace("'", "''")}' " + Char.ConvertFromUtf32(13);
            }

            if (!string.IsNullOrEmpty(_fromDateLocal) && !string.IsNullOrEmpty(_toDateLocal))
            {
                sqlQuery += $"AND A.FEC_EMIS BETWEEN CONVERT(DATETIME, '{_fromDateLocal}', 104) AND ";
                sqlQuery += $"CONVERT(DATETIME, '{_toDateLocal}', 104) " + Char.ConvertFromUtf32(13);
            }

            if (!string.IsNullOrEmpty(_documentSate))
            {
                sqlQuery += $"AND A.TIPO_DOC = '{_fromDateLocal.Replace("'", "''")}' " + Char.ConvertFromUtf32(13);
            }

            sqlQuery += "ORDER BY A.CO_CLI, A.FEC_EMIS " + Char.ConvertFromUtf32(13);

            return sqlQuery;
        }

        private List<CustomerStatementAccount> LoadDataCustomerStatementAccount(
            DataTable _dataTableCustomerStatementAccount)
        {
            var listCustomerStatementAccount = new List<CustomerStatementAccount>();

            foreach (DataRow row in _dataTableCustomerStatementAccount.Rows)
            {
                listCustomerStatementAccount.Add(new CustomerStatementAccount
                {
                    Balance = decimal.Parse(row["Saldo"].ToString().Trim()),
                    CompanyDescription = row["cNomEmpresa"].ToString().Trim(),
                    CompanyId = row["cCodEmpresa"].ToString().Trim(),
                    CompanyRif = row["cRifEmpresa"].ToString().Trim(),
                    ContactPerson = row["RESPONS"].ToString().Trim(),
                    CustomerDescription = row["CLI_DES"].ToString().Trim(),
                    CustomerEmail = row["EMAIL"].ToString().Trim(),
                    CustomerId = row["CO_CLI"].ToString().Trim(),
                    DocumentDaysToCome = int.Parse(row["nDiaPorVencer"].ToString().Trim()),
                    DocumentDescription = row["OBSERVA"].ToString().Trim(),
                    DocumentEmissionDate = DateTime.Parse(row["FEC_EMIS"].ToString().Trim()),
                    DocumentExpirationDate = DateTime.Parse(row["FEC_VENC"].ToString().Trim()),
                    DocumentExpiredDate = int.Parse(row["nDiaVencido"].ToString().Trim()),
                    DocumentId = int.Parse(row["NRO_DOC"].ToString().Trim()),
                    DocumentType = row["TIPO_DOC"].ToString().Trim(),
                    IsTaxPayer = bool.Parse(row["CONTRIBU_E"].ToString().Trim()),
                    NetAmount = decimal.Parse(row["MONTO_NET"].ToString().Trim()),
                    SellerDescription = row["VEN_DES"].ToString().Trim(),
                    SellerId = row["CO_VEN"].ToString().Trim(),
                });
            }

            return listCustomerStatementAccount;
        }

        #endregion Methods

        //// GET: api/Tests/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Tests
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Tests/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Tests/5
        //public void Delete(int id)
        //{
        //}
    }
}