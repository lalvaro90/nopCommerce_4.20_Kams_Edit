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
    
    public partial class Forums_Post
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Forums_Post()
        {
            this.Forums_PostVote = new HashSet<Forums_PostVote>();
        }
    
        public int Id { get; set; }
        public int TopicId { get; set; }
        public int CustomerId { get; set; }
        public string Text { get; set; }
        public string IPAddress { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public System.DateTime UpdatedOnUtc { get; set; }
        public int VoteCount { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Forums_Topic Forums_Topic { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Forums_PostVote> Forums_PostVote { get; set; }
    }
}
