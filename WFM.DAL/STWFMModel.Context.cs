﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class STWFMEntities : DbContext
    {
        public STWFMEntities()
            : base("name=STWFMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<DataAudit> DataAudits { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<Division> Divisions { get; set; }
        public virtual DbSet<LoginAudit> LoginAudits { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderTracking> OrderTrackings { get; set; }
        public virtual DbSet<OrderWayForward> OrderWayForwards { get; set; }
        public virtual DbSet<PaperQuality> PaperQualities { get; set; }
        public virtual DbSet<Printer> Printers { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderType> OrderTypes { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
    }
}
