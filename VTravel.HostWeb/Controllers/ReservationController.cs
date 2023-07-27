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
    
    [Route("api/reservation"),
     Authorize
        ]
    public class ReservationController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReservationController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        

        [HttpGet, Route("get-list")]//
        public IActionResult GetList(int roomId)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;


                List<ReservData> reservations = new List<ReservData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,from_date,to_date,customer_id,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id,details,noOfRooms,no_of_guests,is_host_booking  
                 FROM reservation WHERE is_active='Y' AND room_id={0} 
               AND  property_id in(select property_id from partner_user_property where partner_user_id={1}) AND from_date > '{2}'  ORDER BY from_date"
                                  , roomId, userId, DateTime.Today.AddDays(-60).ToString("yyyy-MM-dd"));

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    reservations.Add(
                        new ReservData
                        {
                            id=Convert.ToInt32(r["id"].ToString()),
                            fromDate =DateTime.Parse(r["from_date"].ToString()),
                            toDate = DateTime.Parse(r["to_date"].ToString()),
                            customerId = r["customer_id"].ToString(),
                            roomId = r["room_id"].ToString(),
                            propertyId = r["property_id"].ToString(),
                            custName = r["cust_name"].ToString(),
                            custEmail = r["cust_email"].ToString(),
                            custPhone = r["cust_phone"].ToString(),
                            bookingChannelId= r["booking_channel_id"].ToString(),
                            details = r["details"].ToString(),
                            noOfRooms = r["noOfRooms"].ToString(),
                            isHostBooking = r["is_host_booking"].ToString(),
                            noOfGuests = String.IsNullOrEmpty(r["no_of_guests"].ToString()) ? 0 : int.Parse(r["no_of_guests"].ToString()),
                        }
                        );

                }


                response.Data = reservations;
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
        public IActionResult Create([FromBody] ReservData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                //check if user has permmision to this property
                var propertyId = 0;
                MySqlHelper sqlHelper = new MySqlHelper();
                var query = string.Format(@"select property_id from partner_user_property where property_id={0} and partner_user_id={1}",model.propertyId, userId);
                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            propertyId = Convert.ToInt32(r["property_id"].ToString());
                           

                        }
                    }
                }

                if (model != null && propertyId==int.Parse(model.propertyId))
                {
                    if(checkIfInventory(model.fromDate,model.toDate,0,int.Parse(model.roomId),int.Parse(model.propertyId)
                        ,int.Parse(model.noOfRooms)))
                    {

                        if (updateInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                       , int.Parse(model.noOfRooms)))
                        {

                            sqlHelper = new MySqlHelper();

                            query = string.Format(@"INSERT INTO reservation(from_date,to_date,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id,details,created_by,noOfRooms,is_host_booking,no_of_guests)
                              VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},'{11}',{12});
                                         SELECT LAST_INSERT_ID() AS id;",
                                            model.fromDate.ToString("yyyy-MM-dd"), model.toDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                                            model.custName, model.custEmail, model.custPhone, 45, model.details, userId, model.noOfRooms, 'Y', model.noOfGuests);

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

                            
                        }
                       

                        
                        //reduce inventory

                       

                    }
                    else
                    {
                        response.Message = "Inventory not available";
                    }
                    

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

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] ReservData model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                //get model.propertyId
                MySqlHelper sqlHelper = new MySqlHelper();
                var query = string.Format(@"select property_id from reservation where id={0}", id);
                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            model.propertyId = r["property_id"].ToString();


                        }
                    }
                }

                //check if user has permmision to this property
                var propertyId = 0;
                sqlHelper = new MySqlHelper();
                query = string.Format(@"select property_id from partner_user_property where property_id={0} and partner_user_id={1}", model.propertyId, userId);
                ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            propertyId = Convert.ToInt32(r["property_id"].ToString());


                        }
                    }
                }

                if (model != null && propertyId == int.Parse(model.propertyId))
                {

                    sqlHelper = new MySqlHelper();
                    

                     query = string.Format(@"UPDATE reservation SET cust_name='{0}',cust_email='{1}',cust_phone='{2}',details='{3}',updated_by={4}, updated_on='{5}', no_of_guests={6} WHERE id={7} AND is_host_booking='Y' AND property_id={8}",
                                    model.custName, model.custEmail, model.custPhone,
                                    model.details, userId, DateTime.Now.ToString("yyyy-MM-dd"),model.noOfGuests, id,propertyId);

                     ds = sqlHelper.GetDatasetByMySql(query);

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

        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete")]
        public IActionResult Delete(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                ReservData model = new ReservData();

                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                //get model.propertyId
                MySqlHelper sqlHelper = new MySqlHelper();
                var query = string.Format(@"select property_id from reservation where id={0}", id);
                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            model.propertyId = r["property_id"].ToString();


                        }
                    }
                }
                var propertyId = 0;
                sqlHelper = new MySqlHelper();
                query = string.Format(@"select property_id from partner_user_property where property_id={0} and partner_user_id={1}", model.propertyId, userId);
                ds = sqlHelper.GetDatasetByMySql(query);
                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow r = ds.Tables[0].Rows[0];
                            propertyId = Convert.ToInt32(r["property_id"].ToString());


                        }
                    }
                }

                if (id > 0 && propertyId == int.Parse(model.propertyId))
                {

                    sqlHelper = new MySqlHelper();

                    query = string.Format(@"UPDATE reservation SET is_active='N' WHERE is_host_booking='Y' AND id={0} AND property_id={1}",
                           id, propertyId);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    //increase inventory
                    query = string.Format(@"select * 
                     FROM reservation WHERE id={0}", id);

                     ds = sqlHelper.GetDatasetByMySql(query);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    var noOfRooms = Convert.ToInt32(r["noOfRooms"].ToString());                                    
                                    var roomId = Convert.ToInt32(r["room_id"].ToString());
                                    var startDate = DateTime.Parse(r["from_date"].ToString());
                                    var endDate = DateTime.Parse(r["to_date"].ToString());
                                    while (startDate < endDate)
                                    {
                                        query = string.Format(@"update inventory set booked_qty=booked_qty-{0} WHERE is_active='Y' AND property_id={1} AND room_id={2} AND inv_date='{3}'"
                                        , noOfRooms, propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                                        ds = sqlHelper.GetDatasetByMySql(query);
                                        startDate = startDate.AddDays(1);
                                    }
                                    

                                }
                            }
                        }
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

        bool checkIfReservation(DateTime startDate, DateTime endDate, int thisId,int roomId,int propertyId)
        {

            try
            {
               

                List<ReservData> reservations =(List<ReservData>)(((ApiResponse)((OkObjectResult)this.GetList(roomId)).Value).Data);
                foreach (var  reservData in reservations)
                {
                    if (thisId != reservData.id)
                    {
                        var resFromDate = reservData.fromDate;
                        var resToDate = reservData.toDate;

                        if (((startDate >= resFromDate) && (startDate < resToDate))
                          || ((endDate > resFromDate) && (endDate <= resToDate)))
                        {
                            return true;
                        }
                        else if (((resFromDate >= startDate) && (resFromDate < endDate))
                          || ((resToDate > startDate) && (resToDate <= endDate)))
                        {
                            return true;
                        }
                    }
                   
                }
            }
            catch(Exception ex)
            {
                return true;
            }

            return false;
        }

        bool checkIfInventory(DateTime startDate, DateTime endDate, int thisQty, int roomId, int propertyId, int noOfRooms)
        {

            try
            {
                bool hasInventory = true;

                
                MySqlHelper sqlHelper = new MySqlHelper();

                while(hasInventory&&(startDate < endDate))
                {
                    //check inventory for the day
                    hasInventory = false;
                    var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty  FROM inventory WHERE is_active='Y' AND property_id={0} AND room_id={1} AND inv_date='{2}'"
                                  , propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null){
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    var totalQty = Convert.ToInt32(r["total_qty"].ToString());
                                    var bookedQty = Convert.ToInt32(r["booked_qty"].ToString());
                                    var availableQty = totalQty - bookedQty+thisQty;
                                    if (availableQty >= noOfRooms)
                                    {
                                        hasInventory = true;
                                        
                                    }
                                }
                            }
                        }
                    }
                    startDate = startDate.AddDays(1);
                }
                return hasInventory;
            }
            catch (Exception ex)
            {
                
            }

            return false;
        }
        bool updateInventory(DateTime startDate, DateTime endDate, int thisQty, int roomId, int propertyId, int noOfRooms)
        {

            try
            {
                bool hasInventory = true;

                List<InvData> inventory = new List<InvData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                while (hasInventory && (startDate < endDate))
                {
                    //check inventory for the day
                    hasInventory = false;
                    var query = string.Format(@"select id,inv_date,room_id,property_id,	total_qty,booked_qty  FROM inventory WHERE is_active='Y' AND property_id={0} AND room_id={1} AND inv_date='{2}'"
                                  , propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    var totalQty = Convert.ToInt32(r["total_qty"].ToString());
                                    var bookedQty = Convert.ToInt32(r["booked_qty"].ToString());
                                    var availableQty = totalQty - bookedQty + thisQty;
                                    if (availableQty >= noOfRooms)
                                    {
                                        

                                        query = string.Format(@"update inventory set booked_qty=booked_qty+{0} WHERE is_active='Y' AND property_id={1} AND room_id={2} AND inv_date='{3}'"
                                        ,noOfRooms-thisQty, propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                                        ds = sqlHelper.GetDatasetByMySql(query);
                                        hasInventory = true;

                                    }
                                }
                            }
                        }
                    }
                    startDate = startDate.AddDays(1);
                }
                return hasInventory;
            }
            catch (Exception ex)
            {

            }

            return false;
        }
    }
}


