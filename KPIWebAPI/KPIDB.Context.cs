﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KPIWebAPI
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class KPIEntities : DbContext
    {
        public KPIEntities()
            : base("name=KPIEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CompanyLocationMaster> CompanyLocationMasters { get; set; }
        public virtual DbSet<CompanyMaster> CompanyMasters { get; set; }
        public virtual DbSet<CompanyTypeMaster> CompanyTypeMasters { get; set; }
        public virtual DbSet<DynamicURL> DynamicURLs { get; set; }
        public virtual DbSet<MachineHistory> MachineHistories { get; set; }
        public virtual DbSet<MachineMaster> MachineMasters { get; set; }
        public virtual DbSet<MachineMouldMapping> MachineMouldMappings { get; set; }
        public virtual DbSet<MachineStatusMaster> MachineStatusMasters { get; set; }
        public virtual DbSet<MachineTypeMaster> MachineTypeMasters { get; set; }
        public virtual DbSet<MailMaster> MailMasters { get; set; }
        public virtual DbSet<MenuMaster> MenuMasters { get; set; }
        public virtual DbSet<MouldHistory> MouldHistories { get; set; }
        public virtual DbSet<MouldInventoryMaster> MouldInventoryMasters { get; set; }
        public virtual DbSet<MouldMaster> MouldMasters { get; set; }
        public virtual DbSet<MouldStatusMaster> MouldStatusMasters { get; set; }
        public virtual DbSet<MouldTypeMaster> MouldTypeMasters { get; set; }
        public virtual DbSet<OrderStatusMaster> OrderStatusMasters { get; set; }
        public virtual DbSet<PalletMaster> PalletMasters { get; set; }
        public virtual DbSet<ProdReadyStored> ProdReadyStoreds { get; set; }
        public virtual DbSet<ProductCategoryMaster> ProductCategoryMasters { get; set; }
        public virtual DbSet<ProductGroupDetail> ProductGroupDetails { get; set; }
        public virtual DbSet<ProductGroupMaster> ProductGroupMasters { get; set; }
        public virtual DbSet<ProductInventoryMaster> ProductInventoryMasters { get; set; }
        public virtual DbSet<ProductionProgram> ProductionPrograms { get; set; }
        public virtual DbSet<ProductionProgramBatch> ProductionProgramBatches { get; set; }
        public virtual DbSet<ProductionProgramRMMapping> ProductionProgramRMMappings { get; set; }
        public virtual DbSet<ProductionRawMaterial> ProductionRawMaterials { get; set; }
        public virtual DbSet<ProductMaster> ProductMasters { get; set; }
        public virtual DbSet<ProductRawMaterialMapping> ProductRawMaterialMappings { get; set; }
        public virtual DbSet<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual DbSet<PurchaseMaster> PurchaseMasters { get; set; }
        public virtual DbSet<PurchaseRcvdDetail> PurchaseRcvdDetails { get; set; }
        public virtual DbSet<PurchaseRcvdMaster> PurchaseRcvdMasters { get; set; }
        public virtual DbSet<PurchaseRcvdStored> PurchaseRcvdStoreds { get; set; }
        public virtual DbSet<PurchaseStatusMaster> PurchaseStatusMasters { get; set; }
        public virtual DbSet<RackMaster> RackMasters { get; set; }
        public virtual DbSet<RawMaterialInventoryMaster> RawMaterialInventoryMasters { get; set; }
        public virtual DbSet<RawMaterialMaster> RawMaterialMasters { get; set; }
        public virtual DbSet<RoleMaster> RoleMasters { get; set; }
        public virtual DbSet<RoleRight> RoleRights { get; set; }
        public virtual DbSet<SalesDetail> SalesDetails { get; set; }
        public virtual DbSet<SalesDispatchDetail> SalesDispatchDetails { get; set; }
        public virtual DbSet<SalesDispatchRack> SalesDispatchRacks { get; set; }
        public virtual DbSet<SalesMaster> SalesMasters { get; set; }
        public virtual DbSet<SalesStatusMaster> SalesStatusMasters { get; set; }
        public virtual DbSet<TagColourMaster> TagColourMasters { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<UOMMaster> UOMMasters { get; set; }
        public virtual DbSet<UserMaster> UserMasters { get; set; }
    
        public virtual ObjectResult<Nullable<int>> sp_GetEmptyPallets()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_GetEmptyPallets");
        }
    
        public virtual ObjectResult<Nullable<int>> sp_GetEmptyRacks()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_GetEmptyRacks");
        }
    
        public virtual ObjectResult<sp_GetRMInventory_Result> sp_GetRMInventory()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetRMInventory_Result>("sp_GetRMInventory");
        }
    
        public virtual ObjectResult<Nullable<int>> sp_GetAvailableMachineID(Nullable<int> mouldID)
        {
            var mouldIDParameter = mouldID.HasValue ?
                new ObjectParameter("MouldID", mouldID) :
                new ObjectParameter("MouldID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_GetAvailableMachineID", mouldIDParameter);
        }
    }
}
