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
    
    public partial class CompanyMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CompanyMaster()
        {
            this.CompanyLocationMasters = new HashSet<CompanyLocationMaster>();
        }
    
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int CompanyTypeID { get; set; }
        public string Notes { get; set; }
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CompanyLocationMaster> CompanyLocationMasters { get; set; }
        public virtual CompanyTypeMaster CompanyTypeMaster { get; set; }
    }
}
