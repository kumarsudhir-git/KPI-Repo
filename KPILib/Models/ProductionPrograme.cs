using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public class ProductionPrograme
    {
        public int ProductionProgramID { get; set; }
        public Nullable<int> SalesDetailsID { get; set; }

        public int SalesID { get; set; }
        public DateTime SalesDate { get; set; }
        public int SalesUserID { get; set; }
        public string SalesUser { get; set; }
        public int CompanyLocationID { get; set; }
        public string CompanyLocation { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public int ProductQty { get; set; }
        public int RawMaterialID { get; set; }

        //public string RawMaterialName { get; set; }

        public int RMQty { get; set; }
        public int MouldID { get; set; }

        public string MouldName { get; set; }
        public int InProductionQty { get; set; }
        public List<MachineAvailability> Machines { get; set; }
        public List<MappedRawMaterial> MappedRawMaterials { get; set; }
        public string MachineNames { get; set; }
        public int MachineID { get; set; }

        public string MachineName { get; set; }

        public int ProductQtyCompleted { get; set; }
        public int UserID { get; set; }

        public string ProductionUser { get; set; }

        public bool RMAvailable { get; set; }
        public bool MouldAvailable { get; set; }
        public bool MachineAvailable { get; set; }
        public bool ProgramInProgress { get; set; }

        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

        public int CanProduceQty { get; set; }
        public int BalanceQty { get; set; }
        public List<ProductionBatches> ProgramBatches { get; set; }

        public ProductionPrograme()
        {
            Machines = new List<MachineAvailability>();
            ProgramBatches = new List<ProductionBatches>();
        }
    }

    public class ProductionProgramesResponse
    {
        public List<ProductionPrograme> data { get; set; }
        public ProductionPrograme productionPrograme { get; set; }
        public ResponseObj Response { get; set; }

        public ProductionProgramesResponse()
        {
            this.data = new List<ProductionPrograme>();
            this.productionPrograme = new ProductionPrograme();
            this.Response = new ResponseObj();
        }
    }

    public class MachineAvailability
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public bool MachineAvailable { get; set; }

    }

    public class MappedRawMaterial
    {
        public int RawMaterialID { get; set; }
        public string RawMaterialName { get; set; }
        public int RMQty { get; set; }
        public decimal RMAvailableQty { get; set; }
        public bool IsRMQtyLessThanRequired
        {
            get
            {
                return RMAvailableQty < RMQty;
            }
        }
    }

    public class ProductionPlan
    {
        public int ProductionProgramID { get; set; }
        public DateTime ProductionProgramDate { get; set; }
        public Nullable<int> SalesDetailsID { get; set; }

        public int SalesID { get; set; }
        public DateTime SalesDate { get; set; }
        public int SalesUserID { get; set; }
        public string SalesUser { get; set; }
        public int CompanyLocationID { get; set; }
        public string CompanyLocation { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public int ProductQty { get; set; }
        public int RawMaterialID { get; set; }

        //public string RawMaterialName { get; set; }

        public int RMQty { get; set; }
        public int MouldID { get; set; }

        public string MouldName { get; set; }

        public int MachineID { get; set; }

        public string MachineName { get; set; }

        public int ProductQtyCompleted { get; set; }
        public int UserID { get; set; }

        public string ProductionUser { get; set; }

        public List<RMInventory> RawMaterialList { get; set; }
    }

    public class ProductionPlanResponse
    {
        public ProductionPlan data { get; set; }
        public ResponseObj Response { get; set; }

        public ProductionPlanResponse()
        {
            this.data = new ProductionPlan();
            data.RawMaterialList = new List<RMInventory>();
            this.Response = new ResponseObj();
        }
    }

    public class ProductRackingPlan
    {
        public int ProductionProgramID { get; set; }
        public DateTime ProductionProgramDate { get; set; }
        public Nullable<int> SalesDetailsID { get; set; }

        public int SalesID { get; set; }
        public DateTime SalesDate { get; set; }
        public int SalesUserID { get; set; }
        public string SalesUser { get; set; }
        public int CompanyLocationID { get; set; }
        public string CompanyLocation { get; set; }

        public int ProductID { get; set; }
        public string ProductName { get; set; }

        public int ProductQty { get; set; }
        public int RawMaterialID { get; set; }

        //public string RawMaterialName { get; set; }

        public int RMQty { get; set; }
        public int MouldID { get; set; }

        public string MouldName { get; set; }

        public int MachineID { get; set; }

        public string MachineName { get; set; }

        public int ProductQtyCompleted { get; set; }
        public int UserID { get; set; }

        public string ProductionUser { get; set; }

        public List<ProductInRack> ProductInRacks { get; set; }
    }

    public class ProductInRack
    {
        public int ProdReadyStoredID { get; set; }
        public Nullable<int> ProductionProgramID { get; set; }
        public int Qty { get; set; }
        public int RackID { get; set; }
        public string RackNo { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public System.DateTime RcvdDate { get; set; }
    }

    public class ProductRackingPlanResponse
    {
        public ProductRackingPlan data { get; set; }
        public ResponseObj Response { get; set; }

        public ProductRackingPlanResponse()
        {
            this.data = new ProductRackingPlan();
            data.ProductInRacks = new List<ProductInRack>();
            this.Response = new ResponseObj();
        }
    }

    public class ProductionBatches
    {
        public ProductionBatches()
        {
            MappedRawMaterials = new List<MappedRawMaterial>();
        }
        public long ProgramBatchID { get; set; }
        public int? ProductionProgramID { get; set; }
        public int InProductionQty { get; set; }
        public int ProductQtyCompleted { get; set; }
        public int UserID { get; set; }
        public DateTime? AddedOn { get; set; }
        public string MouldName { get; set; }
        public string ProductName { get; set; }
        public int RMQty { get; set; }
        public int ProductQty { get; set; }
        public int CanProduceQty { get; set; }
        public int BalanceQty { get; set; }
        public string MachineName { get; set; }
        public bool RMAvailable { get; set; }
        public bool MouldAvailable { get; set; }
        public bool MachineAvailable { get; set; }
        public bool ProgramInProgress { get; set; }
        public string MachineNames { get; set; }
        public string ProductionUser { get; set; }
        public string CompanyLocation { get; set; }
        public int SalesID { get; set; }
        public DateTime SalesDate { get; set; }
        public int SalesUserID { get; set; }
        public string SalesUser { get; set; }
        public List<MappedRawMaterial> MappedRawMaterials { get; set; }
    }
}
