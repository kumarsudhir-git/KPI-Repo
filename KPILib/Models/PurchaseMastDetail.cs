using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class PurchaseMastDetail
    {
        [Key]
        public int PurchaseDetailsID { get; set; }
        public int PurchaseID { get; set; }
        public int RawMaterialID { get; set; }
        public string RawMaterialName { get; set; }
        public int Qty { get; set; }
        public string PurchaseDetailsInstructions { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "PO Date")]
        public System.DateTime PurchaseDate { get; set; }

        [Display(Name = "Company / Location")]
        public string CompanyLocation { get; set; }
        public string PurchaseMasterInstructions { get; set; }

        [Editable(allowEdit: false)]
        public string Status { get; set; }

        public string User { get; set; }

    }


    public class PurchaseMastDetailResponse
    {
        public PurchaseMastDetail data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseMastDetailResponse()
        {
            this.data = new PurchaseMastDetail();
            this.Response = new ResponseObj();
        }
    }

    public class PurchaseMastDetailsResponse
    {
        public List<PurchaseMastDetail> data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseMastDetailsResponse()
        {
            this.data = new List<PurchaseMastDetail>();
            this.Response = new ResponseObj();
        }
    }

}
