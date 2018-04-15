namespace Lands.Domain.Soccer
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Board
    {
        [Key]
        public int BoardId { get; set; }

        [Required(ErrorMessage = "The field {0} os required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values between {1} and {2}...!!!")]
        [Display(Name = "Board Status")]
        public int BoardStatusId { get; set; }

        [Required(ErrorMessage = "The field {0} os required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values between {1} and {2}...!!!")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        [Display(Name = "Way payed...?")]
        public bool WayPayed { get; set; }

        [NotMapped]
        public byte[] ImageArray { get; set; }

        [Display(Name = "Image")]
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return "noimage";
                }

                return string.Format(
                    "http://chejconsultor.ddns.net:9015/{0}",
                    ImagePath.Substring(1));
            }
        }

        #region Relations Table

        [JsonIgnore]
        public virtual BoardStatus BoardStatus { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Prediction> Predictions { get; set; }

        #endregion Relations Table
    }
}