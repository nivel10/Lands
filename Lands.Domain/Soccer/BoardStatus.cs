namespace Lands.Domain.Soccer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class BoardStatus
    {
        [Key]
        public int BoardStatusId { get; set; }

        [Required(ErrorMessage = "The field {0} is reqeuired...!!!")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contains a maximum of {1} characters length...!!!")]
        [Index("BoardStatus_Name_Index", IsUnique = true)]
        public string Name { get; set; }

        #region Relations Table

        [JsonIgnore]
        public virtual ICollection<Board> Boards { get; set; }

        #endregion
    }
}
