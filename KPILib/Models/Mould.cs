using System;
using System.Collections.Generic;
using System.Text;
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
        public string MouldName { get; set; }
        public string Description { get; set; }
        public int MouldTypeID { get; set; }
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

        public MouldMasterResponse()
        {
            this.data = new Mould();
            this.Response = new ResponseObj();
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

}
