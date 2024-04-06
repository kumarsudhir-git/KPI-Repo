using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class PalletMaster
    {
        [Key]
        public int PalletID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(15, MinimumLength = 1, ErrorMessage = "{0} should be minimum 1 characters and a maximum of 15 characters")]
        [DataType(DataType.Text)]
        [Display(Name = "Pallet No.")]
        public string PalletNo { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }
        
        [Display(Name = "Discontinued?")] 
        public bool IsDiscontinued { get; set; }

        public int RawMaterialID { get; set; }

        [Display(Name = "Raw Material Name")]
        public string RawMaterialName { get; set; }

        [Display(Name = "Qty (in Kgs)")]
        public decimal Qty { get; set; } //bags

        [Display(Name = "Qty (Bags)")]
        public decimal QtyBags { get; set; } //kgs

        [Display(Name = "Open Bags")]
        public decimal QtyOpened { get; set; } //opened

        [Display(Name = "Available Space (Kgs)")]
        public decimal AvailablityKgs { get; set; } //availablle space kgs

        public int TagColourID { get; set; }
        public string TagColour { get; set; }
        public string TagColours { get; set; }

        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    }

    public class PalletMasterResponse
    {
        public PalletMaster data { get; set; }
        public ResponseObj Response { get; set; }

        public PalletMasterResponse()
        {
            this.data = new PalletMaster();
            this.Response = new ResponseObj();
        }
    }

    public class PalletMastersResponse
    {
        public List<PalletMaster> data { get; set; }
        public ResponseObj Response { get; set; }

        public PalletMastersResponse()
        {
            this.data = new List<PalletMaster>();
            this.Response = new ResponseObj();
        }
    }
}
