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

        [Required]
        public int PalletID { get; set; }

        [Display(Name = "Pallet No")]
        public string PalletNo { get; set; }
        public List<PalletMaster> Pallets { get; set; }

        [Required]
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
}
