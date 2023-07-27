using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace VTravel.Admin.Controllers
{
    
    [Route("api/inventory")
        , Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS")
        ]
    public class InventoryController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public InventoryController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        

        [HttpGet, Route("get-list")]//
        public IActionResult GetList(int propertyId,string dateFrom,string dateTo)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty,price,extra_bed_price,child_price  FROM inventory WHERE is_active='Y' AND property_id={0} AND inv_date BETWEEN '{1}' AND '{2}'  ORDER BY inv_date"
                                  , propertyId, dateFrom, dateTo);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    inventory.Add(
                        new InvData
                        {
                            id=Convert.ToInt32(r["id"].ToString()),
                            invDate =DateTime.Parse(r["inv_date"].ToString()),                           
                            roomId = r["room_id"].ToString(),
                            propertyId = r["property_id"].ToString(),
                            totalQty = Convert.ToInt32(r["total_qty"].ToString()),
                            bookedQty = Convert.ToInt32(r["booked_qty"].ToString()),
                            price=double.Parse(r["price"].ToString()),                           
                            extraBedPrice = double.Parse(r["extra_bed_price"].ToString()),
                            childPrice = double.Parse(r["child_price"].ToString()),
                        }
                        );

                }


                response.Data = inventory;
                response.ActionStatus = "SUCCESS";
                   
                

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-list-room")]//
        public IActionResult GetListRoom(int propertyId, int roomId)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty,price,extra_bed_price,child_price  FROM inventory WHERE is_active='Y' AND property_id={0} AND room_id={1} AND inv_date >= '{2}'  ORDER BY inv_date"
                                  , propertyId, roomId,DateTime.Today.ToString("yyyy-MM-dd"));

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    inventory.Add(
                        new InvData
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            invDate = DateTime.Parse(r["inv_date"].ToString()),
                            roomId = r["room_id"].ToString(),
                            propertyId = r["property_id"].ToString(),
                            totalQty = Convert.ToInt32(r["total_qty"].ToString()),
                            bookedQty = Convert.ToInt32(r["booked_qty"].ToString()),
                            price = double.Parse(r["price"].ToString()),
                            extraBedPrice = double.Parse(r["extra_bed_price"].ToString()),
                            childPrice = double.Parse(r["child_price"].ToString()),
                        }
                        );

                }


                response.Data = inventory;
                response.ActionStatus = "SUCCESS";//



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpPost, Route("create"), Authorize(Roles = "ADMIN")]
        public IActionResult Create([FromBody] InvData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userId =   claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var isFound = false;
                    var query = string.Format(@"SELECT 1 FROM inventory WHERE inv_date='{0}' AND room_id={1} AND property_id={2} AND is_active='Y'",
                                     model.invDate.ToString("yyyy-MM-dd"),model.roomId, model.propertyId);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
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

                    if (isFound)
                    {
                        //update 
                        if (model.mode == "QTY") {
                            query = string.Format(@"UPDATE inventory SET total_qty={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                       model.totalQty, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                         );

                        }
                        else if (model.mode == "PRICE")
                        {
                            query = string.Format(@"UPDATE inventory SET price={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                      model.price, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }
                        else if (model.mode == "EXTRA_BED_PRICE")
                        {
                            query = string.Format(@"UPDATE inventory SET extra_bed_price={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                      model.extraBedPrice, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }
                        else if (model.mode == "CHILD_PRICE")
                        {
                            query = string.Format(@"UPDATE inventory SET child_price={0},updated_by={1},updated_on='{2}' 
                              WHERE inv_date='{3}' AND room_id={4} AND property_id={5}",
                                      model.childPrice, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }
                        else if (model.mode == "ALL")
                        {
                            query = string.Format(@"UPDATE inventory SET total_qty={0},price={1},extra_bed_price={2},child_price={3},updated_by={4},updated_on='{5}' 
                              WHERE inv_date='{6}' AND room_id={7} AND property_id={8}",
                                     model.totalQty, model.price,model.extraBedPrice,model.childPrice, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId
                                        );
                        }


                    }
                    else
                    {
                        //create
                        query = string.Format(@"INSERT INTO inventory(inv_date,room_id,property_id,total_qty,created_by,price,extra_bed_price,child_price)
                              VALUES('{0}',{1},{2},{3},{4},{5},{6},{7});
                                         SELECT LAST_INSERT_ID() AS id;",
                                    model.invDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                                    model.totalQty, userId,model.price,model.extraBedPrice,model.childPrice);
                    }

                    ds = sqlHelper.GetDatasetByMySql(query);
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

        [HttpPost, Route("create-bulk")]
        public IActionResult CreateBulk([FromBody] InvData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    while (model.fromDate <= model.toDate)
                    {
                        model.invDate = model.fromDate;
                        model.mode = "ALL";
                        this.Create(model);
                        model.fromDate=model.fromDate.AddDays(1);
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

                    var query = string.Format(@"UPDATE reservation SET is_active='N' WHERE id={0}",
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


