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
    
    public partial class RMInventoryMasterBatch
    {
        public int MasterBatchId { get; set; }
        public string CodeNo { get; set; }
        public string Colour { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> QtyInStock { get; set; }
        public Nullable<int> MinOrderLevel { get; set; }
        public Nullable<int> LocationId { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}
