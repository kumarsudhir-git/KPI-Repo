using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    #region PurchaseRcvMast
    public class PurchaseRcvMast
    {
        [Key]
        public int PurchaseRcvdID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Rcvd Date")]
        public DateTime RcvdDate { get; set; }

        [Display(Name = "PO Number")]
        public int PurchaseID { get; set; }

        #region PurchaseDetails

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "PO Date")]
        public System.DateTime PurchaseDate { get; set; }

        [Display(Name = "Company / Location")]
        public string CompanyLocation { get; set; }
        public string Instructions { get; set; }

        [Display(Name = "Booked By")]
        [Editable(allowEdit: false)]
        public string User { get; set; }

        #endregion

        [Editable(allowEdit: false)]
        public string Status { get; set; }

        public int RcvdByUserID { get; set; }

        [Display(Name = "Received By")]
        [Editable(allowEdit: false)]
        public string ReceivedByUser { get; set; }

        public string Notes { get; set; }
        public List<PurchaseRcvDet> LineItems { get; set; }
    }

    public class PurchaseRcvMastResponse
    {
        public PurchaseRcvMast data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseRcvMastResponse()
        {
            this.data = new PurchaseRcvMast() { LineItems = new List<PurchaseRcvDet>() };
            this.Response = new ResponseObj();
        }
    }


    public class PurchaseRcvMastsResponse
    {
        public List<PurchaseRcvMast> data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseRcvMastsResponse()
        {
            this.data = new List<PurchaseRcvMast>();
            this.Response = new ResponseObj();
        }
    }

    #endregion


    #region PurchaseRcvDet
    public class PurchaseRcvDet
    {
        [Key]
        public int PurchaseRcvdDetailsID { get; set; }
        public int PurchaseRcvdID { get; set; }
        public int PurchaseDetailsID { get; set; }
        public int RawMatarialID { get; set; }
        public string RawMatarialName { get; set; }

        [Editable(allowEdit: false)]
        public int Qty { get; set; }

        [Editable(allowEdit: false)]
        public int QtyRcvdSoFar { get; set; }

        [Editable(allowEdit: false)]
        public int QtyBalance { get; set; }
        public int QtyRcvdNow { get; set; }
        //public List<PurchaseRcvStor> StoredItems { get; set; }
        public string StoredLocation { get; set; }
    }

    public class PurchaseRcvDetResponse
    {
        public PurchaseRcvDet data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseRcvDetResponse()
        {
            this.data = new PurchaseRcvDet();
            this.Response = new ResponseObj();
        }
    }

    public class PurchaseRcvDetsResponse
    {
        public List<PurchaseRcvDet> data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseRcvDetsResponse()
        {
            this.data = new List<PurchaseRcvDet>();
            this.Response = new ResponseObj();
        }
    }

    #endregion


    #region PurchaseRcvDet
    public class PurchaseRcvStor
    {
        [Key]
        public int PurRcvdStoredID { get; set; }
        public int PurchaseRcvdDetailsID { get; set; }
        public int Qty { get; set; }
        public int QtyBags { get; set; }
        public int PalletID { get; set; }
        public string PalletNo { get; set; }
        public int TagColourID { get; set; }
        public string TagColour { get; set; }
        public int RMID { get; set; }
        public string RMName { get; set; }
    }

    public class PurchaseRcvStorResponse
    {
        public PurchaseRcvStor data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseRcvStorResponse()
        {
            this.data = new PurchaseRcvStor();
            this.Response = new ResponseObj();
        }
    }


    public class PurchaseRcvStorsResponse
    {
        public List<PurchaseRcvStor> data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseRcvStorsResponse()
        {
            this.data = new List<PurchaseRcvStor>();
            this.Response = new ResponseObj();
        }
    }

    #endregion



    public class PurchaseRcvPrint
    {
        [Key]
        public int PurchaseRcvdID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Rcvd Date")]
        public DateTime RcvdDate { get; set; }

        [Display(Name = "PO Number")]
        public int PurchaseID { get; set; }

        #region PurchaseDetails

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "PO Date")]
        public System.DateTime PurchaseDate { get; set; }

        [Display(Name = "Company / Location")]
        public string CompanyLocation { get; set; }
        public string Instructions { get; set; }

        [Display(Name = "Booked By")]
        [Editable(allowEdit: false)]
        public string User { get; set; }

        #endregion

        [Editable(allowEdit: false)]
        public string Status { get; set; }

        public int RcvdByUserID { get; set; }

        [Display(Name = "Received By")]
        [Editable(allowEdit: false)]
        public string ReceivedByUser { get; set; }

        public string Notes { get; set; }
        public List<PurchaseRcvDet> LineItems { get; set; }
        public List<PurchaseRcvStor> StoredItems { get; set; }
    }


    public class PurchaseRcvPrintResponse
    {
        public PurchaseRcvPrint data { get; set; }
        public ResponseObj Response { get; set; }

        public PurchaseRcvPrintResponse()
        {
            this.data = new PurchaseRcvPrint() { LineItems = new List<PurchaseRcvDet>(), StoredItems = new List<PurchaseRcvStor>() };
            this.Response = new ResponseObj();
        }
    }

}
