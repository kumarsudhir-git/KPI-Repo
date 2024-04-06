using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace KPILib.Models
{
    public class UOM
    {
        public int UOMID { get; set; }
        public string UnitsName { get; set; }
        public bool IsDiscontinued { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
    }
}
