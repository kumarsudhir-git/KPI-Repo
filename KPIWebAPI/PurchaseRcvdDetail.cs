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
    
    public partial class PurchaseRcvdDetail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PurchaseRcvdDetail()
        {
            this.PurchaseRcvdStoreds = new HashSet<PurchaseRcvdStored>();
        }
    
        public int PurchaseRcvdDetailsID { get; set; }
        public int PurchaseRcvdID { get; set; }
        public int PurchaseDetailsID { get; set; }
        public int Qty { get; set; }
    
        public virtual PurchaseDetail PurchaseDetail { get; set; }
        public virtual PurchaseRcvdMaster PurchaseRcvdMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseRcvdStored> PurchaseRcvdStoreds { get; set; }
    }
}
