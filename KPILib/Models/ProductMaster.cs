using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public class ProductMaster
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }

    public class ProductMasterResponse
    {
        public ProductMaster productMaster { get; set; }
        public ResponseObj Response { get; set; }
        public List<ProductMaster> productMastersListData { get; set; }

        public ProductMasterResponse()
        {
            this.productMaster = new ProductMaster();
            this.Response = new ResponseObj();
            this.productMastersListData = new List<ProductMaster>();
        }

    }

}
