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
    
    public partial class RewardPointsHistory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RewardPointsHistory()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int StoreId { get; set; }
        public int Points { get; set; }
        public Nullable<int> PointsBalance { get; set; }
        public decimal UsedAmount { get; set; }
        public string Message { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public Nullable<System.DateTime> EndDateUtc { get; set; }
        public Nullable<int> ValidPoints { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
