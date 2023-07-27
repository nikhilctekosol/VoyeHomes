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

    [Route("api/destination"), Authorize(Roles = "ADMIN")]
    public class DestinationController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public DestinationController(IHostingEnvironment hostingEnvironment)
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

                    var query = string.Format(@"UPDATE destination SET sort_order=sort_order+{0} WHERE sort_order>={1};  
                  UPDATE destination SET sort_order={1} WHERE id={2}", model.pushDownValue,
                                     model.sortOrder, model.itemId);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);



                    response.ActionStatus = "SUCCESS";
                    response.Message = "sorted";
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

                List<Destination> destinations = new List<Destination>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,thumbnail,thumbnail_alt,description,meta_title,meta_keywords,meta_description, IFNULL(short_desc, '') short_desc   
                 FROM destination WHERE is_active='Y' ORDER BY sort_order,title"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    destinations.Add(
                        new Destination
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            thumbnail = r["thumbnail"].ToString(),
                            thumbnail_alt = r["thumbnail_alt"].ToString(),
                            description = r["description"].ToString(),
                            short_desc = r["short_desc"].ToString(),
                            meta_title = r["meta_title"].ToString(),
                            meta_keywords = r["meta_keywords"].ToString(),
                            meta_description = r["meta_description"].ToString()
                        }
                        );

                }


                response.Data = destinations;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get")]
        public IActionResult Get(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                Destination destination = new Destination();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,thumbnail,thumbnail_alt,description,meta_title,meta_keywords,meta_description, IFNULL(short_desc, '') short_desc 
                  FROM destination WHERE is_active='Y' AND id={0} ORDER BY sort_order", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    destination = new Destination
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        title = r["title"].ToString(),
                        thumbnail = r["thumbnail"].ToString(),
                        thumbnail_alt = r["thumbnail_alt"].ToString(),
                        description = r["description"].ToString(),
                        short_desc = r["short_desc"].ToString(),
                        meta_title = r["meta_title"].ToString(),
                        meta_keywords = r["meta_keywords"].ToString(),
                        meta_description = r["meta_description"].ToString()
                    };

                }

                response.Data = destination;
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
        public IActionResult Create([FromBody] Destination model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO destination(title,thumbnail,description,meta_title,meta_keywords,meta_description,thumbnail_alt, short_desc)
                                     VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.title, model.thumbnail,model.description, model.meta_title, model.meta_keywords, model.meta_description,model.thumbnail_alt, model.short_desc);

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
                    return BadRequest("Invalid Destination details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] Destination model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE destination SET title='{0}',thumbnail='{1}',description='{2}',meta_title='{3}',meta_keywords='{4}',meta_description='{5}',thumbnail_alt='{6}',short_desc='{8}' WHERE id={7}",
                                     model.title,model.thumbnail,model.description, model.meta_title, model.meta_keywords, model.meta_description, model.thumbnail_alt, id, model.short_desc);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Destination details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }
              
        [HttpDelete, Route("delete")]
        public IActionResult Delete(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (id > 0)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE destination SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Destination details");
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


