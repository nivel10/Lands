namespace Lands.API.Controllers
{
    using HtmlAgilityPack;
    using Lands.API.Helpers;
    using Lands.API.Models.ServicesVzLa;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    [Authorize]
    [RoutePrefix("API/WebServices")]
    public class WebServicesController : ApiController
    {
        #region Attributes

        private Uri url;
        private string value;
        private RestClient restClient;
        private RestRequest restRequest;
        private HtmlDocument htmlDocument;
        private IRestResponse iRestResponse;
        private List<HtmlNode> listDataA;
        private List<HtmlNode> listDataB;
        private List<HtmlNode> listDataC;
        private List<HtmlNode> listDataError;
        private HtmlNode errorMessage;
        private string lcAux001;
        private int lnAux001;

        private string simboloVariable;
        private string fechaEmision;
        private string fechaVencimiento;
        private string importeEnergia;
        private string importeAseo;
        private string totalFactura;

        private ServicesVzLaCantv servicesVzLaCantv;
        private ServicesVzLaCne servicesVzLaCne;
        private ServicesVzLaCorpoElect servicesVzLaCorpoElect;
        private List<ServicesVzLaCorpoElectDetails> servicesVzLaCorpoElectDetails;

        #endregion Attributes

        #region Methods

        [Route("GetCantvData")]
        public IHttpActionResult GetCantvData(
            int _numberCode,
            int _numberPhone)
        {
            try
            {
                url = new Uri(MethodsHelper.GetUrlCantv());
                value = $"sarea={_numberCode}&stelefono={_numberPhone}";
                restClient = new RestClient(url);
                this.restRequest = new RestRequest(Method.POST);
                this.restRequest.AddHeader(
                    "cache-control",
                    "no-cache");
                this.restRequest.AddHeader(
                    "content-type",
                    "application/x-www-form-urlencoded");
                this.restRequest.AddParameter(
                    "application/x-www-form-urlencoded",
                    value,
                    ParameterType.RequestBody);
                this.iRestResponse = restClient.Execute(this.restRequest);
                servicesVzLaCantv = new ServicesVzLaCantv();

                if (this.iRestResponse != null ||
                    !string.IsNullOrEmpty(this.iRestResponse.Content))
                {
                    this.htmlDocument = new HtmlDocument();
                    this.htmlDocument.LoadHtml(this.iRestResponse.Content);

                    listDataError = this.htmlDocument.DocumentNode.Descendants("table")
                        .Where(node => node.GetAttributeValue("width", "")
                        .Equals("350")).ToList();

                    errorMessage = listDataError
                        .Where(lde => lde.InnerText.Trim().Contains("Numero invalido"))
                        .FirstOrDefault();

                    this.listDataA = this.htmlDocument.DocumentNode.Descendants("body")
                        .Where(node => node.GetAttributeValue("table", "")
                        .Equals("")).ToList();

                    lcAux001 = string.Empty;
                    lnAux001 = 0;

                    if (this.listDataA.Count > 0)
                    {
                        foreach (var listItemA in this.listDataA)
                        {
                            if (lnAux001 == 1)
                            {
                                this.listDataB = listItemA.Descendants("font")
                                    .Where(node => node.GetAttributeValue("face", "")
                                    .Equals("Verdana, Arial, Helvetica, sans-serif")).ToList();

                                if (this.listDataB.Count > 0)
                                {
                                    lnAux001 = 0;

                                    foreach (var listItemB in this.listDataB)
                                    {
                                        if ((lnAux001 % 2) == 0)
                                        {
                                            lcAux001 = listItemB.InnerText.Trim();
                                        }
                                        else
                                        {
                                            switch (lcAux001)
                                            {
                                                case "Saldo actual Bs.":
                                                    servicesVzLaCantv.SaldoActual = listItemB.InnerText.Trim();
                                                    break;
                                                case "Fecha de &uacute;ltima facturaci&oacute;n:":
                                                    servicesVzLaCantv.UltimaFacturacion = listItemB.InnerText.Trim();
                                                    break;
                                                case "Fecha corte:":
                                                    servicesVzLaCantv.FechaCorte = listItemB.InnerText.Trim();
                                                    break;
                                                case "Fecha de vencimiento:":
                                                    servicesVzLaCantv.FechaVencimiento = listItemB.InnerText.Trim();
                                                    break;
                                                case "Saldo vencido:":
                                                    servicesVzLaCantv.SaldoVencido = listItemB.InnerText.Trim();
                                                    break;
                                                case "Monto del &uacute;ltimo pago realizado:":
                                                    servicesVzLaCantv.UltimoPago = listItemB.InnerText.Trim();
                                                    break;
                                            }
                                        }
                                        lnAux001++;
                                    }

                                    if (errorMessage != null)
                                    {
                                        servicesVzLaCantv.Error = true;
                                        servicesVzLaCantv.Descripcion = "Invalid number...!!!";
                                    }

                                    return Ok(servicesVzLaCantv);
                                }
                            }
                            lnAux001++;
                        }
                    }
                }

                //  ModelState.AddModelError(string.Empty, "Not data found...!!!");
                //  return BadRequest(ModelState);

                servicesVzLaCantv.Error = true;
                servicesVzLaCantv.Descripcion = "Not data found...!!!";
                return Ok(servicesVzLaCantv);
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(string.Empty, ex.Message);
                //return BadRequest(ModelState);
                return Ok(new ServicesVzLaCantv
                {
                    Descripcion = ex.Message,
                    Error = true,
                });
            }
        }

        [Route("GetCneData")]
        public IHttpActionResult GetCneData(
            string _nationalityUser,
            int _numberUser
            )
        {
            try
            {
                value = $"?nacionalidad={_nationalityUser}&cedula={_numberUser}";
                //  url = new Uri($"http://www.cne.gob.ve/web/registro_electoral/ce.php{value}");
                this.url = new Uri($"{MethodsHelper.GetUrlCne()}{value}");
                this.restClient = new RestClient(url);
                this.restRequest = new RestRequest(Method.POST);
                this.restRequest.AddHeader(
                    "cache-control",
                    "no-cache");
                this.iRestResponse = restClient.Execute(this.restRequest);
                servicesVzLaCne = new ServicesVzLaCne();

                if (this.iRestResponse != null &&
                    !string.IsNullOrEmpty(this.iRestResponse.Content))
                {
                    this.htmlDocument = new HtmlDocument();
                    this.htmlDocument.LoadHtml(this.iRestResponse.Content);

                    //  Get data primary
                    this.listDataA = this.htmlDocument.DocumentNode.Descendants("table")
                        .Where(node => node.GetAttributeValue("cellpadding", "")
                        .Equals("2")).ToList();

                    //  Get data secundary
                    this.listDataB = this.htmlDocument.DocumentNode.Descendants("td")
                        .Where(node => node.GetAttributeValue("align", "")
                        .Equals("center")).ToList();

                    if (this.listDataB.Count > 0)
                    {
                        foreach (var listItemB in this.listDataB)
                        {
                            this.listDataC = listItemB.Descendants("b")
                                .ToList();
                            if (this.listDataC.Count > 0 &&
                                !listItemB.InnerText.Contains("DATOS DEL ELECTOR") &&
                                !listItemB.InnerText.Contains("Conoce los Miembros de Mesa de tu Centro de Votación."))
                            {
                                servicesVzLaCne.Mensaje =
                                    string.Format(
                                        string.IsNullOrEmpty(servicesVzLaCne.Mensaje) ? "{0}{1}" : "{0}; {1}",
                                        servicesVzLaCne.Mensaje,
                                        listItemB.InnerText.Trim());
                            }
                        }
                    }

                    lcAux001 = string.Empty;
                    lnAux001 = 0;

                    if (this.listDataA.Count > 0)
                    {
                        //foreach (var lustItemA in this.listDataA)
                        //{
                        //if(lnAux001 == 0)
                        //{
                        this.listDataC = this.listDataA[0].Descendants("td")
                            .Where(node => node.GetAttributeValue("align", "")
                            .Equals("left")).ToList();

                        if (this.listDataC.Count > 0)
                        {
                            lnAux001 = 0;

                            foreach (var listItemC in this.listDataC)
                            {
                                if ((lnAux001 % 2) == 0)
                                {
                                    lcAux001 = listItemC.InnerText.Trim();
                                }
                                else
                                {
                                    switch (lcAux001)
                                    {
                                        case "Cédula:":
                                            servicesVzLaCne.Cedula = listItemC.InnerText.Trim();
                                            break;

                                        case "Nombre:":
                                            servicesVzLaCne.Nombre = listItemC.InnerText.Trim();
                                            break;

                                        case "Estado:":
                                            servicesVzLaCne.Estado = listItemC.InnerText.Trim();
                                            break;

                                        case "Municipio:":
                                            servicesVzLaCne.Municipio = listItemC.InnerText.Trim();
                                            break;

                                        case "Parroquia:":
                                            servicesVzLaCne.Parroquia = listItemC.InnerText.Trim();
                                            break;

                                        case "Centro:":
                                            servicesVzLaCne.Centro = listItemC.InnerText.Trim();
                                            break;

                                        case "Dirección:":
                                            servicesVzLaCne.Direccion = listItemC.InnerText.Trim();
                                            break;
                                    }
                                }
                                lnAux001++;
                            }
                            return Ok(servicesVzLaCne);
                        }
                        else
                        {
                            this.listDataB = this.listDataA[0].Descendants("td")
                                .Where(node => node.GetAttributeValue("colspan", "")
                                .Equals("3")).ToList();

                            foreach (var lisItemDataB in this.listDataB)
                            {
                                if (!string.IsNullOrEmpty(lisItemDataB.InnerText))
                                {
                                    servicesVzLaCne.Descripcion += lisItemDataB.InnerText + Char.ConvertFromUtf32(13);
                                }
                            }
                            servicesVzLaCne.Error = true;
                            ModelState.AddModelError(string.Empty, servicesVzLaCne.Descripcion);
                            return Ok(servicesVzLaCne);
                        }
                        //}

                        //    lnAux001++;
                        //}
                    }
                    else
                    {
                        //  Get error consult
                        this.listDataA = this.htmlDocument.DocumentNode.Descendants("table")
                            .Where(node => node.GetAttributeValue("align", "")
                            .Equals("center")).ToList();

                        foreach (var listItemA in this.listDataA)
                        {
                            this.listDataB = listItemA.Descendants("td")
                                .ToList();

                            if (this.listDataB.Count > 0)
                            {
                                servicesVzLaCne.Descripcion += this.listDataB[0].InnerText + Char.ConvertFromUtf32(13);
                            }

                            //  lcAux001 = listItemA.InnerText;
                        }
                        servicesVzLaCne.Error = true;
                        ModelState.AddModelError(string.Empty, servicesVzLaCne.Descripcion);
                        return Ok(servicesVzLaCne);
                    }
                }

                servicesVzLaCne.Error = true;
                if (this.iRestResponse.ErrorException == null)
                {
                    ModelState.AddModelError(string.Empty, "No data found...!!!");
                    servicesVzLaCne.Descripcion = "No data found...!!!";
                }
                else
                {
                    //  Get error request
                    servicesVzLaCne.Descripcion = this.iRestResponse.ErrorException.Message + Char.ConvertFromUtf32(13);
                    if (this.iRestResponse.ErrorException.InnerException != null)
                    {
                        servicesVzLaCne.Descripcion += this.iRestResponse.ErrorException.InnerException.Message;
                    }
                }
                return Ok(servicesVzLaCne);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                servicesVzLaCne.Error = true;
                servicesVzLaCne.Descripcion = ex.Message;
                return Ok(servicesVzLaCne);
            }
        }

        [Route("GetCorpoElectData")]
        public IHttpActionResult GetCorpoElec(string _userNic)
        {
            try
            {
                value = $"?nic={_userNic}";
                url = new Uri($"{MethodsHelper.GetUlrCorpoElect()}{value}");
                restClient = new RestClient(url);
                this.restRequest = new RestRequest(Method.POST);
                this.restRequest.AddHeader(
                    "cache-control",
                    "no-cache");
                this.iRestResponse = restClient.Execute(this.restRequest);

                servicesVzLaCorpoElect = new ServicesVzLaCorpoElect();

                if (this.iRestResponse.ErrorException == null)
                {
                    if (this.iRestResponse != null &&
                        !string.IsNullOrEmpty(this.iRestResponse.Content))
                    {
                        this.htmlDocument = new HtmlDocument();
                        this.htmlDocument.LoadHtml(this.iRestResponse.Content);

                        //  Get the header data
                        this.listDataA = this.htmlDocument.DocumentNode.Descendants("td")
                            .ToList();

                        foreach (var listItemDataA in this.listDataA)
                        {
                            if (listItemDataA.InnerText.Contains("NIC:"))
                            {
                                this.listDataB = listItemDataA.DescendantsAndSelf("input")
                                .Where(node => !string.IsNullOrWhiteSpace(node.Id) &&
                                       node.Attributes["value"] != null)
                                .ToList();

                                servicesVzLaCorpoElect.NicUsuario = this.listDataB[0].Attributes["value"].Value;
                            }

                            if (listItemDataA.InnerText.Contains("USUARIO:"))
                            {
                                this.listDataB = listItemDataA.DescendantsAndSelf("input")
                                .Where(node => !string.IsNullOrWhiteSpace(node.Id) &&
                                       node.Attributes["value"] != null)
                                .ToList();
                                servicesVzLaCorpoElect.NombreUsuario = this.listDataB[0].Attributes["value"].Value;
                            }

                            if (listItemDataA.InnerText.Contains("DEUDA PENDIENTE DEL USUARIO"))
                            {
                                this.listDataB = listItemDataA.DescendantsAndSelf("input")
                                    .Where(node => !string.IsNullOrWhiteSpace(node.Id) &&
                                           node.Attributes["value"] != null)
                                    .ToList();
                                servicesVzLaCorpoElect.DeudaPendienteUsuario = this.listDataB[0].Attributes["value"].Value;
                            }

                            if (listItemDataA.InnerText.Contains("Deuda Vencida"))
                            {
                                this.listDataB = listItemDataA.DescendantsAndSelf("input")
                                    .Where(node => !string.IsNullOrWhiteSpace(node.Id) &&
                                           node.Attributes["value"] != null)
                                           .ToList();
                                servicesVzLaCorpoElect.DeudaVencidaUsuario = this.listDataB[0].Attributes["value"].Value;
                            }
                        }

                        //  Get the details data
                        this.listDataA = this.htmlDocument.DocumentNode.Descendants("td")
                            .Where(node => node.GetAttributeValue("style", "")
                            .Equals("HEIGHT: 71px"))
                            .ToList();

                        if (this.listDataA.Count > 0)
                        {
                            this.InitialFieldCorpoElectDetail();
                            lcAux001 = string.Empty;
                            lnAux001 = 0;
                            servicesVzLaCorpoElectDetails = new List<ServicesVzLaCorpoElectDetails>();

                            this.listDataB = this.listDataA[0].Descendants("td")
                                .ToList();
                            foreach (var listItemDataB in this.listDataB)
                            {
                                if (listItemDataB.InnerText.ToString() == "Total Factura")
                                {
                                    lcAux001 = listItemDataB.InnerText.ToString();
                                }

                                if (lcAux001 == listItemDataB.InnerText.ToString())
                                {
                                }
                                else
                                {
                                    if (lcAux001 == "Total Factura")
                                    {
                                        switch (lnAux001)
                                        {
                                            case 0:
                                                this.simboloVariable = listItemDataB.InnerText.Trim();
                                                break;
                                            case 1:
                                                this.fechaEmision = listItemDataB.InnerText.Trim();
                                                break;
                                            case 2:
                                                this.fechaVencimiento = listItemDataB.InnerText.Trim();
                                                break;
                                            case 3:
                                                this.importeEnergia = listItemDataB.InnerText.Trim();
                                                break;
                                            case 4:
                                                this.importeAseo = listItemDataB.InnerText.Trim();
                                                break;
                                            case 5:
                                                this.totalFactura = listItemDataB.InnerText.Trim();

                                                this.servicesVzLaCorpoElectDetails.Add(
                                                    new ServicesVzLaCorpoElectDetails
                                                    {
                                                        FechaEmision = this.fechaEmision,
                                                        FechaVencimiento = this.fechaVencimiento,
                                                        ImporteAseo = this.importeAseo,
                                                        ImporteEnergia = this.importeEnergia,
                                                        SimboloVariable = this.simboloVariable,
                                                        TotalAseo = this.importeAseo,
                                                    });
                                                lnAux001 = -1;
                                                this.InitialFieldCorpoElectDetail();
                                                break;
                                        }

                                        lnAux001++;
                                    }
                                }
                            }
                            servicesVzLaCorpoElect.Details = this.servicesVzLaCorpoElectDetails;
                        }
                    }
                    else
                    {
                        //  Ojo aqui
                    }
                }
                else
                {
                    //  Ojo aqui
                }

                return Ok(servicesVzLaCorpoElect);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                servicesVzLaCorpoElect.Error = true;
                servicesVzLaCorpoElect.Descripcion = ex.Message;
                return Ok(servicesVzLaCorpoElect);
            }
        }

        private void InitialFieldCorpoElectDetail()
        {
            this.fechaEmision = string.Empty;
            this.fechaVencimiento = string.Empty;
            this.importeAseo = string.Empty;
            this.importeEnergia = string.Empty;
            this.simboloVariable = string.Empty;
            this.importeAseo = string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        #endregion Methods
    }
}