//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KamsUserManager.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaxTransactionLog
    {
        public int Id { get; set; }
        public int StatusCode { get; set; }
        public string Url { get; set; }
        public string RequestMessage { get; set; }
        public string ResponseMessage { get; set; }
        public int CustomerId { get; set; }
        public System.DateTime CreatedDateUtc { get; set; }
    }
}
