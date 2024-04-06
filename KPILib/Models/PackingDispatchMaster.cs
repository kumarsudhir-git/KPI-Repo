using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class PackingDispatchMaster
    {
        [Key]
        [Display(Name = "SO Number")]
        public int SalesID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "SO Date")]
        public System.DateTime SalesDate { get; set; }

        [Display(Name = "Customer Name")]
        public int CompanyID { get; set; }

        [Display(Name = "Customer Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Customer Location")]
        public int CompanyLocationID { get; set; }

        [Display(Name = "Customer Location")]
        public string CompanyLocation { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        public string Instructions { get; set; }

        [Display(Name = "Status")]
        [Editable(allowEdit: false)]
        public int SalesStatusID { get; set; }

        [Editable(allowEdit: false)]
        public string Status { get; set; }

        public int UserID { get; set; }

        [Display(Name = "User")]
        [Editable(allowEdit: false)]
        public string User { get; set; }

        public SalesDispatchItem Item { get; set; }

        public List<ProductInRack> RackDetails { get; set; }
    }

    public class PackingDispatchMasterResponse
    {
        public PackingDispatchMaster data { get; set; }
        public ResponseObj Response { get; set; }

        public PackingDispatchMasterResponse()
        {
            this.data = new PackingDispatchMaster() { RackDetails = new List<ProductInRack>() };
            this.Response = new ResponseObj();
        }
    }
}
