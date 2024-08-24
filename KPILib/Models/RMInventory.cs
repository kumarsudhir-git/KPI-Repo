using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class RMInventory
    {
        [Key]
        public int RMInventoryID { get; set; }

        [Required(ErrorMessage = "Required")]
        public int PalletID { get; set; }

        [Display(Name = "Pallet No")]
        public string PalletNo { get; set; }
        public List<PalletMaster> Pallets { get; set; }

        [Required(ErrorMessage = "Required")]
        public int RawMaterialID { get; set; }

        [Display(Name = "Raw Material")]
        public string RawMaterialName { get; set; }
        public List<RawMaterial> RawMaterials { get; set; }

        public List<TagColor> TagColours { get; set; }

        [Display(Name = "Qty (in Kgs)")]
        public decimal Qty { get; set; } //bags

        [Display(Name = "Qty (Bags)")]
        public decimal QtyBags { get; set; } //kgs

        [Display(Name = "Open Bags")]
        public decimal QtyOpened { get; set; } //opened

        [Display(Name = "Tag Colour")]
        public int TagColourID { get; set; }
        public string TagColour { get; set; }

        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public int? AddedBy { get; set; }
        public string AddedByName { get; set; }
        public int? ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        [Display(Name = "Min Order Level")]
        public int? MinOrderlevel { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
    }

    public class RMInventoryResponse
    {
        public RMInventory data { get; set; }
        public ResponseObj Response { get; set; }

        public RMInventoryResponse()
        {
            this.data = new RMInventory();
            this.Response = new ResponseObj();
        }
    }

    public class RMInventorysResponse
    {
        public List<RMInventory> data { get; set; }
        public ResponseObj Response { get; set; }

        public RMInventorysResponse()
        {
            this.data = new List<RMInventory>();
            this.Response = new ResponseObj();
        }
    }

    public class TagColor
    {
        [Key]
        [Display(Name = "Tag Colour")]
        public int TagColourID { get; set; }

        [Display(Name = "Tag Colour")]
        public string TagColour { get; set; }

    }
    public class RMInventoryMasterBatchResponse
    {
        public RMInventoryMasterBatchModel rMInventoryMasterBatchModel { get; set; }
        public List<RMInventoryMasterBatchModel> data { get; set; }
        public ResponseObj Response { get; set; }

        public RMInventoryMasterBatchResponse()
        {
            rMInventoryMasterBatchModel = new RMInventoryMasterBatchModel();
            this.data = new List<RMInventoryMasterBatchModel>();
            this.Response = new ResponseObj();
        }
    }

    public class RMInventoryMasterBatchModel
    {
        [Key]
        public int MasterBatchId { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Code No")]
        public string CodeNo { get; set; }
        public string Colour { get; set; }
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        [Display(Name = "Qty In Stock")]
        public int? QtyInStock { get; set; }
        [Display(Name = "Min Order Level")]
        public int? MinOrderLevel { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
        public int AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime AddedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByName { get; set; }
    }

    public class RMInventoryPackageBagsModelResponse
    {
        public RMInventoryPackageBagsModel rMInventoryPackageBagsModel { get; set; }
        public List<RMInventoryPackageBagsModel> data { get; set; }
        public ResponseObj Response { get; set; }

        public RMInventoryPackageBagsModelResponse()
        {
            rMInventoryPackageBagsModel = new RMInventoryPackageBagsModel();
            this.data = new List<RMInventoryPackageBagsModel>();
            this.Response = new ResponseObj();
        }
    }

    public class RMInventoryPackageBagsModel
    {
        [Key]
        public int PackageBagId { get; set; }
        [Required(ErrorMessage = "Required")]
        public int? Size { get; set; }
        [Display(Name = "Vendor")]
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        [Display(Name = "Qty In Stock")]
        public int? QtyInStock { get; set; }
        [Display(Name = "Min Order Level")]
        public int? MinOrderLevel { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
        public int AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime AddedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByName { get; set; }
    }

    public class RMInventoryFinishedGoodResponse
    {
        public RMInventoryFinishedGoodModel rMInventoryFinishedGoodModel { get; set; }
        public List<RMInventoryFinishedGoodModel> data { get; set; }
        public ResponseObj Response { get; set; }

        public RMInventoryFinishedGoodResponse()
        {
            rMInventoryFinishedGoodModel = new RMInventoryFinishedGoodModel();
            this.data = new List<RMInventoryFinishedGoodModel>();
            this.Response = new ResponseObj();
        }
    }
    public class RMInventoryFinishedGoodModel
    {
        [Key]
        public int FinishedGoodId { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Product")]
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        [Display(Name = "Rack")]
        public int? RackId { get; set; }
        public string RackNumber { get; set; }
        [Display(Name = "Package")]
        public int? Package { get; set; }
        [Display(Name = "Quantity")]
        public int? Qty { get; set; }
        [Display(Name = "Min Order Level")]
        public int? MinOrderLevel { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
        public int AddedBy { get; set; }
        public string AddedByName { get; set; }
        public DateTime AddedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedByName { get; set; }
    }

}
