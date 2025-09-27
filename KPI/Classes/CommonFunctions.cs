using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace KPI.Classes
{
    public class CommonFunctions
    {
        public static HttpClient client
        {
            get
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri(ConfigurationManager.AppSettings["WebAPIBaseURL"].ToString())
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (HttpContext.Current.Session["JWTToken"] != null)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {HttpContext.Current.Session["JWTToken"]}");
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer {HttpContext.Current.Session["JWTToken"].ToString()}");
                }
                return client;
            }
        }

        public static void WriteToLog(string msg)
        {
            FileInfo fi = new FileInfo(HttpContext.Current.Server.MapPath("~/LOG_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt"));
            StreamWriter sw;
            string sLine = "";

            if (!fi.Exists)
                sw = fi.CreateText();
            else
                sw = fi.AppendText();

            sLine = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + " :: " + msg;
            sw.WriteLine(sLine);

            sw.Close();
        }



    }

}