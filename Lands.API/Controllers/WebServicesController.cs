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

        private ServicesVzLaCantv servicesVzLaCantv;
        private ServicesVzLaCne servicesVzLaCne;

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
                restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader(
                    "cache-control",
                    "no-cache");
                restRequest.AddHeader(
                    "content-type",
                    "application/x-www-form-urlencoded");
                restRequest.AddParameter(
                    "application/x-www-form-urlencoded",
                    value,
                    ParameterType.RequestBody);
                iRestResponse = restClient.Execute(restRequest);
                servicesVzLaCantv = new ServicesVzLaCantv();

                if (iRestResponse != null ||
                    !string.IsNullOrEmpty(iRestResponse.Content))
                {
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(iRestResponse.Content);

                    listDataError = htmlDocument.DocumentNode.Descendants("table")
                        .Where(node => node.GetAttributeValue("width", "")
                        .Equals("350")).ToList();

                    errorMessage = listDataError
                        .Where(lde => lde.InnerText.Trim().Contains("Numero invalido"))
                        .FirstOrDefault();

                    listDataA = htmlDocument.DocumentNode.Descendants("body")
                        .Where(node => node.GetAttributeValue("table", "")
                        .Equals("")).ToList();

                    lcAux001 = string.Empty;
                    lnAux001 = 0;

                    if (listDataA.Count > 0)
                    {
                        foreach (var listItemA in listDataA)
                        {
                            if (lnAux001 == 1)
                            {
                                listDataB = listItemA.Descendants("font")
                                    .Where(node => node.GetAttributeValue("face", "")
                                    .Equals("Verdana, Arial, Helvetica, sans-serif")).ToList();

                                if (listDataB.Count > 0)
                                {
                                    lnAux001 = 0;

                                    foreach (var listItemB in listDataB)
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
                url = new Uri($"http://www.cne.gob.ve/web/registro_electoral/ce.php{value}");
                restClient = new RestClient(url);
                restRequest = new RestRequest(Method.POST);
                restRequest.AddHeader(
                    "cache-control",
                    "no-cache");
                iRestResponse = restClient.Execute(restRequest);
                servicesVzLaCne = new ServicesVzLaCne();

                if (iRestResponse != null &&
                    !string.IsNullOrEmpty(iRestResponse.Content))
                {
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(iRestResponse.Content);

                    //  Get data primary
                    listDataA = htmlDocument.DocumentNode.Descendants("table")
                        .Where(node => node.GetAttributeValue("cellpadding", "")
                        .Equals("2")).ToList();

                    //  Get data secundary
                    listDataB = htmlDocument.DocumentNode.Descendants("td")
                        .Where(node => node.GetAttributeValue("align", "")
                        .Equals("center")).ToList();

                    if (listDataB.Count > 0)
                    {
                        foreach (var listItemB in listDataB)
                        {
                            listDataC = listItemB.Descendants("b")
                                .ToList();
                            if (listDataC.Count > 0 && 
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

                    if (listDataA.Count > 0)
                    {
                        //foreach (var lustItemA in listDataA)
                        //{
                        //if(lnAux001 == 0)
                        //{
                        listDataC = listDataA[0].Descendants("td")
                            .Where(node => node.GetAttributeValue("align", "")
                            .Equals("left")).ToList();

                        if (listDataC.Count > 0)
                        {
                            lnAux001 = 0;

                            foreach (var listItemC in listDataC)
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
                        //}

                        //    lnAux001++;
                        //}
                    }
                }

                ModelState.AddModelError(string.Empty, "No data found...!!!");
                servicesVzLaCne.Error = true;
                servicesVzLaCne.Descripcion = "No data found...!!!";
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