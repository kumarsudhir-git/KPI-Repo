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
    
    public partial class RoleMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RoleMaster()
        {
            this.UserMasters = new HashSet<UserMaster>();
        }
    
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int AddedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserMaster> UserMasters { get; set; }
    }
}
