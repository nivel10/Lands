namespace Lands.Domain.Soccer
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Match
    {
        [Key]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        [Display(Name = "Local Team")]
        public int LocalId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        [Display(Name = "Visitor Team")]
        public int VisitorId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values beteewn {1} and {2}...!!!")]
        public int StatusMatchId { get; set; }

        [Display(Name = "Local Goals")]
        public int? LocalGoals { get; set; }

        [Display(Name = "Visitor Goals")]
        public int? VisitorGoals { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Time")]
        public DateTime DateTime { get; set; }

        #region Relations Table

        [JsonIgnore] // <== Evita error al serializar 
        public virtual Group Group { get; set; }

        [JsonIgnore] // <== Evita error al serializar 
        public virtual Team Local { get; set; }

        [JsonIgnore] // <== Evita error al serializar 
        public virtual Team Visitor { get; set; }

        [JsonIgnore] // <== Evita error al serializar 
        public virtual StatusMatch StatusMatch { get; set; }

        [JsonIgnore] // <== Evita error al serializar 
        public virtual ICollection<Prediction> Predictions { get; set; }

        #endregion
    }
}