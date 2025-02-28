using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KPIWebAPI.Classes
{
    public static class CommonLogger
    {
        private static readonly ILogger _logger = new LoggerConfiguration()
                                                            .WriteTo.File(Path.Combine(HttpRuntime.AppDomainAppPath, "Logs", "KPIWebAPI-log.txt"), rollingInterval: RollingInterval.Day)
                                                            .CreateLogger();

        public static void Info(string message)
        {
            _logger.Information(message);
        }

        public static void Error(Exception ex, string errorMessage)
        {
            _logger.Error(ex, errorMessage);
        }
    }
}
