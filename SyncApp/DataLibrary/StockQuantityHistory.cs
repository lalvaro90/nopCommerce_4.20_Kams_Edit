//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLibrary
{
    using System;
    using System.Collections.Generic;
    
    public partial class StockQuantityHistory
    {
        public int Id { get; set; }
        public int QuantityAdjustment { get; set; }
        public int StockQuantity { get; set; }
        public string Message { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public int ProductId { get; set; }
        public Nullable<int> CombinationId { get; set; }
        public Nullable<int> WarehouseId { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
