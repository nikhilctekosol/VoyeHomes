using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;


namespace VTravel.Admin.Controllers
{
    
    [Route("api/partner-user"), Authorize(Roles = "ADMIN")]
    public class PartnerUserController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public PartnerUserController(IHostingEnvironment hostingEnvironment)
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

                List<PartnerUser> users = new List<PartnerUser>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select t1.id,user_name,user_role,name_of_user,GROUP_CONCAT(t2.property_id) AS user_properties FROM partner_user t1 left join partner_user_property t2 on t1.id=t2.partner_user_id WHERE t1.is_active='Y' GROUP BY t1.id ORDER BY name_of_user"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    users.Add(
                        new PartnerUser
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            nameOfUser = r["name_of_user"].ToString(),
                            userName = r["user_name"].ToString(),
                            userRole = r["user_role"].ToString(),
                            userProperties = Array.ConvertAll(r["user_properties"].ToString().Split(','), s => int.Parse(s)) 
                        }
                        ); ;

                }


                response.Data = users;
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
        public IActionResult Create([FromBody] PartnerUser model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO partner_user(user_name,name_of_user,user_role) VALUES('{0}','{1}','{2}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.userName, model.nameOfUser,model.userRole);

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

                                foreach (var property_id in model.userProperties)
                                {
                                    query = string.Format(@"INSERT INTO partner_user_property(partner_user_id,property_id)  VALUES({0},{1})", model.id, property_id);
                                    ds = sqlHelper.GetDatasetByMySql(query);
                                }

                                response.ActionStatus = "SUCCESS";
                            }
                        }
                    }

                }
                else
                {
                    return BadRequest("Invalid details");
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
        public IActionResult Update([FromBody] PartnerUser model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE partner_user SET user_name='{0}',name_of_user='{1}',user_role='{2}' WHERE id={3}",
                                     model.userName, model.nameOfUser, model.userRole,  id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    query = string.Format(@"DELETE FROM partner_user_property  WHERE partner_user_id={0}",id);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    foreach(var property_id in model.userProperties)
                    {
                        query = string.Format(@"INSERT INTO partner_user_property(partner_user_id,property_id)  VALUES({0},{1})", id, property_id);
                        ds = sqlHelper.GetDatasetByMySql(query);
                    }

                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid details");
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

                    var query = string.Format(@"UPDATE partner_user SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid details");
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


