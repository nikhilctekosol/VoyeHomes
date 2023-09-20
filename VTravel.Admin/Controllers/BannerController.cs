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
using System.Security.Claims;
using System.Linq;

namespace VTravel.Admin.Controllers
{
    [Route("api/banner"), Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS,MARKETING")]
    public class BannerController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public BannerController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        [HttpGet, Route("get-list")]
        public IActionResult GetList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<BannerList> bannerlist = new List<BannerList>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select b.id, image_url, image_alt, b.title, b.description, IFNULL(b.navigate_url, '') navigate_url, b.is_active, show_in_home, IFNULL(b.property_id, 0) property_id
                                            , IFNULL(b.destination,0) destination_id, IFNULL(p.title, '') property, IFNULL(d.title, '') destination 
                                            from hero_banner b
                                            left join property p on p.id = b.property_id
                                            left join destination d on d.id = b.destination where b.is_active = 'Y'"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    bannerlist.Add(
                        new BannerList
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            image_url = r["image_url"].ToString(),
                            image_alt = r["image_alt"].ToString(),
                            title = r["title"].ToString(),
                            description = r["description"].ToString(),
                            navigate_url = r["navigate_url"].ToString(),
                            active = r["is_active"].ToString(),
                            show_in_home = r["show_in_home"].ToString(),
                            property_id = r["property_id"]== DBNull.Value ? "0" : r["property_id"].ToString(),
                            destination_id = r["destination_id"] == DBNull.Value ? "0" : r["destination_id"].ToString(),
                            property = r["property"].ToString(),
                            destination = r["destination"].ToString()
                        }
                        );

                }


                response.Data = bannerlist;
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

                BannerList bannerlist = new BannerList();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select b.id, image_url, image_alt, b.title, b.description, b.navigate_url, b.is_active, show_in_home, b.property_id, b.destination destination_id, p.title property, d.title destination from hero_banner b
                                            left join property p on p.id = b.property_id
                                            left join destination d on d.id = b.destination where b.is_active = 'Y' and b.id = {0}", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    bannerlist = new BannerList
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        image_url = r["image_url"].ToString(),
                        image_alt = r["image_alt"].ToString(),
                        title = r["title"].ToString(),
                        description = r["description"].ToString(),
                        navigate_url = r["navigate_url"].ToString(),
                        active = r["is_active"].ToString(),
                        show_in_home = r["show_in_home"].ToString(),
                        property_id = r["property_id"].ToString(),
                        destination_id = r["destination_id"].ToString(),
                        property = r["property"].ToString(),
                        destination = r["destination"].ToString()

                    };

                }

                response.Data = bannerlist;
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
        public IActionResult Create([FromBody] BannerList model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    var query = string.Format(@"INSERT INTO hero_banner(title, description, image_url, image_alt, property_id, destination, is_active, show_in_home, create_by, created_on, navigate_url)
                                        VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}', '{10}');
                                        SELECT LAST_INSERT_ID() AS id;",
                                     model.title, model.description, model.image_url, model.image_alt, model.property_id, model.destination_id, 'Y', model.show_in_home, userId, DateTime.Today.ToString("yyyy-MM-dd"), model.navigate_url);

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
                    return BadRequest("Invalid Banner details");
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
        public IActionResult Update([FromBody] BannerList model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    var query = string.Format(@"UPDATE hero_banner SET title='{0}',description='{1}',image_url='{2}',image_alt='{3}',property_id='{4}',destination='{5}'
                                            ,show_in_home='{6}',updated_by='{7}',updated_on='{8}', navigate_url = '{10}' WHERE id={9}",
                                     model.title, model.description, model.image_url, model.image_alt, model.property_id, model.destination_id, model.show_in_home, userId, DateTime.Today.ToString("yyyy-MM-dd"), id, model.navigate_url);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Banner details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
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

                    var query = string.Format(@"UPDATE hero_banner SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid banner details");
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
