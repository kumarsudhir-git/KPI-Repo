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

        public List<MachineMasterModel> Machines { get; set; }
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
        [Display(Name = "Maintenance Frequency")]
        public int? MaintenanceFrequency { get; set; }
        [Display(Name = "Last Maintenance Date")]
        public DateTime? LastMaintenanceDate { get; set; }
        [Display(Name = "Date Of Purchase")]
        public DateTime? PurchaseDate { get; set; }

        // --- Calculated on the fly (not mapped to DB) ---
        [Display(Name = "Maintenance Due Date")]
        public DateTime? MaintenanceDueDate =>
            (LastMaintenanceDate.HasValue && MaintenanceFrequency.HasValue)
                ? LastMaintenanceDate.Value.AddDays(MaintenanceFrequency.Value)
                : (DateTime?)null;
        [Display(Name = "Due In (Days)")]
        public int? DueInDays =>
            MaintenanceDueDate.HasValue
                ? (int)(MaintenanceDueDate.Value.Date - DateTime.Today).TotalDays
                : (int?)null;
        public Mould()
        {
            Machines = new List<MachineMasterModel>();
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
        public List<MouldMachineMapping> mouldMachineMapData { get; set; }
        public ResponseObj Response { get; set; }

        public MouldMastersResponse()
        {
            this.data = new List<Mould>();
            mouldMachineMapData = new List<MouldMachineMapping>();
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

    public class MouldMachineMapping
    {
        public int MouldID { get; set; }
        public List<int> MachineID { get; set; }
        public int UserID { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
