using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.HostWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace VTravel.HostWeb.Controllers
{

    [Route("api/amenity"), Authorize]
    public class AmenityController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public AmenityController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }

        //
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

                    var query = string.Format(@"UPDATE amenity SET sort_order=sort_order+{0} WHERE sort_order>={1};  
                  UPDATE Tag SET sort_order={1} WHERE id={2}", model.pushDownValue,
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

                List<Amenity> amenities = new List<Amenity>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,amenity_name,image1  
                 FROM amenity WHERE is_active='Y' ORDER BY sort_order,amenity_name"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    amenities.Add(
                        new Amenity
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            amenityName = r["amenity_name"].ToString(),
                            image1 = r["image1"].ToString(),
                         }
                        );

                }


                response.Data = amenities;
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

                Amenity amenity = new Amenity();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,amenity_name,image1 
                  FROM amenity WHERE is_active='Y' AND id={0} ORDER BY sort_order", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    amenity = new Amenity
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        amenityName = r["amenity_name"].ToString(),
                        image1 = r["image1"].ToString(),
                    };

                }

                response.Data = amenity;
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
        public IActionResult Create([FromBody] Amenity model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO amenity(amenity_name,image1) VALUES('{0}','{1}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.amenityName, model.image1);

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

        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] Amenity model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE amenity SET amenity_name='{0}',image1='{1}' WHERE id={2}",
                                     model.amenityName,model.image1, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

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

                    var query = string.Format(@"UPDATE amenity SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

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


