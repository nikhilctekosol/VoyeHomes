using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.HostWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace VTravel.HostWeb.Controllers
{
    
    [Route("api/inventory"),
    Authorize//
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

                var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty  FROM inventory WHERE is_active='Y' AND property_id={0} AND inv_date BETWEEN '{1}' AND '{2}'  ORDER BY inv_date"
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
        public IActionResult GetListRoom( int roomId)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                //var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty  FROM inventory WHERE is_active='Y'
                //        AND  property_id in(select property_id from partner_user_property where partner_user_id={0}) AND room_id={1} AND inv_date >= '{2}'  ORDER BY inv_date"
                //                  , userId, roomId, DateTime.Today.ToString("yyyy-MM-dd")); 
                var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id,	IFNULL(r.noofrooms,0) total_qty,i.booked_qty,i.price price1,i.extra_bed_price,i.child_price, IFNULL(rb.rate, 0) price, rb.rateplan,
                                            GROUP_CONCAT(concat(o.occupancy, ' - ', rb1.rate)) AS occ_rates FROM inventory i
                                            left join rateplan_breakup rb on rb.rateplan =  i.rateplan and rb.room_id = i.room_id and rb.occupancy = 2
                                            left join rateplan_breakup rb1 on rb1.rateplan =  i.rateplan and rb1.room_id = i.room_id 
                                            left join occupancy o on o.id = rb1.occupancy
                                            left join room r on i.room_id = r.id
                                            WHERE i.is_active='Y' AND i.property_id in(select property_id from partner_user_property where partner_user_id={0}) AND i.room_id={1} AND i.inv_date >= '{2}'
                                            group by i.id, i.inv_date, i.room_id, i.property_id, r.noofrooms, i.booked_qty, i.price, i.extra_bed_price, i.child_price, rb.rate, rb.rateplan 
                                            ORDER BY i.inv_date;"
                                  , userId, roomId, DateTime.Today.ToString("yyyy-MM-dd"));

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


       

      

    }
}


