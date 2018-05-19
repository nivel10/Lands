namespace Lands.API.Models.ServicesVzLa
{
    using Lands.Domain;
    using System.ComponentModel.DataAnnotations;

    public class ServicesVzLaUserEdit : User
    {
        [Required(ErrorMessage = "The field {0} is requiered...!!")]
        [MaxLength(100, ErrorMessage = "The field {0} only can contains a maximum of {1} characters lenght...!!!")]
        [DataType(DataType.EmailAddress)]
        public string NewEmail
        {
            get;
            set;
        }

        [Required(ErrorMessage = "The field {0} is requiered...!!")]
        [DataType(DataType.Password)]
        public string NewPassword
        {
            get;
            set;
        }
    }
}