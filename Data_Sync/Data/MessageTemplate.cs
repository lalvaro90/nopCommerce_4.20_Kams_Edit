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
    
    public partial class MessageTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BccEmailAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> DelayBeforeSend { get; set; }
        public int DelayPeriodId { get; set; }
        public int AttachedDownloadId { get; set; }
        public int EmailAccountId { get; set; }
        public bool LimitedToStores { get; set; }
    }
}
