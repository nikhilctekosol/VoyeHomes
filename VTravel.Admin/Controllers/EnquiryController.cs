using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace VTravel.Admin.Controllers
{
    
    [Route("api/enquiry"),
        Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS")
        
        ]
    public class EnquiryController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public EnquiryController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        

        [HttpGet, Route("get-list")]//
        public IActionResult GetList(string enquiry_status,int page_no,int rows_count)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                var enquiry_status_txt = string.Empty;
                if (enquiry_status != "ALL")
                {
                    enquiry_status_txt = string.Format(" AND enquiry_status='{0}'", enquiry_status);
                }
                int offset = page_no * rows_count;
                var query = string.Format(@"select t1.id,t1.cust_id,t1.property_id,convert_tz(t1.created_on,@@session.time_zone,'+05:30') AS created_on ,t1.checkin_date,t1.checkout_date,t1.adults_count,t1.children_count
               ,t1.booking_channel_id,t1.assigned_user,t1.reservation_id,t1.enquiry_status
              ,t2.cust_name,t2.cust_email,t2.cust_phone, t3.title as property_title,t4.channel_name AS booking_channel_name
              ,t5.user_name AS assigned_user_name ,t5.name_of_user AS assigned_name_of_user
                 FROM enquiry t1 LEFT JOIN customer t2 ON t1.cust_id=t2.id
                 LEFT JOIN property t3 ON t1.property_id=t3.id 
                 LEFT JOIN booking_channel t4 ON t1.booking_channel_id=t4.id 
                LEFT JOIN admin_user t5 ON t1.assigned_user=t5.id                 
               WHERE t1.is_active='Y' {0} ORDER BY t1.id DESC 
               LIMIT {2} OFFSET {3};
          SELECT count(1) AS total_count, {1} AS page_no,{2} AS rows_count FROM enquiry WHERE is_active='Y' {0}"
             , enquiry_status_txt, page_no,rows_count, offset);
                                                
               

                using (var connection = new MySqlConnection(Startup.conStr))
                {

                    var results = connection.QueryMultiple(query);
                    response.Data = new { record_list = (List<EnqData>)results.Read<EnqData>()
                                         ,record_count= (List<RecordCount>)results.Read<RecordCount>()
                    };
                    response.ActionStatus = "SUCCESS";

                }

                

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPost, Route("create")]
        public IActionResult Create([FromBody] ReservData model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    if(checkIfInventory(model.fromDate,model.toDate,0,int.Parse(model.roomId),int.Parse(model.propertyId)
                        ,int.Parse(model.noOfRooms)))
                    {
                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        MySqlHelper sqlHelper = new MySqlHelper();

                        var query = string.Format(@"INSERT INTO reservation(from_date,to_date,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id,details,created_by,noOfRooms,no_of_guests,final_amount,enquiry_ref)
                              VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},{11},{12},'{13}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                         model.fromDate.ToString("yyyy-MM-dd"), model.toDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                                         model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId,model.noOfRooms,model.noOfGuests,model.finalAmount,model.enquiry_ref);

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
                                   
                                }
                            }
                        }
                        //reduce inventory

                        if(updateInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                        , int.Parse(model.noOfRooms)))
                        {
                            response.ActionStatus = "SUCCESS";
                        }

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

        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] ReservData model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();//
                    IEnumerable<Claim> claims = User.Claims;
                    var userId =  claims.Where(c => c.Type == "id").FirstOrDefault().Value;



                    var query = string.Format(@"UPDATE reservation SET cust_name='{0}',cust_email='{1}',cust_phone='{2}',booking_channel_id={3},details='{4}',updated_by={5}, updated_on='{6}',no_of_guests={7},final_amount={8},enquiry_ref='{9}' WHERE id={10}",
                                    model.custName, model.custEmail, model.custPhone, model.bookingChannelId,
                                    model.details, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),model.noOfGuests,model.finalAmount,model.enquiry_ref, id);

                    var ds = sqlHelper.GetDatasetByMySql(query);

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

                    var query = string.Format(@"UPDATE reservation SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);

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
                                    var propertyId = Convert.ToInt32(r["property_id"].ToString());
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
               

                List<ReservData> reservations =(List<ReservData>)(((ApiResponse)((OkObjectResult)this.GetList("roomId", propertyId,0)).Value).Data);
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


