namespace Lands.Domain.Soccer
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class StatusMatch
    {
        [Key]
        public int StatusMatchId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contains a maximum of {1} characters length...!!!")]
        [Index("StatusMatch_Name_Index", IsUnique = true)]
        [Display(Name = "Status")]
        public string Name { get; set; }

        #region Relations Table

        public virtual ICollection<Match> Matches { get; set; }

        #endregion Relations Table
    }
}