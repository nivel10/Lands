﻿namespace Lands.Domain.Soccer
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class GroupTeam
    {
        [Key]
        public int GroupTeamId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can values between {1} and {2}...!!!")]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can values between {1} and {2}...!!!")]
        public int TeamId { get; set; }

        //  CHEJ - Relaciones
        [JsonIgnore] // <== Evita error al serializar
        public virtual Group Group { get; set; }

        [JsonIgnore] // <== Evita error al serializar
        public virtual Team Team { get; set; }
    }
}
