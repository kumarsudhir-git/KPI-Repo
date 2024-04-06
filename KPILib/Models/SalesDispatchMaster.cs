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
