using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

namespace VTravel.Admin.Controllers
{
    [Route("api/owner"), Authorize(Roles = "ADMIN")]
    public class OwnerController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public OwnerController(IHostingEnvironment hostingEnvironment)
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

                List<Owners> owners = new List<Owners>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id, name, address, gst_no, pan_no, ownership_type from owner_master where is_active = 'Y'");

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    owners.Add(
                        new Owners
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            name = r["name"].ToString(),
                            address = r["address"].ToString(),
                            gstno = r["gst_no"].ToString(),
                            pan = r["pan_no"].ToString(),
                            ownershiptype = r["ownership_type"].ToString(),
                        }
                        );

                }


                response.Data = owners;
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
        public IActionResult Create([FromBody] Owners model)
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

                    var query = string.Format(@"INSERT INTO owner_master(name, address, gst_no, pan_no, ownership_type, created_by, created_on, is_active) VALUES('{0}','{1}','{2}','{3}','{4}', {5}, '{6}', '{7}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.name, model.address, model.gstno, model.pan, model.ownershiptype, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 'Y');

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
                    return BadRequest("Invalid Owner details");
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
        public IActionResult Update([FromBody] Owners model)
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

                    var query = string.Format(@"UPDATE owner_master SET name='{0}',address='{1}', gst_no = '{2}', pan_no = '{3}', ownership_type='{4}', updated_by = {5}, updated_on = '{6}' WHERE id={7}",
                                     model.name, model.address, model.gstno, model.pan, model.ownershiptype, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Owner details");
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

                    var query = string.Format(@"UPDATE owner_master SET is_active='N' WHERE id={0}", id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Owner details");
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
