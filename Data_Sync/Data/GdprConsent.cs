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
    
    public partial class GdprConsent
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRequired { get; set; }
        public string RequiredMessage { get; set; }
        public bool DisplayDuringRegistration { get; set; }
        public bool DisplayOnCustomerInfoPage { get; set; }
        public int DisplayOrder { get; set; }
    }
}
