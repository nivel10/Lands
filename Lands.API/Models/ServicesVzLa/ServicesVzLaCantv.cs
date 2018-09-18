namespace Lands.API.Models.ServicesVzLa
{
    //  using Newtonsoft.Json;

    public class ServicesVzLaCantv
    {
        //  [JsonProperty(PropertyName = "error")]
        public bool Error { get; set; }

        //  [JsonProperty(PropertyName = "descripcion")]
        public string Descripcion { get; set; }

        //  [JsonProperty(PropertyName = "saldoactual")]
        public string SaldoActual { get; set; }

        //  [JsonProperty(PropertyName = "ultimafacturacion")]
        public string UltimaFacturacion { get; set; }

        //  [JsonProperty(PropertyName = "fechacorte")]
        public string FechaCorte { get; set; }

        //  [JsonProperty(PropertyName = "fechavencimiento")]
        public string FechaVencimiento { get; set; }

        //  [JsonProperty(PropertyName = "saldovencido")]
        public string SaldoVencido { get; set; }

        //  [JsonProperty(PropertyName = "ultimopago")]
        public string UltimoPago { get; set; }
    }
}