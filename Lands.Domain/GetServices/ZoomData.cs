namespace Lands.Domain.GetServices
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ZoomData
    {
        [Key]
        public int ZoomDataId { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contais value between {1} and {2}...!!!")]
        [Index("ZoomDataId_UserId_Tracking_Index", IsUnique = true, Order = 1)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "The field {0} is required...!!!")]
        [MaxLength(15, ErrorMessage = "The field {0} only can contains a maximun of {1} characters lenght...!!!")]
        //  [RegularExpression(@"[0-9]{1,9}(\.[0-9]{0,2})?$", ErrorMessage = "The field {0} only can contains value numerics...!!!")]
        [RegularExpression(@"[0-9]{1,15}", ErrorMessage = "The field {0} only can contains value numerics...!!!")]
        [Index("ZoomDataId_UserId_Tracking_Index", IsUnique = true, Order = 2)]
        public string Tracking { get; set; }

        #region Relations Table

        [JsonIgnore]
        public virtual User User { get; set; }

        #endregion Relations Table
    }
}