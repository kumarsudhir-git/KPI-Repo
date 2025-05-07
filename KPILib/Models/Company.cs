using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "{0} should be minimum 3 characters and a maximum of 150 characters")]
        [DataType(DataType.Text)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public string Notes { get; set; }

        [Display(Name = "Discontinued?")]
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

        public int LocationCount { get; set; }
        public int CompanyTypeID { get; set; }
        public string LocationName { get; set; }

        public List<CompanyLocation> Locations { get; set; }
    }

    public class CompanyResponse
    {
        public Company data { get; set; }
        public ResponseObj Response { get; set; }

        public CompanyResponse()
        {
            this.data = new Company();
            this.Response = new ResponseObj();
        }
    }

    public class CompaniesResponse
    {
        public List<Company> data { get; set; }
        public ResponseObj Response { get; set; }

        public CompaniesResponse()
        {
            this.data = new List<Company>();
            this.Response = new ResponseObj();
        }
    }

}
