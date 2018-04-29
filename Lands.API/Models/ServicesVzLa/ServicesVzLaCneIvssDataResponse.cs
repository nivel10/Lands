namespace Lands.API.Models.ServicesVzLa
{
    using System;
    using System.Collections.Generic;

    public class ServicesVzLaCneIvssDataResponse
    {
        //[Key]
        public int CneIvssDataId { get; set; }

        //[Display(Name = "User")]
        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[Range(1, 1000, ErrorMessage = "The field {0} only can contais value between {1} and {2}...!!!")]
        //[Index("CneIvssData_UserId_NationalityId_IdentificationCard_Index", IsUnique = true, Order = 1)]
        //public int UserId { get; set; }

        //[Display(Name = "Nationality")]
        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[Range(1, 1000, ErrorMessage = "The field {0} only can contais value between {1} and {2}...!!!")]
        //[Index("CneIvssData_UserId_NationalityId_IdentificationCard_Index", IsUnique = true, Order = 2)]
        public int NationalityId { get; set; }

        //[Display(Name = "Identification Card")]
        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[MaxLength(8, ErrorMessage = "The field {0} only can contains a maximun of {1} characters lenght...!!!")]
        //[RegularExpression(@"[0-9]{1,9}(\.[0-9]{0,2})?$", ErrorMessage = "The field {0} only can contains value numerics...!!!")]
        //[Index("CneIvssData_UserId_NationalityId_IdentificationCard_Index", IsUnique = true, Order = 3)]
        public string IdentificationCard { get; set; }

        //[Required(ErrorMessage = "The field {0} is required...!!!")]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

        //[Display(Name = "Is CNE...?")]
        public bool IsCne { get; set; }

        //[Display(Name = "Is IVSS...?")]
        public bool IsIvss { get; set; }

        #region Relations Table

        //[JsonIgnore]
        //public virtual User User { get; set; }

        //[JsonIgnore]
        //public virtual Nationality Nationality { get; set; }

        //[JsonIgnore]
        public List<ServicesVzLaNationalityResponse> NationalityDatas { get; set; }

        #endregion Relations Table
    }
}