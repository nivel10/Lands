namespace Lands.API.Models.ServicesVzLa
{
    public class ServicesVzLaNationalityResponse
    {
        //[Key]
        public int NationalityId { get; set; }

        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[MaxLength(1, ErrorMessage = "The field {0} only can contains a maximun of {1} characters lenght...!!!")]
        //[RegularExpression(@"[a-zA-ZñÑ\s]", ErrorMessage = "The field {0} only can contains values alphabetic...!!!")]
        //[Index("Nationality_Abbreviation_Name_Index", IsUnique = true, Order = 1)]
        public string Abbreviation { get; set; }

        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[MaxLength(15, ErrorMessage = "The field {0} only can contains a maximun of {1} characters lenght...!!!")]
        //[Index("Nationality_Abbreviation_Name_Index", IsUnique = true, Order = 2)]
        public string Name { get; set; }

        //[Display(Name = "Nationality")]
        public string FullNationality
        {
            get
            {
                return string.Format(
                    "{0} - {1}",
                    Abbreviation,
                    Name);
            }
        }

        #region Relations Table

        //[JsonIgnore]
        //public virtual ICollection<CneIvssData> CneIvssData { get; set; }

        #endregion Relations Table
    }
}