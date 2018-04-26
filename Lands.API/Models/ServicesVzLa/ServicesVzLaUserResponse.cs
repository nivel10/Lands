namespace Lands.API.Models.ServicesVzLa
{
    using Lands.Domain.Soccer;
    using System.Collections.Generic;

    public class ServicesVzLaUserResponse
    {
        //  [Key]
        public int UserId { get; set; }

        //  [Display(Name = "First Name")]
        //  [Required(ErrorMessage = "The field {0} is requiered.")]
        //  [MaxLength(50, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght...!!!")]
        public string FirstName { get; set; }

        //  [Display(Name = "Last Name")]
        //  [Required(ErrorMessage = "The field {0} is requiered...!!")]
        //  [MaxLength(50, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght...!!!")]
        public string LastName { get; set; }

        //  [Required(ErrorMessage = "The field {0} is requiered...!!")]
        //  [MaxLength(100, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght...!!!")]
        //  [Index("User_Email_Index", IsUnique = true)]
        //  [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        //  [MaxLength(20, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght...!!!")]
        //  [DataType(DataType.PhoneNumber)]
        public string Telephone { get; set; }

        //  [Display(Name = "Image")]
        public string ImagePath { get; set; }

        //  [Display(Name = "User Type")]
        //  [Range(1, 100, ErrorMessage = "The field {0} only can contais value between {1} and {2}...!!!")]
        public int UserTypeId { get; set; }
        
        //  [NotMapped]
        public byte[] ImageArray { get; set; }

        //   [NotMapped]
        public string Password { get; set; }

        //  [Display(Name = "Image")]
        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                {
                    return "noimage";
                }

                return string.Format(
                    "http://chejconsultor.ddns.net:9015{0}",
                    ImagePath.Substring(1));
            }
        }

        //  [Display(Name = "User")]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }

        //  [Display(Name = "App Name")]
        public string AppName { get; set; }

        #region Relations Table

        //  [JsonIgnore]
        //public List<Board> Boards { get; set; }

        ////  [JsonIgnore]
        //public List<Prediction> Predictions { get; set; }

        //  [JsonIgnore]
        public List<ServicesVzLaCantvDataResponse> CantvDatas { get; set; }

        //  [JsonIgnore]
        public List<ServicesVzLaCneIvssDataResponse> CneIvssDatas { get; set; }

        //   [JsonIgnore]
        public List<ServicesVzLaZoomDataResponse> ZoomDatas { get; set; }

        #endregion Relations Table

    }
}