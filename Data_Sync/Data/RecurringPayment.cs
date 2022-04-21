//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data_Sync.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class RecurringPayment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RecurringPayment()
        {
            this.RecurringPaymentHistories = new HashSet<RecurringPaymentHistory>();
        }
    
        public int Id { get; set; }
        public int CycleLength { get; set; }
        public int CyclePeriodId { get; set; }
        public int TotalCycles { get; set; }
        public System.DateTime StartDateUtc { get; set; }
        public bool IsActive { get; set; }
        public bool LastPaymentFailed { get; set; }
        public bool Deleted { get; set; }
        public int InitialOrderId { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
    
        public virtual Order Order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecurringPaymentHistory> RecurringPaymentHistories { get; set; }
    }
}
