namespace Lands.Domain.Soccer
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;

    public class Prediction
    {

        [Key]
        public int PredictionId { get; set; }

        [Required(ErrorMessage = "The field {0} os required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values between {1} and {2}...!!!")]
        [Display(Name = "Board")]
        public int BoardId { get; set; }

        [Required(ErrorMessage = "The field {0} os required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values between {1} and {2}...!!!")]
        [Display(Name = "Match")]
        public int MatchId { get; set; }

        [Display(Name = "Local Goals")]
        public int LocalGoals { get; set; }

        [Display(Name = "Visitor Goals")]
        public int VisitorGoals { get; set; }

        [Required(ErrorMessage = "The field {0} os required...!!!")]
        [Range(1, 1000, ErrorMessage = "The field {0} only can contains values between {1} and {2}...!!!")]
        [Display(Name = "User")]
        public int UserId { get; set; }

        public int? Points { get; set; }

        #region Relations Table

        [JsonIgnore]
        public virtual Board Board { get; set; }

        [JsonIgnore]
        public virtual Match Match { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        #endregion Relations Table
    }
}