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
    
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int CategoryId { get; set; }
        public string Size { get; set; }
        public double Qty { get; set; }
        public double UnitCost { get; set; }
        public double TotalCost { get; set; }
        public double VAT { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string CategoryType { get; set; }
        public string Installation { get; set; }
        public Nullable<int> VisibilityId { get; set; }
        public Nullable<int> FrameworkWarrantyPeriod { get; set; }
        public Nullable<int> LetteringWarrantyPeriod { get; set; }
        public Nullable<int> IlluminationWarrantyPeriod { get; set; }
        public Nullable<int> IlluminationId { get; set; }
        public string SpecialInstruction { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Order Order { get; set; }
    }
}
