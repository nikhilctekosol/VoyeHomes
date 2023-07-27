using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.HostWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;


namespace VTravel.HostWeb.Controllers
{
    
    [Route("api/attribute"), Authorize]
    public class AttributeController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public AttributeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        
            
       

        [HttpPost, Route("sort")]
        public IActionResult Sort([FromBody] SortData model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                 
                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE property SET sort_order=sort_order+{0} WHERE sort_order>={1};  
                  UPDATE property SET sort_order={1} WHERE id={2}", model.pushDownValue,
                                     model.sortOrder,model.itemId);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);


                    
                    response.ActionStatus = "SUCCESS";
                    response.Message ="products sorted";
                }
                else
                {
                    return BadRequest("Invalid sort product");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-list")]
        public IActionResult GetList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<VAttribute> attributes = new List<VAttribute>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,attribute_name,sort_order 
                 FROM attribute WHERE is_active='Y' ORDER BY sort_order,attribute_name"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                //
                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    attributes.Add(
                        new VAttribute
                        {
                            id=Convert.ToInt32(r["id"].ToString()),
                            attributeName = r["attribute_name"].ToString()
                        }
                        );

                }


                response.Data = attributes;
                response.ActionStatus = "SUCCESS";
                   
                

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPost, Route("create")]
        public IActionResult Create([FromBody] Property model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO property(property_type_id,title) VALUES({0},'{1}'); SELECT LAST_INSERT_ID() AS id;}",
                                     model.propertyTypeId, model.title);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                DataRow r = ds.Tables[0].Rows[0];                              
                                model.id = Convert.ToInt32(r["id"].ToString());
                                response.Data = model;
                                response.ActionStatus = "SUCCESS";
                            }
                        }
                    }

                }
                else
                {
                    return BadRequest("Invalid sort product");
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


