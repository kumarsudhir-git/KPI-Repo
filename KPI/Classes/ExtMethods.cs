using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KPI.Classes
{
    public static class ExtMethods
    {
        public static string DefaultDateFormat(this DateTime dt)
        {
            return dt.ToString("dd-MMM-yyyy");
        }

        public static string DefaultDateTimeFormat(this DateTime dt)
        {
            return dt.ToString("dd-MMM-yyyy hh:mm:ss tt");
        }
    }
}