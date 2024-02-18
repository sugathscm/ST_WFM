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
    
    public partial class Quote
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quote()
        {
            this.OrderTermDetails = new HashSet<OrderTermDetail>();
            this.QuoteTermDetails = new HashSet<QuoteTermDetail>();
            this.QuoteItems = new HashSet<QuoteItem>();
            this.Orders = new HashSet<Order>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ClientId { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Code { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> ChanneledById { get; set; }
        public string Location { get; set; }
        public string Comments { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<decimal> Value { get; set; }
        public Nullable<int> Version { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> BaseQuoteId { get; set; }
        public Nullable<int> CodeNumber { get; set; }
        public Nullable<bool> FileAttched { get; set; }
        public Nullable<bool> ConvertedToOrder { get; set; }
        public bool IsVAT { get; set; }
        public string Header { get; set; }
        public Nullable<bool> IsConverted { get; set; }
        public Nullable<int> WarrantyPeriodId { get; set; }
        public bool IsApproved { get; set; }
        public string ContactPerson { get; set; }
        public string ContactMobile { get; set; }
        public Nullable<int> FrameworkWarrantyPeriod { get; set; }
        public Nullable<int> LetteringWarrantyPeriod { get; set; }
        public Nullable<int> IlluminationWarrantyPeriod { get; set; }
        public string FileNumber { get; set; }
        public Nullable<int> PowerSupplyAmp { get; set; }
        public Nullable<int> AdvancePayment { get; set; }
        public Nullable<int> ValidDays { get; set; }
        public Nullable<int> DeliveryPeriod { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderTermDetail> OrderTermDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuoteTermDetail> QuoteTermDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuoteItem> QuoteItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
        public virtual Client Client { get; set; }
    }
}
