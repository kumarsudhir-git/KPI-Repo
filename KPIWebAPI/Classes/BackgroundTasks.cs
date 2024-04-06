using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace KPIWebAPI.Classes
{
    public class BackgroundTasks
    {
        //public async Task<bool> SendEmail(string SenderName, string SenderEmail, string ToEmail, string Subject, string Body)
        //{
        //    bool isSuccess = false;

        //        string response = "";

        //        //StringBuilder sb = new StringBuilder();
        //        //sb.AppendLine("Sending Emails...<br/>");
        //        ////Response.Flush();

        //        var emails = db.DraftRequests.Where(m => m.RequestStatus == "OP");
        //        foreach (var eml in emails) 
        //        {
        //            try
        //            {
        //                string emailBody = GetTemplate(J60EmailType.Draft);
        //                emailBody = emailBody.Replace("!REQNO!", eml.RequestID.ToString());
        //                emailBody = emailBody.Replace("!REFCODE!", eml.CitationDetails);

        //                MailMessage mailMessage = new MailMessage();
        //                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"], ConfigurationManager.AppSettings["smtpUserName"]);
        //                mailMessage.Sender = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
        //                mailMessage.To.Add(new MailAddress(eml.EmailID));
        //                mailMessage.CC.Add(new MailAddress(ConfigurationManager.AppSettings["CC1"]));
        //                mailMessage.Subject = "Your Draft from Vakeel Ka Number...";
        //                mailMessage.Body = emailBody;
        //                //mailMessage.IsBodyHtml = true;

        //                var filename = db.Judgments.SingleOrDefault(x => x.UniqueID == eml.UniqueID).CitationDetails;

        //                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"~\Drafts\" + filename + ".pdf"));
        //                mailMessage.Attachments.Add(new Attachment(file.FullName));

        //                string host = ConfigurationManager.AppSettings["host"];
        //                int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
        //                SmtpClient smtp = new SmtpClient(host, port);

        //                //bool enableDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["enableDefaultCredentials"]);
        //                smtp.UseDefaultCredentials = true; // enableDefaultCredentials;//true;
        //                                                   //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        //                mailMessage.Priority = MailPriority.High;
        //                //smtp.Credentials = new NetworkCredential("usuthar13@gmail.com", "8087298675");
        //                string smtpUser = ConfigurationManager.AppSettings["smtpUser"];
        //                string smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
        //                smtp.Credentials = new NetworkCredential(smtpUser, smtpPassword);

        //                //bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);
        //                //smtp.EnableSsl = enableSSL;

        //                smtp.EnableSsl = true;

        //                smtp.Send(mailMessage);

        //                eml.RequestStatus = "CL";
        //                eml.Notes += "Sent to " + eml.EmailID + " on " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + "\n";

        //                //sb.AppendLine(eml.To + " | SENT<br/>");
        //                ////Response.Flush();
        //            }
        //            catch (Exception e)
        //            {
        //                eml.Notes += e.Message + "\n";

        //                LogException(e.Message);

        //                //sb.AppendLine(eml.To + " | " + e.Message + "<br/>");
        //                ////Response.Flush();
        //            }

        //            db.SubmitChanges();
        //        }

        //        //db.SubmitChanges();

        //        //divStatus.InnerHtml = sb.ToString();




        //    return isSuccess;
        //}
    }
}