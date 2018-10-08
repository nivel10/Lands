namespace Lands.API.Models.Test001
{
    using System;
    using System.Collections.Generic;

    public class Employee
    {
        public string Employee_i { get; set; }

        public string Last_name { get; set; }

        public string Password { get; set; }

        public decimal Prioridad { get; set; }

        public bool Activo { get; set; }

        public string Mapa { get; set; }

        public string User_nodo { get; set; }

        public string Group_id { get; set; }

        public bool Camb_sucu { get; set; }

        public bool Pide_sucu { get; set; }

        public string Sucursal { get; set; }

        public string Empresas { get; set; }

        public List<Empresa> EmpresasSinAcceso { get; set; }

        public List<Empresa> EmpresaConAcceso  { get; set; }

        public string Idioma { get; set; }

        public bool Pcontrol_1 { get; set; }

        public bool Pcontrol_2 { get; set; }

        public int Serie { get; set; }

        public DateTime Fec_ult { get; set; }

        public DateTime Fec_prox { get; set; }

        public int Veces { get; set; }

        public DateTime Fec_Ult_FA { get; set; }

        public string Estado { get; set; }

        public string AdLogin { get; set; }

        public string  Message { get; set; }

        public string Employee_p { get; set; }
    }
}