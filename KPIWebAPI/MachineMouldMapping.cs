//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KPIWebAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class MachineMouldMapping
    {
        public int MachineMouldMappingID { get; set; }
        public int MachineID { get; set; }
        public Nullable<int> MouldID { get; set; }
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        public virtual MachineMaster MachineMaster { get; set; }
        public virtual MouldMaster MouldMaster { get; set; }
    }
}
