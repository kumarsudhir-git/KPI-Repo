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
    
    public partial class ProductionProgramBatch
    {
        public long ProgramBatchID { get; set; }
        public Nullable<int> ProductionProgramID { get; set; }
        public int InProductionQty { get; set; }
        public int ProductQtyCompleted { get; set; }
        public int UserID { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
    
        public virtual ProductionProgram ProductionProgram { get; set; }
    }
}
