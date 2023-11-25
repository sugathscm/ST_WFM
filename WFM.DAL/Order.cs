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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.AdditionalCharges = new HashSet<AdditionalCharge>();
            this.InstallationAttachments = new HashSet<InstallationAttachment>();
            this.OrderAttachments = new HashSet<OrderAttachment>();
            this.OrderItems = new HashSet<OrderItem>();
            this.OrderMaterials = new HashSet<OrderMaterial>();
            this.OrderTermDetails = new HashSet<OrderTermDetail>();
        }
    
        public int Id { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Code { get; set; }
        public string BaseQuoteId { get; set; }
        public int ClientId { get; set; }
        public int OrderTypeId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<System.DateTime> CompletionDate { get; set; }
        public int ChanneledById { get; set; }
        public Nullable<int> CodeNumber { get; set; }
        public string ContactPerson { get; set; }
        public Nullable<int> PaperQualityId { get; set; }
        public string ContactMobile { get; set; }
        public Nullable<bool> IsVAT { get; set; }
        public Nullable<int> VATNo { get; set; }
        public Nullable<int> PrinterId { get; set; }
        public string Location { get; set; }
        public string Comments { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<int> CurrentDivisionId { get; set; }
        public Nullable<System.DateTime> LastTransferredDate { get; set; }
        public string LastTransferredBy { get; set; }
        public Nullable<int> QuoteId { get; set; }
        public Nullable<decimal> Value { get; set; }
        public string Header { get; set; }
        public Nullable<int> WarrantyPeriodId { get; set; }
        public string FileNumber { get; set; }
        public int DeliveryTypeId { get; set; }
        public string ArtWork { get; set; }
        public string PurchaseOrder { get; set; }
        public string Designer { get; set; }
        public bool IsPrintingDepartment { get; set; }
        public bool IsCuttingDepartment { get; set; }
        public bool IsPlasticDepartment { get; set; }
        public bool IsCladdingsection { get; set; }
        public bool IsArtDepartment { get; set; }
        public bool IsSteelDepartment { get; set; }
        public bool IsScreenPrintingDept { get; set; }
        public bool IsProductionDepartment { get; set; }
        public bool IsPackingDepartment { get; set; }
        public bool IsInstallationTeam { get; set; }
        public double AdvancePayment { get; set; }
        public Nullable<double> OrderVAT { get; set; }
        public string Remarks { get; set; }
        public string InstallationTeam { get; set; }
        public Nullable<System.DateTime> InstallationDate { get; set; }
        public string InstallationVehicleType { get; set; }
        public string InstallationRemark { get; set; }
        public string InstallationLocation { get; set; }
        public Nullable<bool> isCancelled { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdditionalCharge> AdditionalCharges { get; set; }
        public virtual Client Client { get; set; }
        public virtual DeliveryType DeliveryType { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstallationAttachment> InstallationAttachments { get; set; }
        public virtual OrderType OrderType { get; set; }
        public virtual Quote Quote { get; set; }
        public virtual Status Status { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderAttachment> OrderAttachments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderMaterial> OrderMaterials { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderTermDetail> OrderTermDetails { get; set; }
    }
}
