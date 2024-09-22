using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public enum enumSalesStatus
    {
        Procure_Material = 10,
        Awaiting_Production = 20,
        In_Production = 30,
        Completed_Closed = 40,
        Cancelled = 999
    }

    public class SalesMaster
    {
        [Key]
        [Display(Name = "SO Number")]
        public int SalesID { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "PO Date")]
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

        public string Colour { get; set; }
        public string GMS { get; set; }
        public string GMSInfo { get; set; }
        public string Package { get; set; }
        public string Quantity { get; set; }
        public bool SampleRequired { get; set; }
        public string DeliveryAddress { get; set; }
        public string Transporter { get; set; }
        public DateTime? CommittedDate { get; set; }
        public bool IsSalesRateAccess { get; set; }
        public string Rate { get; set; }
        public string PaymentStatus { get; set; }
        public List<KeyValuePair> Locations { get; set; }
        public List<SalesDetails> LineItems { get; set; }
        public List<KeyValuePair> Products { get; set; }

        public List<int> RMIds { get; set; }

        public SalesMaster()
        {
            //SalesStatusID = 10;
            //UserID = 1001;
            SalesDate = DateTime.Today;
            LineItems = new List<SalesDetails>();
            Locations = new List<KeyValuePair>();
            Products = new List<KeyValuePair>();
        }
    }

    public class SalesMasterResponse
    {
        public SalesMaster data { get; set; }
        public ResponseObj Response { get; set; }

        public SalesMasterResponse()
        {
            this.data = new SalesMaster();
            this.Response = new ResponseObj();
        }
    }

    public class SalesMastersResponse
    {
        public List<SalesMaster> data { get; set; }
        public ResponseObj Response { get; set; }

        public SalesMastersResponse()
        {
            this.data = new List<SalesMaster>();
            this.Response = new ResponseObj();
        }
    }

}
