namespace Lands.Domain.Soccer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contains a maximum of {1} characters length...!!!")]
        [Index("Team_Name_Index", IsUnique = true)]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        #region Relations Table

        [JsonIgnore] // <== Evita error al serializar
        public virtual ICollection<GroupTeam> GroupTeams { get; set; }

        [JsonIgnore] // <== Evita error al serializar
        public virtual ICollection<Match> Locals { get; set; }

        [JsonIgnore] // <== Evita error al serializar
        public virtual ICollection<Match> Visitors { get; set; }

        #endregion Relations Table

    }
}