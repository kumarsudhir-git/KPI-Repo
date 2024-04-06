using System;
using System.Collections.Generic;
using System.Text;

namespace KPILib.Models
{
    public class ResponseObj
    {
        public int ResponseCode { get; set; }
        public string ResponseMsg { get; set; }
        public object ResponseData { get; set; }

        public ResponseObj()
        {
            this.ResponseCode = 999;
            this.ResponseMsg = "Unknown Error!";
            this.ResponseData = null;
        }

        public void IsSuccessful()
        {
            this.ResponseCode = 200;
            this.ResponseMsg = "SUCCESS";
        }
    }
}
