//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WFM.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class QuoteItem
    {
        public int Id { get; set; }
        public Nullable<int> QuoteId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string Size { get; set; }
        public Nullable<double> Qty { get; set; }
        public Nullable<double> UnitCost { get; set; }
        public Nullable<double> VAT { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CategoryType { get; set; }
        public string Installation { get; set; }
        public string Visibility { get; set; }
        public Nullable<int> FrameworkWarrantyPeriod { get; set; }
        public Nullable<int> LetteringWarrantyPeriod { get; set; }
        public Nullable<int> IlluminationWarrantyPeriod { get; set; }
        public Nullable<int> VisibilityId { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Quote Quote { get; set; }
    }
}
