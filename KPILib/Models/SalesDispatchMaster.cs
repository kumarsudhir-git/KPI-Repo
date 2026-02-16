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
        public int SalesDispatchID { get; set; }
        public int SalesDetailsID { get; set; }
        public int ProductID { get; set; }
        public int DispatchQty { get; set; }
        public int UserID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? DispatchDate { get; set; }
        public string DispatchNotes { get; set; }
    }

    public class SalesDispatchTransporterDetailsResponse
    {
        public SalesDispatchTransporterDetailsMaster transporterDetailsMasterObj { get; set; }
        public List<SalesDispatchTransporterDetailsMaster> data { get; set; }
        public ResponseObj responseObj { get; set; }

        public SalesDispatchTransporterDetailsResponse()
        {
            this.transporterDetailsMasterObj = new SalesDispatchTransporterDetailsMaster();
            this.data = new List<SalesDispatchTransporterDetailsMaster>();
            this.responseObj = new ResponseObj();
        }
    }

    public class SalesDispatchTransporterDetailsMaster 
    {
        public SalesDispatchTransporterDetailsMaster()
        {
             this.SalesDetailListObj = new List<SalesDetails>();
        }
        public int SDTRDId { get; set; }
        public int SalesDetailsID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Transporter { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal? TransportationCharge { get; set; }
        public DateTime? DispatchDate { get; set; }
        public string DocketNo { get; set; }
        public string DispatchStatus { get; set; }
        public string DocketPhotoPath { get; set; }
        public string BillNo { get; set; }
        public string PackedBy { get; set; }
        public bool SmsSentFlag { get; set; } = false;
        public int? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public SalesMaster SalesMasterObj { get; set; }
        public List<SalesDetails> SalesDetailListObj { get; set; }
    }
}
