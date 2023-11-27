using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Transactions;

namespace VTravel.Admin.Controllers
{
    [Route("api/tds"), Authorize(Roles = "ADMIN")]
    public class TDSController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public TDSController(IHostingEnvironment hostingEnvironment)
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

                List<Tds> tds = new List<Tds>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id, ownership_type, percentage, effective_from from tds_details where is_active = 'Y'");

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    tds.Add(
                        new Tds
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            ownershiptype = r["ownership_type"].ToString(),
                            percentage = Convert.ToDecimal(r["percentage"].ToString()),
                            effective = Convert.ToDateTime(r["effective_from"]).ToString("dd-MM-yyyy"),
                        }
                        );

                }


                response.Data = tds;
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
        public IActionResult Create([FromBody] Tds model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        MySqlHelper sqlHelper = new MySqlHelper();
                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        var query = string.Format(@"select id, ownership_type, percentage, effective_from from tds_details where is_active = 'Y' and ownership_type = '{0}'", model.ownershiptype);

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);

                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    query = string.Format(@"update tds_details set effective_to = '{0}', updated_by = {1}, updated_on = '{2}', is_active = 'N' where id = {3}"
                                                , Convert.ToDateTime(model.effective).AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"), userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ds.Tables[0].Rows[0]["id"]);

                                    ds = sqlHelper.GetDatasetByMySql(query);
                                }
                            }
                        }

                        query = string.Format(@"INSERT INTO tds_details(ownership_type, percentage, effective_from, created_by, created_on, is_active) VALUES('{0}',{1},'{2}',{3},'{4}', '{5}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                         model.ownershiptype, model.percentage, model.effective, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 'Y');

                        ds = sqlHelper.GetDatasetByMySql(query);
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
                        scope.Complete();
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
    }
}
