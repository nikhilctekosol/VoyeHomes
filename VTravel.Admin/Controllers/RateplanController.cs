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
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc.Routing;

namespace VTravel.Admin.Controllers
{
    [Route("api/rateplan"), Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS,MARKETING")]
    public class RateplanController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public RateplanController(IHostingEnvironment hostingEnvironment)
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

                List<RatePlans> rateplans = new List<RatePlans>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select r.id, r.name, r.color, r.property_id, p.title property, r.is_active from rateplans r
                                    left join property p on p.id = r.property_id
                                    where r.is_active = 'Y' and r.property_id = {0}"
                                   , id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    rateplans.Add(
                        new RatePlans
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            name = r["name"].ToString(),
                            color = r["color"].ToString(),
                            propertyid = r["property_id"] == DBNull.Value ? "0" : r["property_id"].ToString(),
                            active = r["is_active"].ToString()
                        }
                        );

                }


                response.Data = rateplans;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);
        }
        [HttpGet, Route("get-activelist")]
        public IActionResult GetActiveList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<RatePlans> rateplans = new List<RatePlans>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select distinct rp.id, rp.name, rp.color, rp.property_id, p.title property, rp.is_active 
                                            from inventory i
                                            left join rateplans rp on rp.id = i.rateplan
                                            left join property p on p.id = rp.property_id
                                            where i.property_id = {0} and i.rateplan is not null and rp.is_active='Y'"
                                   , id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    rateplans.Add(
                        new RatePlans
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            name = r["name"].ToString(),
                            color = r["color"].ToString(),
                            propertyid = r["property_id"] == DBNull.Value ? "0" : r["property_id"].ToString(),
                            active = r["is_active"].ToString()
                        }
                        );

                }


                response.Data = rateplans;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);
        }
        [HttpGet, Route("get-inactivelist")]
        public IActionResult GetInactiveList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<RatePlans> rateplans = new List<RatePlans>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select rp.id, rp.name, rp.color, rp.property_id, p.title property, rp.is_active  from rateplans rp
                                            left join property p on p.id = rp.property_id
                                            where rp.id not in (select rateplan from inventory where property_id = {0} and rateplan is not null) and p.id = {0} and rp.is_active='Y'"
                                   , id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    rateplans.Add(
                        new RatePlans
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            name = r["name"].ToString(),
                            color = r["color"].ToString(),
                            propertyid = r["property_id"] == DBNull.Value ? "0" : r["property_id"].ToString(),
                            active = r["is_active"].ToString()
                        }
                        );

                }


                response.Data = rateplans;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);
        }
        [HttpGet, Route("get-colors")]
        public IActionResult GetColorList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ColorCodes> colors = new List<ColorCodes>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id, color_name, color_code from color_codes");

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    colors.Add(
                        new ColorCodes
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            colorname = r["color_name"].ToString(),
                            colorcode = r["color_code"].ToString()
                        }
                        );

                }


                response.Data = colors;
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
        [HttpPost, Route("create-rateplan")]
        public IActionResult CreateRatePlan([FromBody] RatePlans model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty;
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    query = string.Format(@"INSERT INTO rateplans(name,color,property_id, is_active, created_by, created_on)
                      VALUES('{0}','{1}',{2},'{3}',{4},'{5}')",
                        model.name, model.color, model.propertyid, 'Y', userId, DateTime.Today.ToString("yyyy-MM-dd"));


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid rateplan details");
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
        [HttpPost, Route("update-rateplan")]
        public IActionResult UpdateRatePlan([FromBody] RatePlans model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();
                    string query = string.Empty;
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    query = string.Format(@"UPDATE rateplans set name= '{0}', color = '{1}', updated_by = {2}, updated_on = '{3}' where id = {4} ",
                        model.name, model.color, userId, DateTime.Today.ToString("yyyy-MM-dd"), model.id);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid rateplan details");
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

                    var query = string.Format(@"UPDATE rateplans SET is_active='N' WHERE id={0}",
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
        [HttpGet, Route("get-plan-details")]
        public IActionResult GetPlanDetails(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<RateplanDetails> rateplandetails = new List<RateplanDetails>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"(select distinct IFNULL(rb.id, 0) id, r.id room_id, r.title room, m.id mealid, m.mealplan, o.id occid, o.occupancy, IFNULL(rb.rate, 0) rate
                                            from rateplans rp
                                            left join room r on r.property_id = rp.property_id
                                            inner join room_occupancy ro on ro.room_id = r.id
                                            inner join occupancy o on (o.id = ro.occupancy)
                                            left join rateplan_breakup rb on rb.rateplan = rp.id and rb.room_id = r.id and o.id = rb.occupancy
                                            inner join mealplans m on m.id = IFNULL(rb.mealplan, 1)
                                            where rp.id = {0} order by r.id)  union 
                                            (select distinct IFNULL(rb.id, 0) id, r.id room_id, r.title room, m.id mealid, m.mealplan, o.id occid, o.occupancy, IFNULL(rb.rate, 0) rate
                                            from rateplans rp
                                            left join room r on r.property_id = rp.property_id
                                            inner join occupancy o on ( o.is_default = 'Y')
                                            left join rateplan_breakup rb on rb.rateplan = rp.id and rb.room_id = r.id and o.id = rb.occupancy
                                            inner join mealplans m on m.id = IFNULL(rb.mealplan, 1)
                                            where rp.id = {0} order by r.id) order by room_id"
                                   , id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    rateplandetails.Add(
                        new RateplanDetails
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            roomid = Convert.ToInt32(r["room_id"].ToString()),
                            room = r["room"].ToString(),
                            mealid = Convert.ToInt32(r["mealid"].ToString()),
                            meal = r["mealplan"].ToString(),
                            occid = Convert.ToInt32(r["occid"].ToString()),
                            occupancy = r["occupancy"].ToString(),
                            rate = r["rate"].ToString()
                        }
                        );

                }


                response.Data = rateplandetails;
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
        [HttpPost, Route("create-rateplan-details")]
        public IActionResult CreateRatePlanDetails([FromBody] List<RateplanDetails> model, int id)
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

                    var query = string.Format(@"DELETE from rateplan_breakup where rateplan = {0}",id);


                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

                    for (int i = 0; i < model.Count; i++)
                    {
                        MySqlHelper sqlHelper1 = new MySqlHelper();
                        sqlHelper1.AddSetParameterToMySqlCommand("roomid", MySqlDbType.Int32, Convert.ToInt32(model[i].roomid));
                        sqlHelper1.AddSetParameterToMySqlCommand("rate_plan", MySqlDbType.Int32, Convert.ToInt32(id));
                        sqlHelper1.AddSetParameterToMySqlCommand("meal_plan", MySqlDbType.Int32, Convert.ToInt32(model[i].mealid));
                        sqlHelper1.AddSetParameterToMySqlCommand("occupancy_id", MySqlDbType.Int32, Convert.ToInt32(model[i].occid));
                        sqlHelper1.AddSetParameterToMySqlCommand("rate1", MySqlDbType.Decimal, Convert.ToDecimal(model[i].rate));
                        sqlHelper1.AddSetParameterToMySqlCommand("userid", MySqlDbType.Int32, Convert.ToInt32(userId));

                        ds = sqlHelper1.GetDatasetByCommand("insert_rateplan_details");

                        sqlHelper1.Dispose();
                    }
                    sqlHelper.Dispose();

                    
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid rateplan details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPost, Route("assign-rateplan")]
        public IActionResult AssignRateplan([FromBody] RPData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"SELECT room.id,title,room_type_id,room.description,type_name FROM room INNER JOIN room_type ON room.room_type_id=room_type.id WHERE property_id={0}
                           ORDER BY room.sort_order, title", model.propertyId
                    );

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);////////////get room list///////////

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {

                        model.roomId = r["id"].ToString();

                        DateTime from = model.fromDate;

                        while (from <= model.toDate)
                        {
                            var isFound = false;
                            query = string.Format(@"SELECT 1 FROM inventory WHERE inv_date='{0}' AND room_id={1} AND property_id={2} AND is_active='Y'",
                                             from.ToString("yyyy-MM-dd"), model.roomId, model.propertyId);

                            ds = sqlHelper.GetDatasetByMySql(query); ///////////////check for data existing in inventory/////////////////
                            if (ds != null)
                            {
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        isFound = true;

                                    }
                                }
                            }
                            if (isFound)   /////////////////if data exists update the row////////////////////
                            {
                                query = string.Format(@"UPDATE inventory SET rateplan={0},updated_by={1},updated_on='{2}' 
                                                        WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                      model.rateplan, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), from.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                            }
                            else    ///////////////if data not exists insert the new row//////////////////
                            {
                                query = string.Format(@"INSERT INTO inventory(inv_date,room_id,property_id, rateplan,created_by, created_on)
                                                        VALUES('{0}',{1},{2},{3},{4},'{5}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                    from.ToString("yyyy-MM-dd"), model.roomId, model.propertyId, model.rateplan, userId, DateTime.Now.ToString("yyyy-MM-dd"));
                            }

                            ds = sqlHelper.GetDatasetByMySql(query);

                            from = from.AddDays(1);
                        }

                        from = model.fromDate;

                    }                    

                    response.ActionStatus = "SUCCESS";
                }
                else
                {
                    return BadRequest("Invalid details");
                }

            }
            catch (Exception ex)//
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }
        [HttpGet, Route("get-assignedplan-list")]
        public IActionResult GetAssignedplanList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<RateplanResult> rateplans = new List<RateplanResult>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select i.id, i.property_id, i.inv_date, IFNULL(i.rateplan, 0) rp_id, IFNULL(rp.name, '') rp_name, IFNULL(rp.color, '#ffffff') rp_color from inventory i
                                            left join rateplans rp on rp.id = i.rateplan
                                            where i.property_id = {0}"
                                   , id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    rateplans.Add(
                        new RateplanResult
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            propertyId = r["property_id"] == DBNull.Value ? "0" : r["property_id"].ToString(),
                            invDate = Convert.ToDateTime(r["inv_date"].ToString()).ToString("yyyy-MM-dd"),
                            rp_id = Convert.ToInt32(r["rp_id"].ToString()),
                            rp_name = r["rp_name"].ToString(),
                            rp_color = r["rp_color"].ToString()
                        });

                }


                response.Data = rateplans;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
