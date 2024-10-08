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
    
    public partial class PurchaseRcvdMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseRcvdMaster()
        {
            this.PurchaseRcvdDetails = new HashSet<PurchaseRcvdDetail>();
        }
    
        public int PurchaseRcvdID { get; set; }
        public System.DateTime RcvdDate { get; set; }
        public int PurchaseID { get; set; }
        public int RcvdByUserID { get; set; }
        public string Notes { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string QCStatus { get; set; }
        public bool QCReceived { get; set; }
        public Nullable<int> CompanyLocationId { get; set; }
    
        public virtual PurchaseMaster PurchaseMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseRcvdDetail> PurchaseRcvdDetails { get; set; }
        public virtual UserMaster UserMaster { get; set; }
    }
}
