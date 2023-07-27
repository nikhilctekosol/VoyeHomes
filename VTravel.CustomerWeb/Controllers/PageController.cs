using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VTravel.CustomerWeb.Models;


namespace VTravel.CustomerWeb.Controllers
{
    public class PageController : Controller
    {
        // GET: PropertyController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PropertyController/Details/5
    
        public IActionResult Details(string id)
        {
            try
            {
               
                PageViewModel pageViewModel = new PageViewModel();
                MySqlHelper sqlHelper = new MySqlHelper();
                if (id == "super-hosts")
                {
                    pageViewModel.hostList = new List<SuperHost>();
                    var query = string.Format(@"SELECT s.id,s.host_name,s.destination_id,s.image,d.title AS destination_title                                                            
                                 from super_host s  LEFT JOIN destination d ON s.destination_id=d.id
                                 where s.is_active='Y' ORDER BY s.sort_order"
                                       );

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {

                        pageViewModel.hostList.Add(new SuperHost
                        {

                            id = Convert.ToInt32(r["id"].ToString()),
                            destination_id = Convert.ToInt32(r["destination_id"].ToString()),
                            host_name = r["host_name"].ToString(),
                            image = r["image"].ToString(),
                            destination_title = r["destination_title"].ToString()

                        });


                       





                    }

                    ViewData["Title"] = "Super Hosts";
                    ViewData["Keywords"] = "Super Hosts";
                    ViewData["Description"] = "Super Hosts";

                    return View("SuperHosts", pageViewModel);
                }
                //else if (id == "partner-with-us")
                //{
                    

                //    ViewData["Title"] = "Partner with Us";
                //    ViewData["Keywords"] = "Partner with Us";
                //    ViewData["Description"] = "Partner with Us";

                   

                //    return View("PartnerWithUs", pageViewModel);
                //}
                else {
                   
               

                        var idArray = id.Split('-');
                        var encodedId = idArray[idArray.Length - 1];
                        var decodedId = General.DecodeString(encodedId);


                        pageViewModel.sitePage = new SitePage();
                        //get top 10 property list grouped by tags in each group for home displayabl tags

                    

                        var query = string.Format(@"SELECT id,title,page_status,content,sort_order,
                                    meta_title,meta_keywords,meta_description                           
                                 from page p  
                                 where p.id={0} AND  p.is_active='Y' AND p.page_status='ACTIVE'", Convert.ToInt32(decodedId)
                                        );

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow r in ds.Tables[0].Rows)
                            {

                                pageViewModel.sitePage = new SitePage
                                {

                                    id = Convert.ToInt32(r["id"].ToString()),
                                    sortOrder = Convert.ToInt32(r["sort_order"].ToString()),
                                    title = r["title"].ToString(),
                                    pageStatus = r["page_status"].ToString(),
                                    content = r["content"].ToString(),
                                    metaTitle = r["meta_title"].ToString(),
                                    metaKeywords = r["meta_keywords"].ToString(),
                                    metaDescription = r["meta_description"].ToString()

                                };


                                ViewData["Title"] = pageViewModel.sitePage.metaTitle;
                                ViewData["Keywords"] = pageViewModel.sitePage.metaKeywords;
                                ViewData["Description"] = pageViewModel.sitePage.metaDescription;

                                ViewData["CanonicalUrl"] = General.GetUrlSlug(pageViewModel.sitePage.metaTitle) + "-" + encodedId;



                            }
                        }
                        else
                        {
                            return Redirect("../Home/Error");
                        }
                       
                    }
                    else
                    {
                        return Redirect("../Home/Error");
                    }
                        
                
                    return View(pageViewModel);
                }
            }
            catch (Exception ex)
            {
                General.LogException(ex);
                return Redirect("../Home/Error");
            }

            return null;
        }

        public IActionResult PartnerWithUs()
        {
            try
            {
                ViewData["CanonicalUrl"] = "page/partner-with-us";
                return View();
                
            }
            catch (Exception ex)
            {
                General.LogException(ex);
                return Redirect("../Home/Error");
            }

            return null;
        }

        //Enquiry
        [HttpPost]
        public IActionResult PartnerWithUs(PartnerEnquiryModel model)
        {
            if (ModelState.IsValid)
            {

              
                try
                {
                    MySqlHelper sqlHelper = new MySqlHelper();



                    var query = string.Format(@"INSERT INTO partner_enquiry(full_name,	mobile,	email,property_location,details)
                                  VALUES('{0}','{1}','{2}','{3}','{4}');SELECT LAST_INSERT_ID() AS id;"
                                     , model.full_name, model.mobile, model.email, model.property_location, model.details);
                    var ds = sqlHelper.GetDatasetByMySql(query);

                    query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='partner_enquiry_email_admin'";
                    ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var emailBody = ds.Tables[0].Rows[0]["content"].ToString();
                            

                            emailBody = emailBody.Replace("#full_name#", model.full_name)
                            .Replace("#mobile#", model.mobile)
                            .Replace("#email#", model.email)
                            .Replace("#property_location#", model.property_location)
                            .Replace("#details#", model.details);



                            var subject = General.GetSettingsValue("partner_enquiry_email_subject")
                                .Replace("#full_name#", model.full_name).Replace("#property_location#", model.property_location);

                            General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("partner_enquiry_email_to"), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("partner_enquiry_from_display_name"));

                            return View("ThankYou");




                        }
                    }

                   


                }
                catch (Exception ex)
                {

                    General.LogException(ex);
                }


            }
            return View();
        }

        public IActionResult GiftCard()
        {
            try
            {
                ViewData["CanonicalUrl"] = "page/gift-card";
                return View();

            }
            catch (Exception ex)
            {
                General.LogException(ex);
                return Redirect("../Home/Error");
            }

            return null;
        }

        //Enquiry
        [HttpPost]
        public IActionResult GiftCard(GiftCardEnquiryModel model)
        {
            if (ModelState.IsValid)
            {


                try
                {
                    MySqlHelper sqlHelper = new MySqlHelper();



                    var query = string.Format(@"INSERT INTO giftcard_enquiry(denomination,	quantity,
                   delivery_option,delivery_mode,receiver_name,receiver_email,receiver_mobile,
                   message,sender_name,sender_email,sender_mobile,when_to_send)
                                  VALUES({0},{1},'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');SELECT LAST_INSERT_ID() AS id;"
                                     , model.denomination, model.quantity, model.delivery_option, model.delivery_mode, model.receiver_name,
                                     model.receiver_email,model.receiver_mobile,model.message,model.sender_name,model.sender_email,model.sender_mobile,model.when_to_send);
                    var ds = sqlHelper.GetDatasetByMySql(query);

                    query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='giftcard_enquiry_email_admin'";
                    ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var emailBody = ds.Tables[0].Rows[0]["content"].ToString();


                            emailBody = emailBody.Replace("#denomination#", model.denomination.ToString())
                            .Replace("#quantity#", model.quantity.ToString())
                            .Replace("#delivery_option#", model.delivery_option)
                            .Replace("#delivery_mode#", model.delivery_mode)
                            .Replace("#receiver_name#", model.receiver_name)
                            .Replace("#receiver_email#", model.receiver_email)
                            .Replace("#receiver_mobile#", model.receiver_mobile)
                            .Replace("#message#", model.message)
                            .Replace("#sender_name#", model.sender_name)
                            .Replace("#sender_email#", model.sender_email)
                            .Replace("#sender_mobile#", model.sender_mobile)
                            .Replace("#when_to_send#", model.when_to_send);



                            var subject = General.GetSettingsValue("giftcard_enquiry_email_subject")
                                .Replace("#sender_name#", model.sender_name);

                            General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("giftcard_enquiry_email_to"), General.GetSettingsValue("giftcard_enquiry_from_email"), General.GetSettingsValue("giftcard_enquiry_from_display_name"));

                            return View("ThankYou");




                        }
                    }




                }
                catch (Exception ex)
                {

                    General.LogException(ex);
                }


            }
            return View();
        }
    }
}
