using KPILib.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace KPI.Classes
{
    public static class KPIAPIManager
    {
        #region API calls for Pallets
        public static PalletMastersResponse GetAllPallets()
        {
            var result = CommonFunctions.client.GetAsync("PalletMasterAPI/GetAll").Result.Content.ReadAsAsync<PalletMastersResponse>().Result;
            return result;
        }

        public static PalletMastersResponse GetAllPallets(int rmid)
        {
            var result = CommonFunctions.client.GetAsync("PalletMasterAPI/GetAllByRMID/" + rmid.ToString()).Result.Content.ReadAsAsync<PalletMastersResponse>().Result;
            return result;
        }

        public static PalletMasterResponse GetPallet(int id)
        {
            var result = CommonFunctions.client.GetAsync("PalletMasterAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<PalletMasterResponse>().Result;
            return result;
        }

        public static PalletMasterResponse AddPallet(PalletMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("PalletMasterAPI/Add", obj).Result.Content.ReadAsAsync<PalletMasterResponse>().Result;
            return result;
        }

        public static PalletMasterResponse EditPallet(PalletMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("PalletMasterAPI/Edit", obj).Result.Content.ReadAsAsync<PalletMasterResponse>().Result;
            return result;
        }
        #endregion

        #region API calls for Racks
        public static RackMastersResponse GetAllRacks()
        {
            var result = CommonFunctions.client.GetAsync("RackMasterAPI/GetAll").Result.Content.ReadAsAsync<RackMastersResponse>().Result;
            return result;
        }

        public static RackMastersResponse GetAllRacks(int prodid)
        {
            var result = CommonFunctions.client.GetAsync("RackMasterAPI/GetAllByProdID/" + prodid.ToString()).Result.Content.ReadAsAsync<RackMastersResponse>().Result;
            return result;
        }

        public static RackMasterResponse GetRack(int id)
        {
            var result = CommonFunctions.client.GetAsync("RackMasterAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<RackMasterResponse>().Result;
            return result;
        }

        public static RackMasterResponse AddRack(RackMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RackMasterAPI/Add", obj).Result.Content.ReadAsAsync<RackMasterResponse>().Result;
            return result;
        }

        public static RackMasterResponse EditRack(RackMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RackMasterAPI/Edit", obj).Result.Content.ReadAsAsync<RackMasterResponse>().Result;
            return result;
        }
        #endregion

        #region API calls for RawMaterials
        public static RawMaterialsResponse GetAllRawMaterials()
        {
            var result = CommonFunctions.client.GetAsync("RawMaterialMasterAPI/GetAll").Result.Content.ReadAsAsync<RawMaterialsResponse>().Result;
            return result;
        }

        public static RawMaterialResponse GetRawMaterial(int id)
        {
            var result = CommonFunctions.client.GetAsync("RawMaterialMasterAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<RawMaterialResponse>().Result;
            return result;
        }

        public static RawMaterialResponse AddRawMaterial(RawMaterial obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RawMaterialMasterAPI/Add", obj).Result.Content.ReadAsAsync<RawMaterialResponse>().Result;
            return result;
        }

        public static RawMaterialResponse EditRawMaterial(RawMaterial obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RawMaterialMasterAPI/Edit", obj).Result.Content.ReadAsAsync<RawMaterialResponse>().Result;
            return result;
        }
        #endregion

        #region API calls for RMInventory
        public static RMInventorysResponse GetAllRMInventory()
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetAll").Result.Content.ReadAsAsync<RMInventorysResponse>().Result;
            return result;
        }

        public static RMInventorysResponse GetAllRMInventoryByRack(int id)
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetAllByRack/" + id.ToString()).Result.Content.ReadAsAsync<RMInventorysResponse>().Result;
            return result;
        }

        public static RMInventorysResponse GetAllRMInventoryByRM(int id)
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetAllByRM/" + id.ToString()).Result.Content.ReadAsAsync<RMInventorysResponse>().Result;
            return result;
        }

        public static RMInventoryResponse GetRMInventory(int id)
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<RMInventoryResponse>().Result;
            return result;
        }

        public static RMInventoryResponse AddRMInventory(RMInventory obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/Add", obj).Result.Content.ReadAsAsync<RMInventoryResponse>().Result;
            return result;
        }

        public static RMInventoryResponse EditRMInventory(RMInventory obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/Edit", obj).Result.Content.ReadAsAsync<RMInventoryResponse>().Result;
            return result;
        }
        #endregion

        #region API calls for Companies
        public static CompaniesResponse GetAllCompanies()
        {
            var result = CommonFunctions.client.GetAsync("CompanyAPI/GetAll").Result.Content.ReadAsAsync<CompaniesResponse>().Result;
            return result;
        }

        public static CompanyResponse GetCompany(int id)
        {
            var result = CommonFunctions.client.GetAsync("CompanyAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<CompanyResponse>().Result;
            return result;
        }

        public static CompanyResponse AddCompany(Company obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("CompanyAPI/Add", obj).Result.Content.ReadAsAsync<CompanyResponse>().Result;
            return result;
        }

        public static CompanyResponse EditCompany(Company obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("CompanyAPI/Edit", obj).Result.Content.ReadAsAsync<CompanyResponse>().Result;
            return result;
        }

        public static CompaniesResponse GetAllCompanyList()
        {
            var result = CommonFunctions.client.GetAsync("CompanyAPI/GetCompanyMasterList").Result.Content.ReadAsAsync<CompaniesResponse>().Result;
            return result;
        }

        #endregion

        #region API calls for CompanyLocations
        public static CompanyLocationsResponse GetAllCompanyLocations(int id)
        {
            var result = CommonFunctions.client.GetAsync("CompanyLocationAPI/GetAll/" + id.ToString()).Result.Content.ReadAsAsync<CompanyLocationsResponse>().Result;
            return result;
        }

        public static CompanyLocationResponse GetCompanyLocation(int id)
        {
            var result = CommonFunctions.client.GetAsync("CompanyLocationAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<CompanyLocationResponse>().Result;
            return result;
        }

        public static CompanyLocationResponse AddCompanyLocation(CompanyLocation obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("CompanyLocationAPI/Add", obj).Result.Content.ReadAsAsync<CompanyLocationResponse>().Result;
            return result;
        }

        public static CompanyLocationResponse EditCompanyLocation(CompanyLocation obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("CompanyLocationAPI/Edit", obj).Result.Content.ReadAsAsync<CompanyLocationResponse>().Result;
            return result;
        }
        #endregion

        #region API calls for Purchase
        public static PurchaseMastersResponse GetAllPurchases()
        {
            var result = CommonFunctions.client.GetAsync("PurchaseAPI/GetAll").Result.Content.ReadAsAsync<PurchaseMastersResponse>().Result;
            return result;
        }

        public static PurchaseMastDetailsResponse GetAllByRMID(int id)
        {
            var result = CommonFunctions.client.GetAsync("PurchaseAPI/GetAllByRMID/" + id.ToString()).Result.Content.ReadAsAsync<PurchaseMastDetailsResponse>().Result;
            return result;
        }

        public static PurchaseMasterResponse GetPurchase(int id)
        {
            var result = CommonFunctions.client.GetAsync("PurchaseAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<PurchaseMasterResponse>().Result;
            return result;
        }

        public static PurchaseMasterResponse AddPurchase(PurchaseMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("PurchaseAPI/Add", obj).Result.Content.ReadAsAsync<PurchaseMasterResponse>().Result;
            return result;
        }

        public static PurchaseMasterResponse EditPurchase(PurchaseMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("PurchaseAPI/Edit", obj).Result.Content.ReadAsAsync<PurchaseMasterResponse>().Result;
            return result;
        }

        public static PurchaseMasterResponse ValidatePONumber(string poNumber, int purchaseID)
        {
            var result = CommonFunctions.client.GetAsync("PurchaseAPI/ValidatePONumber?poNumber=" + poNumber + "&purchaseID=" + purchaseID).Result.Content.ReadAsAsync<PurchaseMasterResponse>().Result;
            return result;
        }

        #endregion

        #region API calls for PurchaseRcv
        public static PurchaseRcvMastsResponse GetAllPurchaseRcvMasts()
        {
            var result = CommonFunctions.client.GetAsync("PurchaseRcvAPI/GetAll").Result.Content.ReadAsAsync<PurchaseRcvMastsResponse>().Result;
            return result;
        }

        public static PurchaseRcvMastResponse GetNewRcv(string PONumber) //pass PO ID
        {
            var result = CommonFunctions.client.GetAsync("PurchaseRcvAPI/GetNewRcv?PONumber=" + PONumber).Result.Content.ReadAsAsync<PurchaseRcvMastResponse>().Result;
            return result;
        }

        public static PurchaseRcvMastResponse AddPurchaseRcv(PurchaseRcvMast obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("PurchaseRcvAPI/Add", obj).Result.Content.ReadAsAsync<PurchaseRcvMastResponse>().Result;
            return result;
        }

        //public static PurchaseRcvMastResponse GetPurchaseRcvMast(int id)
        //{
        //    var result = CommonFunctions.client.GetAsync("PurchaseRcvAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<PurchaseRcvMastResponse>().Result;
        //    return result;
        //}

        public static PurchaseRcvPrintResponse GetRMStackingPlan(int id) //pass PO ID
        {
            var result = CommonFunctions.client.GetAsync("PurchaseRcvAPI/GetRMStackingPlan/" + id.ToString()).Result.Content.ReadAsAsync<PurchaseRcvPrintResponse>().Result;
            return result;
        }

        #endregion

        #region API calls for Products
        public static ProductsResponse GetAllProducts()
        {
            var result = CommonFunctions.client.GetAsync("ProductMasterAPI/GetAll").Result.Content.ReadAsAsync<ProductsResponse>().Result;
            return result;
        }

        public static ProductResponse GetProduct(int id)
        {
            var result = CommonFunctions.client.GetAsync("ProductMasterAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<ProductResponse>().Result;
            return result;
        }

        public static ProductResponse AddProduct(Product obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("ProductMasterAPI/Add", obj).Result.Content.ReadAsAsync<ProductResponse>().Result;
            return result;
        }

        public static ProductResponse EditProduct(Product obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("ProductMasterAPI/Edit", obj).Result.Content.ReadAsAsync<ProductResponse>().Result;
            return result;
        }
        #endregion

        #region API calls for Sales
        public static SalesMastersResponse GetAllSales()
        {
            var result = CommonFunctions.client.GetAsync("SalesAPI/GetAll").Result.Content.ReadAsAsync<SalesMastersResponse>().Result;
            return result;
        }

        public static SalesMastDetailsResponse GetAllByProductID(int id)
        {
            var result = CommonFunctions.client.GetAsync("SalesAPI/GetAllByProductID/" + id.ToString()).Result.Content.ReadAsAsync<SalesMastDetailsResponse>().Result;
            return result;
        }

        public static SalesMasterResponse GetSales(int id, int RoleId)
        {
            var result = CommonFunctions.client.GetAsync("SalesAPI/Get?id=" + id + "&UserRoleId=" + RoleId).Result.Content.ReadAsAsync<SalesMasterResponse>().Result;
            return result;
        }

        public static ProductMasterResponse GetProductsDetails()
        {
            ProductMasterResponse result = CommonFunctions.client.GetAsync("SalesAPI/GetProductsDetails").Result.Content.ReadAsAsync<ProductMasterResponse>().Result;
            return result;
        }
        public static SalesSatusMastersResponse GetSalesStatusMasterDetails()
        {
            SalesSatusMastersResponse result = CommonFunctions.client.GetAsync("SalesAPI/GetSalesStatusMasterData").Result.Content.ReadAsAsync<SalesSatusMastersResponse>().Result;
            return result;
        }

        public static SalesMasterResponse GetCompanyLocationDetails(int id)
        {
            var result = CommonFunctions.client.GetAsync("SalesAPI/GetCompanyLocationDetails/" + id.ToString()).Result.Content.ReadAsAsync<SalesMasterResponse>().Result;
            return result;
        }

        public static SalesMasterResponse AddSales(SalesMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("SalesAPI/Add", obj).Result.Content.ReadAsAsync<SalesMasterResponse>().Result;
            return result;
        }

        public static SalesMasterResponse EditSales(SalesMaster obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("SalesAPI/Edit", obj).Result.Content.ReadAsAsync<SalesMasterResponse>().Result;
            return result;
        }

        public static LookUpMasterResponse GetLookUpData(string LookUpTypeName)
        {
            var result = CommonFunctions.client.GetAsync("SalesAPI/GetLookUpDataFromTypeName?LookUpTypeName=" + LookUpTypeName).Result.Content.ReadAsAsync<LookUpMasterResponse>().Result;
            return result;
        }
        public static RackMastersResponse GetRackMastersData()
        {
            var result = CommonFunctions.client.GetAsync("SalesAPI/GetRackMasterData").Result.Content.ReadAsAsync<RackMastersResponse>().Result;
            return result;
        }

        #endregion

        #region API calls for Dispatch
        public static SalesDispatchMasterResponse GetAllDispatch01(int id)
        {
            var result = CommonFunctions.client.GetAsync("DispatchAPI/GetAll/" + id.ToString()).Result.Content.ReadAsAsync<SalesDispatchMasterResponse>().Result;
            return result;
        }

        public static ResponseObj SetNewDispatchBlockValues(int iSalesDetailsID, int iBlockedQty, int iToDispatchQty, int iToProduceQty, int userID)
        {
            var parameters = new Dictionary<string, int> { { "iSalesDetailsID", iSalesDetailsID }, { "iBlockedQty", iBlockedQty }, { "iToDispatchQty", iToDispatchQty }, { "iToProduceQty", iToProduceQty }, { "UserID", userID } };
            var result = CommonFunctions.client.PostAsJsonAsync("DispatchAPI/SetNewDispatchBlockValues", parameters).Result.Content.ReadAsAsync<ResponseObj>().Result;
            return result;
        }

        public static SalesDispatchDetailMasterResponse GetSalesDispatchDetailData(int salesId)
        {
            var result = CommonFunctions.client.GetAsync("DispatchAPI/GetSalesDispatchDetailData?salesId=" + salesId.ToString()).Result.Content.ReadAsAsync<SalesDispatchDetailMasterResponse>().Result;
            return result;
        }

        public static SalesDispatchDetailMasterResponse SaveSalesDispatchDetailData(SalesDispatchDetailMaster salesDispatchDetailMaster)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("DispatchAPI/SaveSalesDispatchDetailData", salesDispatchDetailMaster).Result.Content.ReadAsAsync<SalesDispatchDetailMasterResponse>().Result;
            return result;
        }

        //public static SalesMastDetailsResponse GetAllByProductID(int id)
        //{
        //    var result = CommonFunctions.client.GetAsync("SalesAPI/GetAllByProductID/" + id.ToString()).Result.Content.ReadAsAsync<SalesMastDetailsResponse>().Result;
        //    return result;
        //}

        //public static SalesMasterResponse GetSales(int id)
        //{
        //    var result = CommonFunctions.client.GetAsync("SalesAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<SalesMasterResponse>().Result;
        //    return result;
        //}

        //public static SalesMasterResponse AddSales(SalesMaster obj)
        //{
        //    var result = CommonFunctions.client.PostAsJsonAsync("SalesAPI/Add", obj).Result.Content.ReadAsAsync<SalesMasterResponse>().Result;
        //    return result;
        //}

        //public static SalesMasterResponse EditSales(SalesMaster obj)
        //{
        //    var result = CommonFunctions.client.PostAsJsonAsync("SalesAPI/Edit", obj).Result.Content.ReadAsAsync<SalesMasterResponse>().Result;
        //    return result;
        //}

        #endregion

        #region API calls for Production
        public static ProductionProgramesResponse GetAllProduction(int id)
        {
            var result = CommonFunctions.client.GetAsync("ProductionAPI/GetAll/" + id.ToString()).Result.Content.ReadAsAsync<ProductionProgramesResponse>().Result;
            return result;
        }
        public static ProductionProgramesResponse GetProductionBatch(int ProductionProgramID)
        {
            try
            {
                var result = CommonFunctions.client.GetAsync("ProductionAPI/GetProductionBatch?ProductionProgramID=" + ProductionProgramID.ToString()).Result.Content.ReadAsAsync<ProductionProgramesResponse>().Result;
                return result;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static ResponseObj StartProduction(int iProductionProgramID)
        {
            var parameters = new Dictionary<string, int> { { "iProductionProgramID", iProductionProgramID } };
            var result = CommonFunctions.client.PostAsJsonAsync("ProductionAPI/StartProduction", parameters).Result.Content.ReadAsAsync<ResponseObj>().Result;
            return result;
        }

        public static ResponseObj StartProduction(Dictionary<string, int> param)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("ProductionAPI/StartProduction", param).Result.Content.ReadAsAsync<ResponseObj>().Result;
            return result;
        }

        public static ResponseObj UpdateProduction(int iProductionProgramID, int iProducedNow, int iBatchId)
        {
            var parameters = new Dictionary<string, int> { { "iProductionProgramID", iProductionProgramID }, { "iProducedNow", iProducedNow }, { "iBatchId", iBatchId } };
            var result = CommonFunctions.client.PostAsJsonAsync("ProductionAPI/UpdateProduction", parameters).Result.Content.ReadAsAsync<ResponseObj>().Result;
            return result;
        }

        public static ProductionPlanResponse GetProductionPlan(int id)
        {
            var result = CommonFunctions.client.GetAsync("ProductionAPI/GetProductionPlan/" + id.ToString()).Result.Content.ReadAsAsync<ProductionPlanResponse>().Result;
            return result;
        }

        public static ProductRackingPlanResponse GetRackingPlan(int id)
        {
            var result = CommonFunctions.client.GetAsync("ProductionAPI/GetRackingPlan/" + id.ToString()).Result.Content.ReadAsAsync<ProductRackingPlanResponse>().Result;
            return result;
        }

        #endregion

        #region API calls for External
        public static ResponseObj POResponse(object id)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("ExternalAPI/POResponse", id).Result.Content.ReadAsAsync<ResponseObj>().Result;
            return result;
        }

        #endregion

        #region API calls for Mould
        public static MouldMastersResponse GetAllMoulds()
        {
            var result = CommonFunctions.client.GetAsync("MouldAPI/GetAll").Result.Content.ReadAsAsync<MouldMastersResponse>().Result;
            return result;
        }

        public static MouldMasterResponse GetMoulds(int id = 0)
        {
            var result = CommonFunctions.client.GetAsync("MouldAPI/Get/" + id.ToString()).Result.Content.ReadAsAsync<MouldMasterResponse>().Result;
            return result;
        }

        public static MouldMasterResponse AddMoulds(Mould obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("MouldAPI/Add", obj).Result.Content.ReadAsAsync<MouldMasterResponse>().Result;
            return result;
        }

        public static MouldMasterResponse EditMoulds(Mould obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("MouldAPI/Edit", obj).Result.Content.ReadAsAsync<MouldMasterResponse>().Result;
            return result;
        }

        public static MouldMastersResponse MapMouldMachine(List<KPILib.Models.MouldMachineMapping> machineMouldMapping)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("MouldAPI/MapMouldMachine", machineMouldMapping).Result.Content.ReadAsAsync<MouldMastersResponse>().Result;
            return result;
        }
        public static MouldMastersResponse GetMouldMachineMappedData(int MouldId = 0)
        {
            if (MouldId == 0)
            {
                return new MouldMastersResponse()
                {
                    Response = new ResponseObj()
                    {
                        ResponseCode = 200
                    }
                };
            }
            var result = CommonFunctions.client.GetAsync("MouldAPI/GetMouldMachineMappedData?MouldId=" + MouldId).Result.Content.ReadAsAsync<MouldMastersResponse>().Result;
            return result;
        }
        #endregion    

        #region API calls for Machine
        public static MachineMastersResponse GetAllMachines()
        {
            var result = CommonFunctions.client.GetAsync("MachineAPI/GetAll").Result.Content.ReadAsAsync<MachineMastersResponse>().Result;
            return result;
        }
        public static MachineMasterResponse GetMachineData(int MachineId = 0)
        {
            var result = CommonFunctions.client.GetAsync("MachineAPI/AddMachine?MachineId=" + MachineId).Result.Content.ReadAsAsync<MachineMasterResponse>().Result;
            return result;
        }
        public static MachineMasterResponse AddUpdateMachineData(MachineMasterModel machine)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("MachineAPI/AddMachine", machine).Result.Content.ReadAsAsync<MachineMasterResponse>().Result;
            return result;
        }
        public static MachineMasterResponse DeleteMachineData(MachineMasterModel machine)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("MachineAPI/DeleteMachine", machine).Result.Content.ReadAsAsync<MachineMasterResponse>().Result;
            return result;
        }
        public static MachineTypeMasterResponse GetMachineTypeMasterList()
        {
            var result = CommonFunctions.client.GetAsync("MachineAPI/GetMachineTypeMasterList").Result.Content.ReadAsAsync<MachineTypeMasterResponse>().Result;
            return result;
        }
        public static MachineStatusMasterResponse GetMachineStatusMasterList()
        {
            var result = CommonFunctions.client.GetAsync("MachineAPI/GetMachineStatusMasterList").Result.Content.ReadAsAsync<MachineStatusMasterResponse>().Result;
            return result;
        }
        public static MachineMouldMappingResponse GetAllMachineMouldMappedData(string MapType = "Machine")
        {
            var result = CommonFunctions.client.GetAsync("MachineAPI/GetAllMachineMouldMappedData?MapType=" + MapType).Result.Content.ReadAsAsync<MachineMouldMappingResponse>().Result;
            return result;
        }

        public static MachineMouldMappingResponse GetMachineMouldMappedData(int MachineId = 0)
        {
            if (MachineId == 0)
            {
                return new MachineMouldMappingResponse()
                {
                    Response = new ResponseObj()
                    {
                        ResponseCode = 200
                    }
                };
            }
            var result = CommonFunctions.client.GetAsync("MachineAPI/GetMachineMouldMappedData?MachineId=" + MachineId).Result.Content.ReadAsAsync<MachineMouldMappingResponse>().Result;
            return result;
        }
        public static MachineMouldMappingResponse MapMachineMould(List<KPILib.Models.MachineMouldMapping> machineMouldMapping)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("MachineAPI/MapMachineMould", machineMouldMapping).Result.Content.ReadAsAsync<MachineMouldMappingResponse>().Result;
            return result;
        }
        public static MachineMouldMappingResponse DeleteMachineMouldMapping(int MachineMouldMappingId = 0, int UserId = 0)
        {
            var result = CommonFunctions.client.PostAsync("MachineAPI/DeleteMachineMouldMapping?MachineMouldMappingId=" + MachineMouldMappingId + "&UserId=" + UserId, null).Result.Content.ReadAsAsync<MachineMouldMappingResponse>().Result;
            return result;
        }
        public static MachineMastersResponse GetMachineMasterData(int MachineId = 0)
        {
            var result = CommonFunctions.client.GetAsync("MachineAPI/GetMachineMasterData?MachineId=" + MachineId).Result.Content.ReadAsAsync<MachineMastersResponse>().Result;
            return result;
        }
        public static MouldMastersResponse GetMouldMasterListData(int MouldId = 0)
        {
            var result = CommonFunctions.client.GetAsync("MouldAPI/GetMouldMasterListData?MouldId=" + MouldId).Result.Content.ReadAsAsync<MouldMastersResponse>().Result;
            return result;
        }

        #endregion    

        #region API calls for Packing
        public static SalesDispatchMasterResponse GetAllPacking(int id)
        {
            var result = CommonFunctions.client.GetAsync("PackingAPI/GetAll/" + id.ToString()).Result.Content.ReadAsAsync<SalesDispatchMasterResponse>().Result;
            return result;
        }
        public static PackingDispatchMasterResponse GetPackingDetails(int id)
        {
            //var result = CommonFunctions.client.PostAsJsonAsync("PackingAPI/GetPackingDetails/", SalesIDProductID).Result.Content.ReadAsAsync<PackingDispatchMasterResponse>().Result;

            var result = CommonFunctions.client.GetAsync("PackingAPI/GetPackingDetails/" + id.ToString()).Result.Content.ReadAsAsync<PackingDispatchMasterResponse>().Result;
            return result;
        }
        #endregion

        #region Login Manager

        public static UserMasterResponse LoginUserResponse(UserMaster userMaster)
        {
            string url = "LoginAPI/UserLogin";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(userMaster), Encoding.UTF8, "application/json");
            UserMasterResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
            return result;
        }

        public static UserMasterResponse LogOutUserResponse(string JWTTokenKey)
        {
            try
            {
                string url = $"LoginAPI/LogOutUser?JWTTokenKey={JWTTokenKey}";
                UserMasterResponse result = CommonFunctions.client.PostAsync(url, null).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
                return result;
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }

        #endregion

        #region UserMaster CRUD

        public static UserMasterResponse GetUserMasterListResponse(int UserId = 0)
        {
            string url = $"UserMasterAPI/GetUsers?UserId={UserId}";
            UserMasterResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
            return result;
        }

        public static UserMasterResponse GetUserMasterResponse(int UserId = 0)
        {
            string url = $"UserMasterAPI/GetUserMasterData?UserId={UserId}";
            UserMasterResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
            return result;
        }
        public static UserMasterResponse ValidateUsername(string Username, int UserId = 0)
        {
            string url = $"UserMasterAPI/ValidateUsername?Username={Username}&UserId={UserId}";
            UserMasterResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
            return result;
        }

        public static UserMasterResponse AddUpdateUserResponse(UserMaster userMaster, int SessionUserID)
        {
            string url = $"UserMasterAPI/AddUser?SessionUserID={SessionUserID}";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(userMaster), Encoding.UTF8, "application/json");
            UserMasterResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
            return result;
        }

        public static UserMasterResponse DeleteUserResponse(int UserId, int SessionUserId)
        {
            string url = $"UserMasterAPI/DeleteUser?UserId={UserId}&SessionUserId={SessionUserId}";
            UserMasterResponse result = CommonFunctions.client.PostAsync(url, null).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
            return result;
        }

        public static UserMasterResponse ForgotPasswordResponse(UserMaster userMaster)
        {
            string url = $"UserMasterAPI/ForgotPassword";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(userMaster), Encoding.UTF8, "application/json");
            UserMasterResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<UserMasterResponse>().Result;
            return result;
        }

        #endregion

        #region RoleMaster CRUD

        public static RoleMasterResponse GetRoleResponse(int RoleId = 0)
        {
            string url = $"UserMasterAPI/GetRoles?RoleId={RoleId}";
            RoleMasterResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<RoleMasterResponse>().Result;
            return result;
        }

        public static RoleMasterResponse AddUpdateRoleResponse(RoleMaster roleMaster)
        {
            string url = "UserMasterAPI/AddRoles";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(roleMaster), Encoding.UTF8, "application/json");
            RoleMasterResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<RoleMasterResponse>().Result;
            return result;
        }

        public static RoleMasterResponse ValidateRoleNameResponse(RoleMaster roleMaster)
        {
            string url = "UserMasterAPI/ValidateRoleName";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(roleMaster), Encoding.UTF8, "application/json");
            RoleMasterResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<RoleMasterResponse>().Result;
            return result;
        }

        public static RoleMasterResponse DeleteRoleResponse(int RoleId = 0, int UserId = 0)
        {
            string url = $"UserMasterAPI/DeleteRole?RoleId={RoleId}&UserId={UserId}";
            RoleMasterResponse result = CommonFunctions.client.PostAsync(url, null).Result.Content.ReadAsAsync<RoleMasterResponse>().Result;
            return result;
        }

        public static RoleRightsResponse RoleRightsResponse(int RoleId)
        {
            string url = $"UserMasterAPI/GetAssignedRoleRightsToMenu?RoleId={RoleId}";
            RoleRightsResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<RoleRightsResponse>().Result;
            return result;
        }

        public static MenuMasterResponse GetParentMenuMasterData()
        {
            string url = $"UserMasterAPI/GetParentMenuMasterData";
            MenuMasterResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<MenuMasterResponse>().Result;
            return result;
        }

        public static MenuMasterResponse GetChildMenuMasterData(int ParentMenuID)
        {
            string url = $"UserMasterAPI/GetChildMenuMasterData?ParentMenuID=" + ParentMenuID;
            MenuMasterResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<MenuMasterResponse>().Result;
            return result;
        }

        public static RoleRightsResponse SaveRoleRightsForMenu(List<RoleRights> roleRights, int MenuParentID, int RoleID, int UserId)
        {
            string url = $"UserMasterAPI/SaveRoleRightsForMenu?MenuParentID={MenuParentID}&RoleID={RoleID}&UserId={UserId}";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(roleRights), Encoding.UTF8, "application/json");
            RoleRightsResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<RoleRightsResponse>().Result;
            return result;
        }

        #endregion

        #region Get data for ddl

        public static RawMaterialsResponse GetRawMaterialData()
        {
            string url = $"ProductMasterAPI/GetRawMaterialData";
            RawMaterialsResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<RawMaterialsResponse>().Result;
            return result;
        }

        public static UnitResponse GetUnitData()
        {
            string url = $"ProductMasterAPI/GetUnitData";
            UnitResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<UnitResponse>().Result;
            return result;
        }

        public static TagColorMasterModelResponse GetTagColorMasterData()
        {
            string url = $"RMInventoryAPI/GetTagColourMasterList";
            TagColorMasterModelResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<TagColorMasterModelResponse>().Result;
            return result;
        }

        #endregion

        #region API Call for Vendor Master

        public static VendorMasterModelResponse GetAllVendorData()
        {
            string url = $"VendorMasterAPI/GetAll";
            VendorMasterModelResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<VendorMasterModelResponse>().Result;
            return result;
        }

        public static VendorMasterModelResponse GetVendorMasterData(int id = 0)
        {
            string url = $"VendorMasterAPI/GetVendor/{id}";
            VendorMasterModelResponse result = CommonFunctions.client.GetAsync(url).Result.Content.ReadAsAsync<VendorMasterModelResponse>().Result;
            return result;
        }

        public static VendorMasterModelResponse AddUpdateVendorDataResponse(VendorMasterModel vendorMaster)
        {
            string url = $"VendorMasterAPI/AddNew";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(vendorMaster), Encoding.UTF8, "application/json");
            VendorMasterModelResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<VendorMasterModelResponse>().Result;
            return result;
        }

        public static VendorMasterModelResponse DeleteVendorDataResponse(VendorMasterModel vendorMaster)
        {
            string url = $"VendorMasterAPI/DeleteVendor";
            HttpContent content = new StringContent(JsonConvert.SerializeObject(vendorMaster), Encoding.UTF8, "application/json");
            VendorMasterModelResponse result = CommonFunctions.client.PostAsync(url, content).Result.Content.ReadAsAsync<VendorMasterModelResponse>().Result;
            return result;
        }

        #endregion

        #region Location Master
        public static LocationMasterResponse GetAllLocations()
        {
            var result = CommonFunctions.client.GetAsync("LocationMasterAPI/GetAllLocation").Result.Content.ReadAsAsync<LocationMasterResponse>().Result;
            return result;
        }

        public static LocationMasterResponse GetLocation(int LocationId)
        {
            var result = CommonFunctions.client.GetAsync("LocationMasterAPI/GetLocation?LocationId=" + LocationId.ToString()).Result.Content.ReadAsAsync<LocationMasterResponse>().Result;
            return result;
        }

        public static LocationMasterResponse SaveLocation(LocationMasterModel obj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("LocationMasterAPI/SaveLocation", obj).Result.Content.ReadAsAsync<LocationMasterResponse>().Result;
            return result;
        }

        public static LocationMasterResponse DeleteLocation(int LocationId)
        {
            var result = CommonFunctions.client.DeleteAsync($"LocationMasterAPI/DeleteLocation?LocationId={LocationId}").Result.Content.ReadAsAsync<LocationMasterResponse>().Result;
            return result;
        }
        public static LocationMasterResponse GetListOfLocationMasterData()
        {
            var result = CommonFunctions.client.GetAsync("LocationMasterAPI/GetLocationMasterData").Result.Content.ReadAsAsync<LocationMasterResponse>().Result;
            return result;
        }
        public static LocationMasterResponse ValidateLocationName(LocationMasterModel locationMasterObj)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("LocationMasterAPI/ValidateLocationName", locationMasterObj).Result.Content.ReadAsAsync<LocationMasterResponse>().Result;
            return result;
        }

        #endregion

        #region RM Inventory API Calls
        #region Master Batch 

        public static RMInventoryMasterBatchResponse GetAllMasterBatch()
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetAllMasterBatch").Result.Content.ReadAsAsync<RMInventoryMasterBatchResponse>().Result;
            return result;
        }

        public static RMInventoryMasterBatchResponse GetMasterBatchData(int BatchId = 0)
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetMasterBatchData?BatchId=" + BatchId.ToString()).Result.Content.ReadAsAsync<RMInventoryMasterBatchResponse>().Result;
            return result;
        }

        public static RMInventoryMasterBatchResponse SaveMasterBatch(RMInventoryMasterBatchModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/SaveMasterBatch", data).Result.Content.ReadAsAsync<RMInventoryMasterBatchResponse>().Result;
            return result;
        }

        public static RMInventoryMasterBatchResponse DeleteMasterBatch(RMInventoryMasterBatchModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/DeleteMasterBatch", data).Result.Content.ReadAsAsync<RMInventoryMasterBatchResponse>().Result;
            return result;
        }

        #endregion

        #region Packaging Bags

        public static RMInventoryPackageBagsModelResponse GetAllPackagBags()
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetAllPackagBags").Result.Content.ReadAsAsync<RMInventoryPackageBagsModelResponse>().Result;
            return result;
        }

        public static RMInventoryPackageBagsModelResponse GetPackagBagData(int PackagBagId = 0)
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetPackagBagData?PackagBagId=" + PackagBagId.ToString()).Result.Content.ReadAsAsync<RMInventoryPackageBagsModelResponse>().Result;
            return result;
        }

        public static RMInventoryPackageBagsModelResponse SavePackageBags(RMInventoryPackageBagsModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/SavePackageBags", data).Result.Content.ReadAsAsync<RMInventoryPackageBagsModelResponse>().Result;
            return result;
        }

        public static RMInventoryPackageBagsModelResponse DeletePackageBags(RMInventoryPackageBagsModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/DeletePackageBags", data).Result.Content.ReadAsAsync<RMInventoryPackageBagsModelResponse>().Result;
            return result;
        }

        #endregion

        #region Finished Good

        public static RMInventoryFinishedGoodResponse GetAllFinishedGood()
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetAllFinishedGood").Result.Content.ReadAsAsync<RMInventoryFinishedGoodResponse>().Result;
            return result;
        }

        public static RMInventoryFinishedGoodResponse GetFinishedGoodData(int FinishedGoodId = 0)
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetFinishedGoodData?FinishedGoodId=" + FinishedGoodId.ToString()).Result.Content.ReadAsAsync<RMInventoryFinishedGoodResponse>().Result;
            return result;
        }

        public static RMInventoryFinishedGoodResponse SaveFinishedGood(RMInventoryFinishedGoodModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/SaveFinishedGood", data).Result.Content.ReadAsAsync<RMInventoryFinishedGoodResponse>().Result;
            return result;
        }

        public static RMInventoryFinishedGoodResponse DeleteFinishedGood(RMInventoryFinishedGoodModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/DeleteFinishedGood", data).Result.Content.ReadAsAsync<RMInventoryFinishedGoodResponse>().Result;
            return result;
        }

        #endregion

        #region Rejection Material

        public static RMInventoryRejectionMaterialResponse GetAllRejectionMaterials()
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetAllRejectionMaterial").Result.Content.ReadAsAsync<RMInventoryRejectionMaterialResponse>().Result;
            return result;
        }

        public static RMInventoryRejectionMaterialResponse GetRejectionMaterialData(int RejectionMaterialId = 0)
        {
            var result = CommonFunctions.client.GetAsync("RMInventoryAPI/GetRejectionMaterialData?RejectionMaterialId=" + RejectionMaterialId.ToString()).Result.Content.ReadAsAsync<RMInventoryRejectionMaterialResponse>().Result;
            return result;
        }

        public static RMInventoryRejectionMaterialResponse SaveRejectionMaterial(RMInventoryRejectionMaterialModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/SaveRejectionMaterial", data).Result.Content.ReadAsAsync<RMInventoryRejectionMaterialResponse>().Result;
            return result;
        }

        public static RMInventoryRejectionMaterialResponse DeleteRejectionMaterial(RMInventoryRejectionMaterialModel data)
        {
            var result = CommonFunctions.client.PostAsJsonAsync("RMInventoryAPI/DeleteRejectionMaterial", data).Result.Content.ReadAsAsync<RMInventoryRejectionMaterialResponse>().Result;
            return result;
        }

        #endregion

        #endregion
    }
}