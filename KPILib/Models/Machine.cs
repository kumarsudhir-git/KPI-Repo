using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public enum enumMachineStatus
    {
        NotInUse = 101,
        InUse = 102,
        InMaintainance = 103,
        Discontinued = 104
    }

    public class Machine
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string Description { get; set; }
        public int MachineTypeID { get; set; }
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

        public int MachineStatusID { get; set; }
        public string MachineStatus { get; set; }
    }


    public class MachineMasterResponse
    {
        public Machine data { get; set; }
        public ResponseObj Response { get; set; }

        public MachineMasterResponse()
        {
            this.data = new Machine();
            this.Response = new ResponseObj();
        }
    }

    public class MachineMastersResponse
    {
        public List<Machine> data { get; set; }
        public ResponseObj Response { get; set; }

        public MachineMastersResponse()
        {
            this.data = new List<Machine>();
            this.Response = new ResponseObj();
        }
    }

}
