using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Data;
using VTravel.Admin.Models;
using System.Linq;
using System.Security.Claims;

namespace VTravel.Admin.Controllers
{
    [Route("api/profit"), Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS,MARKETING")]
    public class ProfitController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ProfitController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;   
        }
        [HttpGet, Route("get-list")]
        public IActionResult GetList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ProfitDetails> profit = new List<ProfitDetails>();
                MySqlHelper sqlHelper = new MySqlHelper();

                //var query = string.Format(@"select t1.id, t1.room_id, t2.title, t1.channel_id, t3.channel_name, t1.mode, t1.price, t1.percentage, t1.include_food, t1.include_extra, t1.taxless_amount from profit_details t1
                //                            LEFT JOIN room t2 on t2.id = t1.room_id
                //                            LEFT JOIN booking_channel t3 on t1.channel_id = t3.id
                //                            where t2.property_id = {0}", id);
                var query = string.Format(@"select t1.id, t1.property_id, t1.channel_id, t3.channel_name, t1.mode, t1.percentage, t1.include_food, t1.include_extra, t1.taxless_amount from profit_details t1
                                            LEFT JOIN booking_channel t3 on t1.channel_id = t3.id
                                            where t1.property_id = {0}", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    profit.Add(
                        new ProfitDetails
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            propertyId = Convert.ToInt32(r["property_id"].ToString()),
                            room = "",
                            channelId = Convert.ToInt32(r["channel_id"].ToString()),
                            channel = r["channel_name"].ToString(),
                            mode = "",
                            price = 0,
                            percentage = Convert.ToDecimal(r["percentage"].ToString()),
                            include_food = r["include_food"].ToString(),
                            include_extra = r["include_extra"].ToString(),
                            taxless_amount = r["taxless_amount"].ToString()
                        }
                        );

                }


                response.Data = profit;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost, Route("create")]
        public IActionResult Create([FromBody] ProfitDetails model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    MySqlHelper sqlHelper = new MySqlHelper();

                    //var query = string.Format(@"INSERT INTO profit_details(room_id, channel_id, mode, price, percentage, include_food, include_extra, taxless_amount, created_by, created_on) 
                    //                            VALUES({0},{1},'{2}', {3}, {4}, {5}, {6}, {7}, {8}, '{9}');
                    //                     SELECT LAST_INSERT_ID() AS id;",
                    //                 model.roomId, model.channelId, model.mode, model.price, model.percentage, model.include_food, model.include_extra, model.taxless_amount, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")); 
                    var query = string.Format(@"INSERT INTO profit_details(property_id, channel_id, mode, percentage, include_food, include_extra, taxless_amount, created_by, created_on) 
                                                VALUES({0},{1},'{2}', {3}, {4}, {5}, {6}, {7}, '{8}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.propertyId, model.channelId, model.mode, model.percentage, model.include_food, model.include_extra, model.taxless_amount, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid profit details");
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
        [HttpPost, Route("update")]
        public IActionResult Update([FromBody] ProfitDetails model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE profit_details set mode = '{0}', percentage = {1}, include_food = {2}, include_extra = {3}, taxless_amount = {4}, updated_by = {5}, updated_on = '{6}'
                                                where id = {7};",
                                     model.mode, model.percentage, model.include_food, model.include_extra, model.taxless_amount, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid profit details");
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

                    var query = string.Format(@"DELETE from profit_details WHERE id={0};", id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid profit details");
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
