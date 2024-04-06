using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class SalesMastDetail
    {
        [Key]
        public int SalesDetailsID { get; set; }
        public int SalesID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Qty { get; set; }
        public string SalesDetailsInstructions { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "PO Date")]
        public System.DateTime SalesDate { get; set; }

        [Display(Name = "Company / Location")]
        public string CompanyLocation { get; set; }
        public string SalesMasterInstructions { get; set; }

        [Editable(allowEdit: false)]
        public string Status { get; set; }

        public string User { get; set; }

    }


    public class SalesMastDetailResponse
    {
        public SalesMastDetail data { get; set; }
        public ResponseObj Response { get; set; }

        public SalesMastDetailResponse()
        {
            this.data = new SalesMastDetail();
            this.Response = new ResponseObj();
        }
    }

    public class SalesMastDetailsResponse
    {
        public List<SalesMastDetail> data { get; set; }
        public ResponseObj Response { get; set; }

        public SalesMastDetailsResponse()
        {
            this.data = new List<SalesMastDetail>();
            this.Response = new ResponseObj();
        }
    }

}
