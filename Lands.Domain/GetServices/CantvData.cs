namespace Lands.Domain.GetServices
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CantvData
    {
        [Key]
        public int CantvDataId { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contais value between {1} and {2}...!!!")]
        [Index("CantvData_UserId_CodePhone_NuberPhone_Index", IsUnique = true, Order = 1)]
        public int UserId { get; set; }

        [Display(Name = "Code")]
        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [MaxLength(3, ErrorMessage = "The field {0} only can contains a maximun of {1} characters lenght...!!!")]
        [RegularExpression(@"[0-9]{1,9}(\.[0-9]{0,2})?$", ErrorMessage = "The field {0} only can contains value numerics...!!!")]
        [Index("CantvData_UserId_CodePhone_NuberPhone_Index", IsUnique = true, Order = 2)]
        public string CodePhone { get; set; }

        [Display(Name = "Number")]
        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [MaxLength(7, ErrorMessage = "The field {0} only can contains a maximun of {1} characters lenght...!!!")]
        [RegularExpression(@"[0-9]{1,9}(\.[0-9]{0,2})?$", ErrorMessage = "The field {0} only can contains value numerics...!!!")]
        [Index("CantvData_UserId_CodePhone_NuberPhone_Index", IsUnique = true, Order = 3)]
        public string NuberPhone { get; set; }

        #region Relations Table

        [JsonIgnore]
        public virtual User User { get; set; }

        #endregion Relations Table
    }
}
