using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class CompanyLocation
    {
        [Key]
        public int CompanyLocationID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} should be minimum 3 characters and a maximum of 50 characters")]
        [DataType(DataType.Text)]
        [Display(Name = "Location")]
        public string LocationName { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string State { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [Url]
        public string Website { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string PAN { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string GSTN { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Notes { get; set; }
        //[Required(ErrorMessage = "{0} is required")]
        //public string CompanyTypeIDs { get; set; }
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

        [Display(Name = "Type of Company")]
        public string TypeOfCompany { get; set; }

        public List<TypeOfCompany> CompanyTypes { get; set; }
    }

    public class CompanyLocationResponse
    {
        public Company comp { get; set; }
        public CompanyLocation data { get; set; }
        public ResponseObj Response { get; set; }

        public CompanyLocationResponse()
        {
            this.data = new CompanyLocation();
            this.Response = new ResponseObj();
        }
    }

    public class CompanyLocationsResponse
    {
        public Company comp { get; set; }
        public List<CompanyLocation> data { get; set; }
        public CompanyLocation location { get; set; }
        public ResponseObj Response { get; set; }

        public CompanyLocationsResponse()
        {
            this.comp = new Company();
            this.data = new List<CompanyLocation>();
            this.location = new CompanyLocation();
            this.Response = new ResponseObj();
        }
    }
}
