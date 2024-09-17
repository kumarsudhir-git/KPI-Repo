using KPILib.Models;
using KPIWebAPI.AuthFilters;
using KPIWebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace KPIWebAPI.Controllers
{
    [CustomAuthFilter("M_PO")]
    public class PurchaseAPIController : CCSPLBaseAPIController
    {
        // GET: PurchaseAPI
        public IHttpActionResult GetAll()
        {
            var returnValue = new PurchaseMastersResponse();

            try
            {
                var data = db.PurchaseMasters.OrderByDescending(x => x.PurchaseID).ToList();
                foreach (var obj in data)
                {
                    var o = mapper.Map<PurchaseMaster, KPILib.Models.PurchaseMaster>(obj);
                    //o.CompanyLocation = obj.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.CompanyLocationMaster.LocationName + "]";
                    VendorMaster vendorMaster = CommonFunctions.GetVendorDetailsFromId(o.CompanyLocationID);
                    if (vendorMaster != null)
                    {
                        o.CompanyLocation = vendorMaster.VendorName + " [" + vendorMaster.Address + "]";
                    }
                    o.Status = obj.PurchaseStatusMaster.PurchaseStatus;
                    o.User = obj.UserMaster.Username;

                    returnValue.data.Add(o);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        // GET: PurchaseAPI
        public IHttpActionResult GetAllByRMID(int id)
        {
            var returnValue = new PurchaseMastDetailsResponse();

            try
            {
                var data = from pd in db.PurchaseDetails
                           from pm in db.PurchaseMasters
                           from comp in db.CompanyMasters
                           from comploc in db.CompanyLocationMasters
                           from rm in db.RawMaterialMasters
                           from usr in db.UserMasters
                           where
                                pd.PurchaseID == pm.PurchaseID &&
                                pm.CompanyLocationID == comploc.CompanyLocationID &&
                                comploc.CompanyID == comp.CompanyID &&
                                pd.RawMatarialID == rm.RawMaterialID &&
                                pm.UserID == usr.UserID &&
                                pm.PurchaseStatusID < ((int)enumPurchaseStatus.Full_Rcvd__Closed) &&
                                pd.RawMatarialID == id &&
                                pd.Qty > pd.RcvdQty
                           select new
                           {
                               pd.PurchaseDetailsID,
                               pm.PurchaseDate,
                               pm.PurchaseID,
                               CompanyLocation = comp.CompanyName + " [" + comploc.LocationName + "]",
                               //PurchaseDetailsInstructions = pd.Instructions.Replace("\n", "<br/>"),
                               //PurchaseMasterInstructions = pm.Instructions.Replace("\n", "<br/>"),
                               PurchaseDetailsInstructions = pd.Instructions.Replace("\n", "..."),
                               PurchaseMasterInstructions = pm.Instructions.Replace("\n", "..."),
                               Qty = pd.Qty,    // * RM_BAG_CAPACITY,
                               RawMaterialID = pd.RawMatarialID,
                               RawMaterialName = rm.RawMaterialName,
                               Status = pm.PurchaseStatusMaster.PurchaseStatus,
                               User = usr.Username
                           };

                //var data = db.PurchaseDetails.Where(x => x.PurchaseMaster.PurchaseStatusID < ((int)enumPurchaseStatus.Full_Rcvd__Closed) && x.RawMatarialID == id && x.Qty > x.RcvdQty).OrderByDescending(x => x.PurchaseID);
                foreach (var obj in data)
                {
                    var o = new PurchaseMastDetail()
                    {
                        PurchaseDetailsID = obj.PurchaseDetailsID,
                        CompanyLocation = obj.CompanyLocation,
                        PurchaseDate = obj.PurchaseDate,
                        PurchaseDetailsInstructions = obj.PurchaseDetailsInstructions,
                        PurchaseID = obj.PurchaseID,
                        PurchaseMasterInstructions = obj.PurchaseMasterInstructions,
                        Qty = obj.Qty,
                        RawMaterialID = obj.RawMaterialID,
                        RawMaterialName = obj.RawMaterialName,
                        Status = obj.Status,
                        User = obj.User

                        //PurchaseDetailsID = obj.PurchaseDetailsID,
                        //CompanyLocation = obj.PurchaseMaster.CompanyLocationMaster.CompanyMaster.CompanyName + " [" + obj.PurchaseMaster.CompanyLocationMaster.LocationName + "]",
                        //PurchaseDate = obj.PurchaseMaster.PurchaseDate,
                        //PurchaseDetailsInstructions = obj.Instructions.Replace("\n", "<br/>"),
                        //PurchaseID = obj.PurchaseID,
                        //PurchaseMasterInstructions = obj.PurchaseMaster.Instructions.Replace("\n", "<br/>"),
                        //Qty = obj.Qty * RM_BAG_CAPACITY,
                        //RawMaterialID = obj.RawMatarialID,
                        //RawMaterialName = obj.RawMaterialMaster.RawMaterialName,
                        //Status = obj.PurchaseMaster.PurchaseStatusMaster.PurchaseStatus,
                        //User = obj.PurchaseMaster.UserMaster.Username
                    };

                    returnValue.data.Add(o);
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        // GET api/values/5
        public IHttpActionResult Get(int id)
        {
            var returnValue = new PurchaseMasterResponse();

            try
            {
                #region get all Locations for ddl
                //TODO: 101 = Vendors
                //var Locations = db.CompanyLocationMasters.Where(x => !x.CompanyMaster.IsDiscontinued && x.CompanyMaster.CompanyTypeID == 101).Where(x => !x.IsDiscontinued).OrderBy(x => x.CompanyMaster.CompanyName).OrderBy(x => x.LocationName).ToList();
                //List<KPILib.Models.KeyValuePair> compLocations = new List<KPILib.Models.KeyValuePair>();
                //foreach (var obj in Locations)
                //{
                //    var o = new KeyValuePair { Key = obj.CompanyLocationID, Value = obj.CompanyMaster.CompanyName + " [" + obj.LocationName + "]" };
                //    compLocations.Add(o);
                //}
                #endregion

                #region get all RawMaterial for ddl
                var Materials = db.RawMaterialMasters.Where(x => !x.IsDiscontinued).OrderBy(x => x.RawMaterialName).ToList();
                List<KPILib.Models.KeyValuePair> materials = new List<KPILib.Models.KeyValuePair>();
                foreach (var obj in Materials)
                {
                    var o = new KeyValuePair { Key = obj.RawMaterialID, Value = obj.RawMaterialName };
                    materials.Add(o);
                }
                materials.Insert(0, new KeyValuePair { Key = 0, Value = "--Select Raw Material--" });

                #endregion

                var data = db.PurchaseMasters.SingleOrDefault(x => x.PurchaseID == id);
                if (data != null)
                {
                    VendorMaster vendorMaster = CommonFunctions.GetVendorDetailsFromId(data.CompanyLocationID);

                    var o = mapper.Map<PurchaseMaster, KPILib.Models.PurchaseMaster>(data);
                    //o.Locations = compLocations;
                    o.Materials = materials;
                    o.Status = data.PurchaseStatusMaster.PurchaseStatus;
                    o.User = data.UserMaster.Username;
                    o.CompanyLocation = vendorMaster?.Address;

                    foreach (var item in data.PurchaseDetails)
                    {
                        var lineItem = mapper.Map<PurchaseDetail, KPILib.Models.PurchaseDetails>(item);
                        lineItem.RawMatarialName = item.RawMaterialMaster.RawMaterialName;
                        o.LineItems.Add(lineItem);
                    }
                    //if (o.LineItems.Count < 5)
                    //{
                    //    for (int i = o.LineItems.Count + 1; i <= 5; i++)
                    //    {
                    //        o.LineItems.Add(new PurchaseDetails());
                    //    }
                    //}

                    returnValue.data = o;
                }
                else
                {
                    var o = new KPILib.Models.PurchaseMaster() { LineItems = new List<PurchaseDetails>(), Materials = materials };
                    o.LineItems.Add(new PurchaseDetails());

                    if (o.LineItems.Count < 5)
                    {
                        for (int i = o.LineItems.Count; i <= 5; i++)
                        {
                            o.LineItems.Add(new PurchaseDetails());
                        }
                    }

                    returnValue.data = o;
                }

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Add(KPILib.Models.PurchaseMaster data)
        {
            var returnValue = new PurchaseMasterResponse();

            try
            {
                var o = mapper.Map<KPILib.Models.PurchaseMaster, PurchaseMaster>(data);
                foreach (var item in data.LineItems.Where(x => x.RawMatarialID != 0))
                {
                    o.PurchaseDetails.Add(new PurchaseDetail { RawMatarialID = item.RawMatarialID, Qty = item.Qty });
                }
                o.Instructions += "";

                //o.AddedOn = DateTime.Now;
                //o.LastModifiedOn = DateTime.Now;

                db.PurchaseMasters.Add(o);
                db.SaveChanges();

                returnValue.data = data;
                returnValue.data.PurchaseID = o.PurchaseID;

                //Need to discuss on this point to sendmail method

                //var po = db.PurchaseMasters.Where(x => x.PurchaseID == o.PurchaseID).Select(x => new { x.PurchaseID, x.UserMaster.Username, UserEmail = x.UserMaster.Email }).FirstOrDefault();

                //VendorMaster vendorMaster = CommonFunctions.GetVendorDetailsFromId(o.CompanyLocationID);

                //Dictionary<string, string> key_vals = new Dictionary<string, string>();
                //key_vals.Add("%NAME%", vendorMaster.VendorName);
                //key_vals.Add("%PO_NO%", po.PurchaseID.ToString());
                //key_vals.Add("%USER%", po.Username);
                //key_vals.Add("%USER_EMAIL%", po.UserEmail);

                //SendEmail(vendorMaster.VendorName, po.Email, po.PurchaseID, key_vals);

                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }

        public IHttpActionResult Edit(KPILib.Models.PurchaseMaster data)
        {
            var returnValue = new PurchaseMasterResponse();

            try
            {
                var o = db.PurchaseMasters.SingleOrDefault(x => x.PurchaseID == data.PurchaseID);
                if (o != null)
                {
                    o.CompanyLocationID = data.CompanyLocationID;
                    o.Instructions = data.Instructions;
                    o.PurchaseDate = data.PurchaseDate;
                    o.PurchaseStatusID = 10;    //Ack Pending

                    //o.PurchaseDetails.Clear();

                    foreach (var item in o.PurchaseDetails.ToList())
                        //o.PurchaseDetails.Remove(item);
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;

                    foreach (var item in data.LineItems.Where(x => x.RawMatarialID != 0))
                    {
                        o.PurchaseDetails.Add(new PurchaseDetail { RawMatarialID = item.RawMatarialID, Qty = item.Qty });
                    }

                    db.Entry(o).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    returnValue.data = data;
                    returnValue.data.PurchaseID = o.PurchaseID;

                    //Need to confirm for email as it require emailid field in VendorMaster table
                    //var po = db.PurchaseMasters.Where(x => x.PurchaseID == o.PurchaseID).Select(x => new { x.PurchaseID, x.CompanyLocationMaster.ContactPerson, x.CompanyLocationMaster.Email, x.UserMaster.Username, UserEmail = x.UserMaster.Email }).FirstOrDefault();

                    //Dictionary<string, string> key_vals = new Dictionary<string, string>();
                    //key_vals.Add("%NAME%", po.ContactPerson);
                    //key_vals.Add("%PO_NO%", po.PurchaseID.ToString());
                    //key_vals.Add("%USER%", po.Username);
                    //key_vals.Add("%USER_EMAIL%", po.UserEmail);

                    //SendEmail(po.ContactPerson, po.Email, po.PurchaseID, key_vals);

                    returnValue.Response.IsSuccessful();
                }

                returnValue.data = data;
                returnValue.Response.IsSuccessful();
            }
            catch (Exception ex)
            {
                //TODO error handling
                returnValue.Response.ResponseMsg = ex.Message;
            }

            return Json(returnValue);
        }


        private async Task<string> SendEmail(string TO_NAME, string TO_EMAIL, int PO_ID, Dictionary<string, string> keyValuePairs)
        {
            string response = "";

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("Sending Emails...<br/>");
            ////Response.Flush();

            try
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"~/Templates/EMAIL_PO_INVITE.txt"));
                StreamReader reader = file.OpenText();
                string emailBody = reader.ReadToEnd();
                reader.Close();

                foreach (var item in keyValuePairs)
                {
                    emailBody = emailBody.Replace(item.Key, item.Value);
                }

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["smtpUser"], ConfigurationManager.AppSettings["smtpUserName"]);
                mailMessage.Sender = new MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                mailMessage.To.Add(new MailAddress(TO_EMAIL, TO_NAME));
                mailMessage.CC.Add(new MailAddress(ConfigurationManager.AppSettings["CC1"]));
                mailMessage.Subject = "Our new PO no. " + PO_ID.ToString() + "...";
                mailMessage.Body = emailBody;
                //mailMessage.IsBodyHtml = true;

                //var filename = db.Judgments.SingleOrDefault(x => x.UniqueID == eml.UniqueID).CitationDetails;

                //FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"~\Drafts\" + filename + ".pdf"));
                //mailMessage.Attachments.Add(new Attachment(file.FullName));

                string host = ConfigurationManager.AppSettings["host"];
                int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
                SmtpClient smtp = new SmtpClient(host, port);

                //bool enableDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["enableDefaultCredentials"]);
                smtp.UseDefaultCredentials = true; // enableDefaultCredentials;//true;
                                                   //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

                mailMessage.Priority = MailPriority.High;
                //smtp.Credentials = new NetworkCredential("usuthar13@gmail.com", "8087298675");
                string smtpUser = ConfigurationManager.AppSettings["smtpUser"];
                string smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPassword);

                //bool enableSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSL"]);
                //smtp.EnableSsl = enableSSL;

                smtp.EnableSsl = true;

                smtp.Send(mailMessage);

                response = "SUCCESS";

                //eml.RequestStatus = "CL";
                //eml.Notes += "Sent to " + eml.EmailID + " on " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + "\n";

                //sb.AppendLine(eml.To + " | SENT<br/>");
                ////Response.Flush();
            }
            catch (Exception e)
            {
                //eml.Notes += e.Message + "\n";

                //LogException(e.Message);

                //sb.AppendLine(eml.To + " | " + e.Message + "<br/>");
                ////Response.Flush();
                ///

                response = e.Message;
            }

            //db.SubmitChanges();

            //divStatus.InnerHtml = sb.ToString();

            return response;
        }
    }
}