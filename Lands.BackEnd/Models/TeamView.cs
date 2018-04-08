namespace Lands.BackEnd.Models
{
    using Lands.Domain.Soccer;
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public class TeamView : Team
    {
        //  CEHJ - propiedad que permite capturar la imagen del BackEnd

        [Display(Name = "Image")]
        public HttpPostedFileBase ImageFile { get; set; }
    }
}