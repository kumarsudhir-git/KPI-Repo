using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public class TypeOfCompany
    {
        public int CompanyTypeID { get; set; }
        public string CompanyType { get; set; }
        public System.DateTime AddedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }

    }
}
