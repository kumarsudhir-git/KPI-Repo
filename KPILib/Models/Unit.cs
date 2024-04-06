using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public partial class Unit
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
    }

    public class UnitResponse
    {
        public Unit unitObj { get; set; }
        public List<Unit> data { get; set; }
        public ResponseObj Response { get; set; }

        public UnitResponse()
        {
            this.data = new List<Unit>();
            this.Response = new ResponseObj();
            this.unitObj = new Unit();
        }
    }
}
