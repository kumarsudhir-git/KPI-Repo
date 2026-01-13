using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "{0} should be minimum 1 characters and a maximum of 40 characters")]
        [DataType(DataType.Text)]

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        
        [Display(Name = "Inc Product Name")]
        public string IncomingProductName { get; set; }


        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Product Code")]
        [MaxLength(50)]
        public string ProductCode { get; set; }

        [MaxLength(200)]
        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }

       
        [Display(Name = "Product Category")]
        public int ProductCategoryID { get; set; }          //prodcat name, prodcats list

        //[Display(Name = "Product Category")]
        //public string ProductCategory { get; set; }

        //public List<ProductCategory> ProductCategories { get; set; }

       
        [Display(Name = "Unit of Measurement")]
        public int UOMID { get; set; }

        [Display(Name = "Unit of Measurement")]
        // [Required(ErrorMessage = "required")]
        public string UOM { get; set; }
        public List<UOM> UOMs { get; set; }

        [Display(Name = "Min. Order Qty")]
        [Required(ErrorMessage = "{0} is required")]
        [Range(0, 999999999)]
        public int MinQtyUOM { get; set; }

        [Range(0, 999999999)]
       
        public int ConversionUOMID { get; set; }
        
        [Range(0, 999999999)]
        

        public decimal ConversionQty { get; set; }

        [Display(Name = "Raw Material")]

        public int RawMaterialID { get; set; }

        [Display(Name = "Min Selling Price")]
        [Required(ErrorMessage = "required")]
        [Range(0, 999999999)]

        public decimal MinimumSellingPrice { get; set; }

        [Display(Name = "Colour")]

        public string Colour { get; set; }

        //[Display(Name = "Raw Material")]
        //public string RawMaterial { get; set; }
        public List<RawMaterial> RawMaterials { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Display(Name = "Mould")]
        public int MouldID { get; set; }

        [Display(Name = "Mould")]
        public string Mould { get; set; }
        public List<Mould> Moulds { get; set; }

        [Display(Name = "RM Reqd for Min Qty (Kgs)")]
        [Range(0, 999999999)]

        [Required(ErrorMessage = "{0} is required")]
        public decimal RMReqdForUOMQty { get; set; }
        
        [Display(Name = "Packaging Qty")]
        [Range(0, 999999999)]
        public int PkgQty { get; set; }

        [Display(Name = "Packs Per Rack")]
        [Range(0, 999999999)]
        public int PkgsPerRack { get; set; }

        [Display(Name = "Discontinued?")]
        public bool IsDiscontinued { get; set; }

        public int InStock { get; set; }
        public int Reserved { get; set; }
        public int Short { get; set; }
        //public int Ordered { get; set; }
        public int LoanedOut { get; set; }
        public int LoanedIn { get; set; }

        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public List<ProductRawMaterialMapping> productRawMaterialMappings { get; set; }

    }

    public class ProductResponse
    {
        public Product data { get; set; }
        public ResponseObj Response { get; set; }

        public ProductResponse()
        {
            this.data = new Product();
            this.Response = new ResponseObj();
        }
    }

    public class ProductsResponse
    {
        public List<Product> data { get; set; }
        public ResponseObj Response { get; set; }

        public ProductsResponse()
        {
            this.data = new List<Product>();
            this.Response = new ResponseObj();
        }
    }
}
