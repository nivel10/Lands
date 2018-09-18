namespace Lands.API.Models.ServicesVzLa
{
    //  using Newtonsoft.Json;

    public class ServicesVzLaCne
    {
        //  [JsonProperty(PropertyName = "error")]
        public bool Error { get; set; }

        //  [JsonProperty(PropertyName = "descripcion")]
        public string Descripcion { get; set; }

        //  [JsonProperty(PropertyName = "modo")]
        public int Modo { get; set; }

        //  [JsonProperty(PropertyName = "cedula")]
        public string Cedula { get; set; }

        //  [JsonProperty(PropertyName = "nombre")]
        public string Nombre { get; set; }

        //  [JsonProperty(PropertyName = "estado")]
        public string Estado { get; set; }

        //  [JsonProperty(PropertyName = "municipio")]
        public string Municipio { get; set; }

        //  [JsonProperty(PropertyName = "parroquia")]
        public string Parroquia { get; set; }

        //  [JsonProperty(PropertyName = "centro")]
        public string Centro { get; set; }

        //  [JsonProperty(PropertyName = "direccion")]
        public string Direccion { get; set; }

        //  [JsonProperty(PropertyName = "servicio")]
        public string Servicio { get; set; }

        public string Mensaje { get; set; }
    }
}