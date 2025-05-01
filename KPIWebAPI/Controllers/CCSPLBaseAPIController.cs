using System;
using System.Configuration;
using System.Web.Http;
using AutoMapper;

namespace KPIWebAPI.Controllers
{

    public class CCSPLBaseAPIController : ApiController
    {
        // Global variables for the base class
        public int RM_BAG_CAPACITY = Convert.ToInt32(ConfigurationManager.AppSettings["RMCapacityPerBagInKG"]);
        public int PALLET_CAPACITY = Convert.ToInt32(ConfigurationManager.AppSettings["PalletCapacityInKG"]);

        // Global variable for generating random numbers
        public static Random RANDOM_NUMBER = new Random();

        public KPIEntities db = new KPIEntities();

        //// Function for generating random number (to be used in LINQ to SQL queries) ... not useful
        //public static int GetRandomNumber()
        //{
        //    return RANDOM_NUMBER.Next();
        //}

        static MapperConfiguration config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<PalletMaster, KPILib.Models.PalletMaster>();
            cfg.CreateMap<KPILib.Models.PalletMaster, PalletMaster>();

            cfg.CreateMap<RackMaster, KPILib.Models.RackMaster>();
            cfg.CreateMap<KPILib.Models.RackMaster, RackMaster>();

            cfg.CreateMap<RawMaterialMaster, KPILib.Models.RawMaterial>();
            cfg.CreateMap<KPILib.Models.RawMaterial, RawMaterialMaster>();

            cfg.CreateMap<UOMMaster, KPILib.Models.UOM>();
            cfg.CreateMap<KPILib.Models.UOM, UOMMaster>();

            cfg.CreateMap<RawMaterialInventoryMaster, KPILib.Models.RMInventory>();
            cfg.CreateMap<KPILib.Models.RMInventory, RawMaterialInventoryMaster>();

            cfg.CreateMap<CompanyMaster, KPILib.Models.Company>();
            cfg.CreateMap<KPILib.Models.Company, CompanyMaster>();

            cfg.CreateMap<CompanyLocationMaster, KPILib.Models.CompanyLocation>();
            cfg.CreateMap<KPILib.Models.CompanyLocation, CompanyLocationMaster>();

            cfg.CreateMap<PurchaseMaster, KPILib.Models.PurchaseMaster>();
            cfg.CreateMap<KPILib.Models.PurchaseMaster, PurchaseMaster>();

            cfg.CreateMap<PurchaseDetail, KPILib.Models.PurchaseDetails>();
            cfg.CreateMap<KPILib.Models.PurchaseDetails, PurchaseDetail>();

            cfg.CreateMap<PurchaseRcvdMaster, KPILib.Models.PurchaseRcvMast>();
            cfg.CreateMap<KPILib.Models.PurchaseRcvMast, PurchaseRcvdMaster>();

            cfg.CreateMap<PurchaseRcvdDetail, KPILib.Models.PurchaseRcvDet>();
            cfg.CreateMap<KPILib.Models.PurchaseRcvDet, PurchaseRcvdDetail>();

            cfg.CreateMap<PurchaseRcvdStored, KPILib.Models.PurchaseRcvStor>();
            cfg.CreateMap<KPILib.Models.PurchaseRcvStor, PurchaseRcvdStored>();


            cfg.CreateMap<ProductMaster, KPILib.Models.Product>();
            cfg.CreateMap<KPILib.Models.Product, ProductMaster>();

            cfg.CreateMap<MouldMaster, KPILib.Models.Mould>();
            cfg.CreateMap<KPILib.Models.Mould, MouldMaster>();

            cfg.CreateMap<ProductCategoryMaster, KPILib.Models.ProductCategory>();
            cfg.CreateMap<KPILib.Models.ProductCategory, ProductCategoryMaster>();


            cfg.CreateMap<TagColourMaster, KPILib.Models.TagColor>();
            cfg.CreateMap<KPILib.Models.TagColor, TagColourMaster>();

            cfg.CreateMap<SalesMaster, KPILib.Models.SalesMaster>();
            cfg.CreateMap<KPILib.Models.SalesMaster, SalesMaster>();

            cfg.CreateMap<SalesDetail, KPILib.Models.SalesDetails>();
            cfg.CreateMap<KPILib.Models.SalesDetails, SalesDetail>();


            cfg.CreateMap<SalesMaster, KPILib.Models.SalesDispatchMaster>();
            cfg.CreateMap<KPILib.Models.SalesDispatchMaster, SalesMaster>();

            cfg.CreateMap<SalesDetail, KPILib.Models.SalesDispatchItem>().ForMember(sdi => sdi.QtyBooked, sd => sd.MapFrom(x => x.Qty));
            cfg.CreateMap<KPILib.Models.SalesDispatchItem, SalesDetail>().ForMember(sd => sd.Qty, sdi => sdi.MapFrom(x => x.QtyBooked));


            cfg.CreateMap<sp_GetRMInventory_Result, KPILib.Models.RMInventory>();
            cfg.CreateMap<KPILib.Models.RMInventory, sp_GetRMInventory_Result>();


            cfg.CreateMap<ProductionProgram, KPILib.Models.ProductionPrograme>();
            cfg.CreateMap<KPILib.Models.ProductionPrograme, ProductionProgram>();

            cfg.CreateMap<ProductionProgram, KPILib.Models.ProductionPlan>();
            //cfg.CreateMap<KPILib.Models.ProductionPlan, ProductionProgram>();

            cfg.CreateMap<ProductionProgram, KPILib.Models.ProductRackingPlan>();
            //cfg.CreateMap<KPILib.Models.ProductRackingPlan, ProductionProgram>();


