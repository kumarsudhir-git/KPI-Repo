using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_Products")]
    public class ProductMasterAPIController : CCSPLBaseAPIController
    {
        #region ProductMaster

        // GET api/values
        public IHttpActionResult GetAll()
        {
            var returnValue = new ProductsResponse();

            try
            {
                var data = db.ProductMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.ProductName).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<ProductMaster, KPILib.Models.Product>(obj);
                    o.UOM = obj.UOMMaster.UnitsName;
                    //o.ProductCategory = obj.ProductCategoryMaster.ProductCategory;
                    o.Mould = obj.MouldMaster.MouldName;
                    //o.RawMaterial = obj.RawMaterialMaster.RawMaterialName;
                    o.InStock = (int)obj.ProdReadyStoreds.Sum(x => x.Qty);
                    o.Reserved = (int)obj.SalesDetails.Where(y => y.ProductID == obj.ProductID).Sum(x => x.QtyBlocked + x.QtyToDispatch);
                    //o.Ordered = (int)obj.PurchaseDetails.Where(x => x.PurchaseMaster.PurchaseStatusID < 30 && x.Qty-x.RcvdQty > 0).Sum(x => x.Qty - x.RcvdQty);   

                    returnValue.data.Add(o);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                CommonLogger.Error(ex, ex.Message);
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var returnValue = new ProductResponse();

            try
            {
                #region get all UOM for ddl
                var UOMs = db.UOMMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.UnitsName).ToList();

                List<UOM> uoms = new List<UOM>();
                foreach (var obj in UOMs)
                {
                    var o = mapper.Map<UOMMaster, KPILib.Models.UOM>(obj);
                    uoms.Add(o);
                }
                #endregion

                #region get all ProductCategories for ddl
                var ProductCategories = db.ProductCategoryMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.ProductCategory).ToList();

                List<ProductCategory> prodcats = new List<ProductCategory>();
                foreach (var obj in ProductCategories)
                {
                    var o = mapper.Map<ProductCategoryMaster, KPILib.Models.ProductCategory>(obj);
                    o.ProductCategoryName = obj.ProductCategory;
                    prodcats.Add(o);
                }
                #endregion

                #region get all MouldMasters for ddl
                var Moulds = db.MouldMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.MouldName).ToList();

                List<Mould> moulds = new List<Mould>();
                foreach (var obj in Moulds)
                {
                    var o = mapper.Map<MouldMaster, KPILib.Models.Mould>(obj);
                    moulds.Add(o);
                }
                #endregion

                #region get all RawMaterialMasters for ddl
                var RawMaterials = db.RawMaterialMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RawMaterialName).ToList();

                List<RawMaterial> rawmats = new List<RawMaterial>();
                foreach (var obj in RawMaterials)
                {
                    var o = mapper.Map<RawMaterialMaster, KPILib.Models.RawMaterial>(obj);
                    rawmats.Add(o);
                }
                #endregion

                #region ProductRawMaterialMapping

                List<ProductRawMaterialMapping> PrdctRwMatMap = db.ProductRawMaterialMappings.Where(x => x.ProductID == id && !x.IsDeleted).ToList();

                List<KPILib.Models.ProductRawMaterialMapping> prdctRawmats = mapper.Map<List<ProductRawMaterialMapping>, List<KPILib.Models.ProductRawMaterialMapping>>(PrdctRwMatMap);

                #endregion


                var data = db.ProductMasters.SingleOrDefault(x => x.ProductID == id);
                if (data != null)
                {
                    var o = mapper.Map<ProductMaster, KPILib.Models.Product>(data);

                    o.UOM = data.UOMMaster.UnitsName;
                    o.UOMs = uoms;

                    //o.ProductCategory = data.ProductCategoryMaster.ProductCategory;
                    //o.ProductCategories = prodcats;

                    o.Mould = data.MouldMaster.MouldName;
                    o.Moulds = moulds;

                    //o.RawMaterial = data.RawMaterialMaster.RawMaterialName;
                    o.RawMaterials = rawmats;

                    o.productRawMaterialMappings = prdctRawmats;

                    returnValue.data = o;
                }
                else
                {
                    var o = new KPILib.Models.Product() { UOMs = uoms, Moulds = moulds, RawMaterials = rawmats, productRawMaterialMappings = prdctRawmats };
                    returnValue.data = o;
                }
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Add(KPILib.Models.Product data)
        {
            var returnValue = new ProductResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.Product, ProductMaster>(data);

                //### set non-nullable default values
                o.ProductCategoryID = 103;  //TODO: Default ProductCategory
                o.UOMID = 101;              //TODO: Default UOM
                o.ConversionUOMID = 101;    //TODO: Default UOM

                o.AddedOn = DateTime.Now;
                o.LastModifiedOn = DateTime.Now;

                o.ProductRawMaterialMappings.ForEach(z =>
                {
                    z.AddedOn = DateTime.Now;
                });

                db.ProductMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.ProductID = o.ProductID;

                if (!AddUpdateproductRawMaterialMappings(data))
                {
                    returnValue.Response.ResponseMsg = "Error while saving ProductRawMaterialMapping";
                }
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(KPILib.Models.Product data)
        {
            var returnValue = new ProductResponse();

            try
            {
                var o = db.ProductMasters.SingleOrDefault(x => x.ProductID == data.ProductID);
                if (o != null)
                {
                    o.ProductName = data.ProductName;
                    o.Description = data.Description;
                    o.UOMID = 101; //TODO: Default UOM              //data.UOMID;
                    o.IsDiscontinued = data.IsDiscontinued;
                    o.LastModifiedOn = DateTime.Now;

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    #region ProductRawMaterialMapping

                    AddUpdateproductRawMaterialMappings(data);

                    #endregion
                }

                returnValue.data = data;
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }


        public IHttpActionResult Delete(KPILib.Models.Product data)
        {
            var returnValue = new ProductResponse();

            try
            {
                var o = db.ProductMasters.SingleOrDefault(x => x.ProductID == data.ProductID);
                if (o != null)
                {
                    o.IsDiscontinued = true;

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }
                AddUpdateproductRawMaterialMappings(data);

                returnValue.data = null;
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }


        public IHttpActionResult Delete(int id)
        {
            var returnValue = new ProductResponse();

            try
            {
                var o = db.ProductMasters.SingleOrDefault(x => x.ProductID == id);
                if (o != null)
                {
                    o.IsDiscontinued = true;

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();
                }

                returnValue.data = null;
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        [HttpGet]
        public IHttpActionResult GetRawMaterialData()
        {
            RawMaterialsResponse rawMaterialsResponse = new RawMaterialsResponse();
            try
            {
                rawMaterialsResponse.data = getRawMaterial();
                rawMaterialsResponse.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                rawMaterialsResponse.Response.ResponseCode = 500;
                rawMaterialsResponse.Response.ResponseMsg = $"Internal Server Error {ex.Message}";
            }
            return Json(rawMaterialsResponse);
        }

        [HttpGet]
        public IHttpActionResult GetUnitData()
        {
            UnitResponse unitResponse = new UnitResponse();
            try
            {
                unitResponse.data = getUnitTypeData();
                unitResponse.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                unitResponse.Response.ResponseCode = 500;
                unitResponse.Response.ResponseMsg = $"Internal Server Error {ex.Message}";
            }
            return Json(unitResponse);
        }

        private List<KPILib.Models.RawMaterial> getRawMaterial()
        {
            #region get all RawMaterialMasters for ddl

            var RawMaterials = db.RawMaterialMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RawMaterialName).ToList();

            List<KPILib.Models.RawMaterial> rawmats = mapper.Map<List<RawMaterialMaster>, List<KPILib.Models.RawMaterial>>(RawMaterials);

            return rawmats;

            #endregion
        }

        private List<KPILib.Models.Unit> getUnitTypeData()
        {
            #region get all Unit for ddl

            var unitTypeData = db.Units.OrderBy(x => x.UnitName).ToList();

            List<KPILib.Models.Unit> units = mapper.Map<List<Unit>, List<KPILib.Models.Unit>>(unitTypeData);

            return units;

            #endregion
        }

        private bool AddUpdateproductRawMaterialMappings(KPILib.Models.Product data)
        {
            try
            {
                List<ProductRawMaterialMapping> productRawMaterialMappings = (from PRMMap in db.ProductRawMaterialMappings
                                                                              where PRMMap.ProductID == data.ProductID
                                                                              select PRMMap).ToList();

                if (productRawMaterialMappings != null && productRawMaterialMappings.Count > 0)
                {
                    productRawMaterialMappings.ForEach(z =>
                    {
                        z.IsDeleted = true;
                        z.LastModifiedOn = DateTime.Now;
                        db.Entry(z).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    });
                }

                if (data.productRawMaterialMappings != null && data.productRawMaterialMappings.Count > 0)
                {
                    data.productRawMaterialMappings = data.productRawMaterialMappings.Where(z => z.RawMaterialID != 0).ToList();

                    List<ProductRawMaterialMapping> ProductRawMaterialMappingObj = mapper.Map<List<KPILib.Models.ProductRawMaterialMapping>, List<ProductRawMaterialMapping>>(data.productRawMaterialMappings);

                    ProductRawMaterialMappingObj.ForEach(x =>
                    {
                        x.IsDeleted = false;
                        x.ProductID = data.ProductID;
                        //x.RMGradeUsed = data.RMGradeUsed;
                        //x.UnitType = x.UnitType == 0 ? 1 : x.UnitType;
                        x.AddedOn = DateTime.Now;
                    });

                    db.ProductRawMaterialMappings.AddRange(ProductRawMaterialMappingObj);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}
