namespace Lands.Domain.Soccer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contains a maximun the {1} characters lenght...!!!")]
        [Index("Group_Name_Index", IsUnique = true)]
        public string Name { get; set; }

        //  CHEJ - relaciones virtuales
        [JsonIgnore] // <== Evita error al serializar
        public virtual  ICollection<GroupTeam> GroupTeams { get; set; }
    }
}
