namespace Lands.BackEnd.Models
{
    using Lands.Domain;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public class UserView : User
    {
        //  CEHJ - propiedad que permite capturar la imagen del BackEnd
        [Display(Name = "Image")]
        public HttpPostedFileBase ImageFile { get; set; }

    }
}