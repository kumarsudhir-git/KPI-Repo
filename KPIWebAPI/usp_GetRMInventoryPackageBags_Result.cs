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
    
    public partial class usp_GetRMInventoryPackageBags_Result
    {
        public int PackageBagId { get; set; }
        public string Size { get; set; }
        public string VendorName { get; set; }
        public Nullable<int> QtyInStock { get; set; }
        public Nullable<int> MinOrderLevel { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public System.DateTime AddedOn { get; set; }
        public string AddedByName { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedByName { get; set; }
    }
}
