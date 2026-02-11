using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class SalesDispatchMaster
    {
        [Key]
        [Display(Name = "SO Number")]
        public int SalesID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "SO Date")]
        public System.DateTime SalesDate { get; set; }

        [Display(Name = "Customer / Location")]
        public int CompanyLocationID { get; set; }

        [Display(Name = "Customer / Location")]
        public string CompanyLocation { get; set; }
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
        public List<SalesDispatchItem> LineItems { get; set; }

        public SalesDispatchMaster()
        {
            LineItems = new List<SalesDispatchItem>();
        }
    }

    public class SalesDispatchMasterResponse
    {
        public List<SalesDispatchMaster> data { get; set; }
        public ResponseObj Response { get; set; }

        public SalesDispatchMasterResponse()
        {
            this.data = new List<SalesDispatchMaster>();
            this.Response = new ResponseObj();
        }
    }

    public class SalesDispatchDetailMasterResponse
    {
        public SalesDispatchDetailMaster salesDispatchDetailObj { get; set; }
        public List<SalesDispatchDetailMaster> data { get; set; }
        public ResponseObj Response { get; set; }
        public SalesDispatchDetailMasterResponse()
        {
            this.salesDispatchDetailObj = new SalesDispatchDetailMaster();
            this.data = new List<SalesDispatchDetailMaster>();
            this.Response = new ResponseObj();
        }
    }

    public class SalesDispatchDetailMaster
    {
        public SalesDispatchDetailMaster()
        {
            SalesMasterObj = new KPILib.Models.SalesMaster();
            SalesDetailListObj = new List<SalesDetails>();
        }
        public int SalesDispatchID { get; set; }
        public int SalesDetailsID { get; set; }
        public int ProductID { get; set; }
        public int DispatchQty { get; set; }
        public int UserID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? DispatchDate { get; set; }
        public string DispatchNotes { get; set; }
        public decimal? TransportationCharge { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Transporter { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DocketNo { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DispatchStatus { get; set; }
        public string DocketPhotoPath { get; set; }
        public bool? SmsSentFlag { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public KPILib.Models.SalesMaster SalesMasterObj { get; set; }
        //public List<ProductMaster> ProductMasterListObj { get; set; }
        public List<SalesDetails> SalesDetailListObj { get; set; }
    }

    //public class SalesDispatchMastersResponse
    //{
    //    public List<SalesDispatchMaster> data { get; set; }
    //    public ResponseObj Response { get; set; }

    //    public SalesDispatchMastersResponse()
    //    {
    //        this.data = new List<SalesDispatchMaster>();
    //        this.Response = new ResponseObj();
    //    }
    //}

}
