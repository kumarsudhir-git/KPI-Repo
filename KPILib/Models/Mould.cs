using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public enum enumMouldStatus
    {
        NotInUse = 101,
        InUse = 102,
        InMaintainance = 103,
        Loaned = 104,
        Discontinued = 105
    }

    public class Mould
    {
        public int MouldID { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Mould Code")]
        public string MouldName { get; set; }
        public string Description { get; set; }
        [Display(Name = "Mould Type")]
        public int MouldTypeID { get; set; }
        [Display(Name = "Is Discontinued")]
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

        public int MouldStatusID { get; set; }
        public string MouldStatus { get; set; }

        public List<Machine> Machines { get; set; }
        public string AllMachines { get; set; }

        public List<Product> Products { get; set; }
        public string AllProducts { get; set; }
        public int? InProductionID { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        [Display(Name = "Location Name")]
        public string LocationName { get; set; }
        [Display(Name = "Total No Of Cavities")]
        public string TotalCavities { get; set; }
        [Display(Name = "Running Cavities")]
        public string RunningCavities { get; set; }
        [Display(Name = "Core Pins")]
        public string CorePins { get; set; }
        public Mould()
        {
            Machines = new List<Machine>();
            Products = new List<Product>();
        }
    }


    public class MouldMasterResponse
    {
        public Mould data { get; set; }
        public ResponseObj Response { get; set; }
        public List<MouldTypeMaster> MouldTypeMasters { get; set; }
        public MouldMasterResponse()
        {
            this.data = new Mould();
            this.Response = new ResponseObj();
            this.MouldTypeMasters = new List<MouldTypeMaster>();
        }
    }

    public class MouldMastersResponse
    {
        public List<Mould> data { get; set; }
        public ResponseObj Response { get; set; }

        public MouldMastersResponse()
        {
            this.data = new List<Mould>();
            this.Response = new ResponseObj();
        }
    }

    public class MouldTypeMaster
    {
        public int MouldTypeID { get; set; }
        public string MouldType { get; set; }
        public string Description { get; set; }
        public bool IsDiscontinued { get; set; }
        //public DateTime AddedOn { get; set; }
        //public DateTime? LastModifiedOn { get; set; }
    }
}
