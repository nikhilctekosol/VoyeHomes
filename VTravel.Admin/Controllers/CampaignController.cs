using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace VTravel.Admin.Controllers
{

    [Route("api/Campaign")
        //, Authorize(Roles = "ADMIN")
        ]
    public class CampaignController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public CampaignController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }

        [HttpPost, Route("send-to-all")]
        public IActionResult Create([FromBody] Campaign model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"SELECT  DISTINCT cust_name,cust_email FROM customer WHERE cust_email='abdulnasarkvr@gmail.com'");

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                //DataRow r = ds.Tables[0].Rows[0];
                                //model.id = Convert.ToInt32(r["id"].ToString());
                                //foreach
                                string emailSubject = "";
                                emailSubject = "Happy New Year";

                                string bodyBase = General.GetSettingsValue("happy_new_year_email_template");

                                foreach (DataRow r in ds.Tables[0].Rows)
                                {
                                    var body = bodyBase.Replace("#FIRST_NAME#", r["cust_name"].ToString());

                                    var emailResponse = General.SendMailMailgun(emailSubject,
                                        body, r["cust_email"].ToString(),
                                       General.GetSettingsValue("campaign_from_email"),
                                        General.GetSettingsValue("campaign_from_display_name"));

                                }



                                response.Data = model;
                                response.ActionStatus = "SUCCESS";
                            }
                        }
                    }

                }
                else
                {
                    return BadRequest("Invalid Tag details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

       
    }
}


