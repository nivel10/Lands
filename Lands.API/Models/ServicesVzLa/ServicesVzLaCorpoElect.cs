namespace Lands.API.Models.ServicesVzLa
{
    using System.Collections.Generic;

    public class ServicesVzLaCorpoElect
    {
        public bool Error { get; set; }

        public string Descripcion { get; set; }

        public string NicUsuario { get; set; }

        public string NombreUsuario { get; set; }

        public string DeudaPendienteUsuario { get; set; }

        public string DeudaVencidaUsuario { get; set; }

        public List<ServicesVzLaCorpoElectDetails> Details  { get; set; }
    }
}