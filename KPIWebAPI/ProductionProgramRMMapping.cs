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
    
    public partial class ProductionProgramRMMapping
    {
        public int ProductionProgramRMMappingID { get; set; }
        public int ProductionProgramID { get; set; }
        public int ProductID { get; set; }
        public int RawMaterialID { get; set; }
        public int RMQty { get; set; }
    
        public virtual ProductionProgram ProductionProgram { get; set; }
        public virtual RawMaterialMaster RawMaterialMaster { get; set; }
    }
}
