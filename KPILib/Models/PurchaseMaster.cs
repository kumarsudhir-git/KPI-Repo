using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class PurchaseMaster
    {
        [Key]
        [Display(Name = "PO Number")]
        public int PurchaseID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "PO Date")]
        public System.DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Vendor / Location")]
        public int CompanyLocationID { get; set; }

        [Display(Name = "Vendor / Location")]
        public string CompanyLocation { get; set; }
        public string Instructions { get; set; }

        [Display(Name = "Status")]
        [Editable(allowEdit: false)]
        public int PurchaseStatusID { get; set; }

        [Editable(allowEdit: false)]
        public string Status { get; set; }

        public int UserID { get; set; }

        [Display(Name = "User")]
        [Editable(allowEdit: false)]
        public string User { get; set; }

        //public List<KeyValuePair> Locations { get; set; }

        public List<PurchaseDetails> LineItems { get; set; }
        public List<KeyValuePair> Materials { get; set; }

        public PurchaseMaster()
        {
            PurchaseStatusID = 10;
            //UserID = 1001;
            PurchaseDate = DateTime.Today;
            LineItems = new List<PurchaseDetails>();
            //Locations = new List<KeyValuePair>();
            Materials = new List<KeyValuePair>();
        }
    }

    public class PurchaseMasterResponse
    {
        public PurchaseMaster data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseMasterResponse()
        {
            this.data = new PurchaseMaster();
            this.Response = new ResponseObj();
        }
    }

    public class PurchaseMastersResponse
    {
        public List<PurchaseMaster> data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseMastersResponse()
        {
            this.data = new List<PurchaseMaster>();
            this.Response = new ResponseObj();
        }
    }

}
