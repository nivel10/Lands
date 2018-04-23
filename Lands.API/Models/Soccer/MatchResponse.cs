namespace Lands.API.Models.Soccer
{
    using Lands.Domain.Soccer;
    using System;
    using System.Collections.Generic;

    //  Se crea este modelo para poder consultar los Match y eliminar las propiedades virtuales en el API

    public class MatchResponse
    {
        //[Key]
        public int MatchId { get; set; }

        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        public int GroupId { get; set; }

        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        //[Display(Name = "Local Team")]
        public int LocalId { get; set; }

        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        //[Display(Name = "Visitor Team")]
        public int VisitorId { get; set; }

        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        public int StatusMatchId { get; set; }

        //[Display(Name = "Local Goals")]
        public int? LocalGoals { get; set; }

        //[Display(Name = "Visitor Goals")]
        public int? VisitorGoals { get; set; }

        //[DataType(DataType.DateTime)]
        //[Display(Name = "Date Time")]
        public DateTime DateTime { get; set; }

        #region Relations Table

        public Group Group { get; set; }

        public Team Local { get; set; }

        public Team Visitor { get; set; }

        public StatusMatch StatusMatch { get; set; }

        public List<Prediction> Predictions { get; set; }

        #endregion
    }
}