            cfg.CreateMap<MouldMaster, KPILib.Models.Mould>();
            cfg.CreateMap<KPILib.Models.Mould, MouldMaster>();

            cfg.CreateMap<MachineMaster, KPILib.Models.MachineMasterModel>();
            cfg.CreateMap<KPILib.Models.MachineMasterModel, MachineMaster>();

            cfg.CreateMap<RoleMaster, KPILib.Models.RoleMaster>();
            cfg.CreateMap<KPILib.Models.RoleMaster, RoleMaster>();

            cfg.CreateMap<UserMaster, KPILib.Models.UserMaster>();
            cfg.CreateMap<KPILib.Models.UserMaster, UserMaster>();

            cfg.CreateMap<ProductRawMaterialMapping, KPILib.Models.ProductRawMaterialMapping>();
            cfg.CreateMap<KPILib.Models.ProductRawMaterialMapping, ProductRawMaterialMapping>();

            cfg.CreateMap<Unit, KPILib.Models.Unit>();
            cfg.CreateMap<KPILib.Models.Unit, Unit>();

            cfg.CreateMap<RoleRight, KPILib.Models.RoleRights>();
            cfg.CreateMap<KPILib.Models.RoleRights, RoleRight>();

            cfg.CreateMap<MenuMaster, KPILib.Models.MenuMaster>();
            cfg.CreateMap<KPILib.Models.MenuMaster, MenuMaster>();

            cfg.CreateMap<ProductionProgramBatch, KPILib.Models.ProductionBatches>();
            cfg.CreateMap<KPILib.Models.ProductionBatches, ProductionProgramBatch>();

            cfg.CreateMap<KPILib.Models.ProductionPrograme, KPILib.Models.ProductionBatches>();
            cfg.CreateMap<KPILib.Models.ProductionBatches, KPILib.Models.ProductionPrograme>();

            cfg.CreateMap<MouldTypeMaster, KPILib.Models.MouldTypeMaster>();
            cfg.CreateMap<KPILib.Models.MouldTypeMaster, MouldTypeMaster>();
            cfg.CreateMap<VendorMaster, KPILib.Models.VendorMasterModel>();
            cfg.CreateMap<KPILib.Models.VendorMasterModel, VendorMaster>();

            cfg.CreateMap<LocationMaster, KPILib.Models.LocationMasterModel>();
            cfg.CreateMap<KPILib.Models.LocationMasterModel, LocationMaster>();

            cfg.CreateMap<usp_GetLocationMasterAllData_Result, KPILib.Models.LocationMasterModel>();
            cfg.CreateMap<KPILib.Models.LocationMasterModel, usp_GetLocationMasterAllData_Result>();

            cfg.CreateMap<RMInventoryMasterBatch, KPILib.Models.RMInventoryMasterBatchModel>();
            cfg.CreateMap<KPILib.Models.RMInventoryMasterBatchModel, RMInventoryMasterBatch>();
            
            cfg.CreateMap<usp_GetRMInventoryMasterBatch_Result, KPILib.Models.RMInventoryMasterBatchModel>();
            cfg.CreateMap<KPILib.Models.RMInventoryMasterBatchModel, usp_GetRMInventoryMasterBatch_Result>();

            cfg.CreateMap<RMInventoryPackageBag, KPILib.Models.RMInventoryPackageBagsModel>();
            cfg.CreateMap<KPILib.Models.RMInventoryPackageBagsModel, RMInventoryPackageBag>();
            
            cfg.CreateMap<usp_GetRMInventoryPackageBags_Result, KPILib.Models.RMInventoryPackageBagsModel>();
            cfg.CreateMap<KPILib.Models.RMInventoryPackageBagsModel, usp_GetRMInventoryPackageBags_Result>();

            cfg.CreateMap<RMInventoryFinishedGood, KPILib.Models.RMInventoryFinishedGoodModel>();
            cfg.CreateMap<KPILib.Models.RMInventoryFinishedGoodModel, RMInventoryFinishedGood>();
            
            cfg.CreateMap<usp_GetRMInventoryFinishedGood_Result, KPILib.Models.RMInventoryFinishedGoodModel>();
            cfg.CreateMap<KPILib.Models.RMInventoryFinishedGoodModel, usp_GetRMInventoryFinishedGood_Result>();

            cfg.CreateMap<MachineTypeMaster, KPILib.Models.MachineTypeMasterModel>();
            cfg.CreateMap<KPILib.Models.MachineTypeMasterModel, MachineTypeMaster>();
            
            cfg.CreateMap<usp_GetRMInventoryFinishedGood_Result, KPILib.Models.RMInventoryFinishedGoodModel>();
            cfg.CreateMap<KPILib.Models.RMInventoryFinishedGoodModel, usp_GetRMInventoryFinishedGood_Result>();

            cfg.CreateMap<MachineStatusMaster, KPILib.Models.MachineStatusMasterModel>();
            cfg.CreateMap<KPILib.Models.MachineStatusMasterModel, MachineStatusMaster>();
            
            cfg.CreateMap<TagColourMaster, KPILib.Models.TagColorMasterModel>();
            cfg.CreateMap<KPILib.Models.TagColorMasterModel, TagColourMaster>();
            
            cfg.CreateMap<LookUpMaster, KPILib.Models.LookUpMasterModel>();
            cfg.CreateMap<KPILib.Models.LookUpMasterModel, LookUpMaster>();
            
            cfg.CreateMap<SalesStatusMaster, KPILib.Models.SalesStatusMaster>();
            cfg.CreateMap<KPILib.Models.SalesStatusMaster, SalesStatusMaster>();
        });

        public static IMapper mapper = config.CreateMapper();
    }
}
