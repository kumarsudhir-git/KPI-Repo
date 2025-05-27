using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public enum enumMachineStatus
    {
        NotInUse = 101,
        InUse = 102,
        InMaintainance = 103,
        Discontinued = 104
    }

    public class MachineMasterModel
    {
        public int MachineID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string MachineName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Required")]
        public int MachineTypeID { get; set; }
        public string MachineTypeName { get; set; }
        public bool IsDiscontinued { get; set; }
        public DateTime AddedOn { get; set; }
        public int AddedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        [Required(ErrorMessage = "Required")]
        public int MachineStatusID { get; set; }
        public string MachineStatus { get; set; }
    }


    public class MachineMasterResponse
    {
        public MachineMasterModel data { get; set; }
        public ResponseObj Response { get; set; }
        public List<MachineTypeMasterModel> machineTypeMasterData { get; set; }
        public List<MachineStatusMasterModel> machineStatusMasterData { get; set; }
        public MachineMasterResponse()
        {
            this.data = new MachineMasterModel();
            this.Response = new ResponseObj();
            machineTypeMasterData = new List<MachineTypeMasterModel>();
            machineStatusMasterData = new List<MachineStatusMasterModel>();
        }
    }

    public class MachineMastersResponse
    {
        public List<MachineMasterModel> data { get; set; }
        public ResponseObj Response { get; set; }

        public MachineMastersResponse()
        {
            this.data = new List<MachineMasterModel>();
            this.Response = new ResponseObj();
        }
    }
    public class MachineTypeMasterResponse
    {
        public List<MachineTypeMasterModel> machineTypeMasterData { get; set; }
        public ResponseObj Response { get; set; }

        public MachineTypeMasterResponse()
        {
            this.machineTypeMasterData = new List<MachineTypeMasterModel>();
            this.Response = new ResponseObj();
        }
    }
    public class MachineStatusMasterResponse
    {
        public List<MachineStatusMasterModel> machineStatusMasterData { get; set; }
        public ResponseObj Response { get; set; }

        public MachineStatusMasterResponse()
        {
            this.machineStatusMasterData = new List<MachineStatusMasterModel>();
            this.Response = new ResponseObj();
        }
    }

    public partial class MachineTypeMasterModel
    {
        public int MachineTypeID { get; set; }
        public string MachineType { get; set; }
        public string Description { get; set; }
        public bool IsDiscontinued { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public partial class MachineStatusMasterModel
    {
        public int MachineStatusID { get; set; }
        public string MachineStatus { get; set; }
        public DateTime AddedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }

    public class MachineMouldMappingResponse
    {
        public List<MachineMouldMappingModel> data { get; set; }
        public List<MachineMouldMapping> machineToMouldMap { get; set; }
        public ResponseObj Response { get; set; }
        public MachineMouldMappingResponse()
        {
            this.data = new List<MachineMouldMappingModel>();
            this.machineToMouldMap = new List<MachineMouldMapping>();
            this.Response = new ResponseObj();
        }
    }

    public class MachineMouldMapping
    {
        public int MachineID { get; set; }
        public List<int> MouldID { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
    }

    public class MachineMouldMappingModel
    {
        public int MachineMouldMappingID { get; set; }
        public int MachineId { get; set; }
        public string MachineName { get; set; }
        public int MouldId { get; set; }
        public string MouldName { get; set; }
        public bool IsDiscontinued { get; set; }
        public int UserId { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
