using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MySql.Data.MySqlClient;
using Dapper;
using System.Transactions;

namespace VTravel.Admin.Controllers
{
    
    [Route("api/reservation")
        ,Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS")
        
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
        public IActionResult GetList(int roomId, int propertyId)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ReservData> reservations = new List<ReservData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select t1.id,t1.from_date,t1.to_date,t1.customer_id,t1.room_id,t1.property_id,t1.cust_name,t1.cust_email,t1.cust_phone,t1.booking_channel_id,t1.details
              ,t1.noOfRooms,t1.no_of_guests,t1.final_amount, t1.is_host_booking,t1.created_on,t1.updated_on,t1.enquiry_ref,t1.res_status,t2.user_name,t2.name_of_user ,t3.user_name AS updated_user_name,t3.name_of_user  AS updated_name_of_user
                , t1.advancepayment, t1.partpayment, t1.balancepayment
                 FROM reservation t1 LEFT JOIN admin_user t2 ON t1.created_by=t2.id
                 LEFT JOIN admin_user t3 ON t1.updated_by=t3.id 
               WHERE t1.is_active='Y' AND t1.room_id={0} AND t1.property_id={1} AND t1.from_date > '{2}'  ORDER BY from_date"
                                  , roomId, propertyId, DateTime.Today.AddDays(-60).ToString("yyyy-MM-dd"));

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                    //DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);

                    reservations.Add(
                        new ReservData
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            fromDate = DateTime.Parse(r["from_date"].ToString()),
                            toDate = DateTime.Parse(r["to_date"].ToString()),
                            customerId = r["customer_id"].ToString(),
                            roomId = r["room_id"].ToString(),
                            propertyId = r["property_id"].ToString(),
                            custName = r["cust_name"].ToString(),
                            custEmail = r["cust_email"].ToString(),
                            custPhone = r["cust_phone"].ToString(),
                            bookingChannelId = r["booking_channel_id"].ToString(),
                            details = r["details"].ToString(),
                            noOfRooms = r["noOfRooms"].ToString(),
                            isHostBooking = r["is_host_booking"].ToString(),
                            noOfGuests = String.IsNullOrEmpty(r["no_of_guests"].ToString()) ? 0 : int.Parse(r["no_of_guests"].ToString()),
                            finalAmount = String.IsNullOrEmpty(r["final_amount"].ToString()) ? 0 : float.Parse(r["final_amount"].ToString()),
                            advancepayment = String.IsNullOrEmpty(r["advancepayment"].ToString()) ? 0 : float.Parse(r["advancepayment"].ToString()),
                            partpayment = String.IsNullOrEmpty(r["partpayment"].ToString()) ? 0 : float.Parse(r["partpayment"].ToString()),
                            balancepayment = String.IsNullOrEmpty(r["balancepayment"].ToString()) ? 0 : float.Parse(r["balancepayment"].ToString()),
                            created_on = String.IsNullOrEmpty(r["created_on"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["created_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
                            updated_on = String.IsNullOrEmpty(r["updated_on"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["updated_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
                            created_by = r["is_host_booking"].ToString() == "Y" ? "Host" : r["user_name"].ToString() + "/" + r["name_of_user"].ToString(),
                            updated_by = r["is_host_booking"].ToString() == "Y" ? "NA" : r["updated_user_name"].ToString() + "/" + r["updated_name_of_user"].ToString(),
                            enquiry_ref = r["enquiry_ref"].ToString(),
                            res_status = r["res_status"].ToString()
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

        [HttpGet, Route("get-list-new")]//
        public IActionResult GetListNew(int roomId, int propertyId)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ReservData> reservations = new List<ReservData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select t1.id,t4.from_date,t4.to_date,t1.customer_id,t4.room_id,t1.property_id,t1.cust_name,t1.cust_email,t1.cust_phone,t1.booking_channel_id,t1.details
                ,t1.noOfRooms,t1.no_of_guests,t1.final_amount, t1.is_host_booking,t1.created_on,t1.updated_on,t1.enquiry_ref,t1.res_status,t2.user_name,t2.name_of_user ,t3.user_name AS updated_user_name,t3.name_of_user  AS updated_name_of_user
                , t1.advancepayment, t1.partpayment, t1.balancepayment, t1.discount, t1.commission, t1.country, t5.country_name
                FROM reservation t1 LEFT JOIN admin_user t2 ON t1.created_by=t2.id
                LEFT JOIN admin_user t3 ON t1.updated_by=t3.id 
                LEFT JOIN reserve_rooms t4 on t4.reservation_id = t1.id
                LEFT JOIN country t5 on t5.id = t1.country
                WHERE t1.is_active='Y' AND t4.room_id={0} AND t1.property_id={1} AND t4.from_date > '{2}'  ORDER BY from_date"
                                  , roomId, propertyId, DateTime.Today.AddDays(-60).ToString("yyyy-MM-dd"));

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                    //DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);

                    reservations.Add(
                        new ReservData
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            fromDate = DateTime.Parse(r["from_date"].ToString()),
                            toDate = DateTime.Parse(r["to_date"].ToString()),
                            customerId = r["customer_id"].ToString(),
                            roomId = r["room_id"].ToString(),
                            propertyId = r["property_id"].ToString(),
                            custName = r["cust_name"].ToString(),
                            custEmail = r["cust_email"].ToString(),
                            custPhone = r["cust_phone"].ToString(),
                            bookingChannelId = r["booking_channel_id"].ToString(),
                            details = r["details"].ToString(),
                            noOfRooms = r["noOfRooms"].ToString(),
                            isHostBooking = r["is_host_booking"].ToString(),
                            noOfGuests = String.IsNullOrEmpty(r["no_of_guests"].ToString()) ? 0 : int.Parse(r["no_of_guests"].ToString()),
                            finalAmount = String.IsNullOrEmpty(r["final_amount"].ToString()) ? 0 : float.Parse(r["final_amount"].ToString()),
                            advancepayment = String.IsNullOrEmpty(r["advancepayment"].ToString()) ? 0 : float.Parse(r["advancepayment"].ToString()),
                            partpayment = String.IsNullOrEmpty(r["partpayment"].ToString()) ? 0 : float.Parse(r["partpayment"].ToString()),
                            balancepayment = String.IsNullOrEmpty(r["balancepayment"].ToString()) ? 0 : float.Parse(r["balancepayment"].ToString()),
                            discount = String.IsNullOrEmpty(r["discount"].ToString()) ? 0 : float.Parse(r["discount"].ToString()),
                            commission = String.IsNullOrEmpty(r["commission"].ToString()) ? 0 : float.Parse(r["commission"].ToString()),
                            country = r["country"].ToString(),
                            created_on = String.IsNullOrEmpty(r["created_on"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["created_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
                            updated_on = String.IsNullOrEmpty(r["updated_on"].ToString()) ? ""
                            : TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(r["updated_on"].ToString()), timeZoneInfo).ToString("dd/MMM/yyyy HH:mm"),
                            created_by = r["is_host_booking"].ToString() == "Y" ? "Host" : r["user_name"].ToString() + "/" + r["name_of_user"].ToString(),
                            updated_by = r["is_host_booking"].ToString() == "Y" ? "NA" : r["updated_user_name"].ToString() + "/" + r["updated_name_of_user"].ToString(),
                            enquiry_ref = r["enquiry_ref"].ToString(),
                            res_status = r["res_status"].ToString()
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

        [HttpGet, Route("get-reserve-rooms")]//
        public IActionResult GetReserveRooms(int Id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ReservedRoomData> reservedrooms = new List<ReservedRoomData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select t1.id, t1.from_date, t1.to_date, t1.room_id, t2.title, t1.years06, t1.years612, t1.years12, t1.amount from reserve_rooms t1
                                            left join room t2 on t2.id = t1.room_id
                                            where t1.reservation_id = '{0}'  ORDER BY t1.id"
                                  , Id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

                    //DateTime istNow = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, timeZoneInfo);

                    reservedrooms.Add(
                        new ReservedRoomData
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            fromDate = DateTime.Parse(r["from_date"].ToString()),
                            toDate = DateTime.Parse(r["to_date"].ToString()),
                            roomId = r["room_id"].ToString(),
                            room = r["title"].ToString(),
                            years06 = Convert.ToInt32(r["years06"].ToString()),
                            years612 = Convert.ToInt32(r["years612"].ToString()),
                            years12 = Convert.ToInt32(r["years12"].ToString()),
                            noOfGuests = Convert.ToInt32(r["years06"].ToString()) + Convert.ToInt32(r["years612"].ToString()) + Convert.ToInt32(r["years12"].ToString()),
                            amount = Convert.ToInt32(r["amount"].ToString())
                        }
                        );

                }


                response.Data = reservedrooms;
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

                       
                        //reduce inventory

                        if(updateInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                        , int.Parse(model.noOfRooms)))
                        {
                            MySqlHelper sqlHelper = new MySqlHelper();

                            var query = string.Format(@"INSERT INTO reservation(from_date,to_date,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id
                                        ,details,created_by,noOfRooms,no_of_guests,final_amount,enquiry_ref, advancepayment, partpayment, balancepayment)
                                        VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},{11},{12},'{13}','{14}','{15}','{16}');
                                        SELECT LAST_INSERT_ID() AS id;",
                                             model.fromDate.ToString("yyyy-MM-dd"), model.toDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                                             model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId, model.noOfRooms, model.noOfGuests, model.finalAmount, model.enquiry_ref
                                             , model.advancepayment, model.partpayment, model.balancepayment);

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

        [HttpPost, Route("create-new")]
        public IActionResult CreateNew([FromBody] ReservDataNew model)
        {
            ApiResponse response = new ApiResponse();//
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            using (var scope = new TransactionScope())
            {
                try
                {
                    if (model != null)
                    {
                        for(int i=0; i<model.rooms.Count; i++)
                        {
                            if (!checkIfInventory(model.rooms[i].fromDate, model.rooms[i].toDate, 0, int.Parse(model.rooms[i].roomId), int.Parse(model.propertyId), model.rooms.Select(r => r.roomId = model.rooms[i].roomId).Count()))
                            {
                                response.ActionStatus = "EXCEPTION";
                                response.Message = "Inventory doesn't matches the reservation!";
                                throw new Exception();
                            }
                        }

                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        for (int i = 0; i < model.rooms.Count; i++)
                        {
                            if(!updateInventory(model.rooms[i].fromDate, model.rooms[i].toDate, 0, int.Parse(model.rooms[i].roomId), int.Parse(model.propertyId), 1)){
                                response.ActionStatus = "EXCEPTION";
                                response.Message = "Something went wrong in inventory update!";
                                throw new Exception();
                            }
                        }

                        MySqlHelper sqlHelper = new MySqlHelper();

                        var query = string.Format(@"INSERT INTO reservation(property_id,cust_name,cust_email,cust_phone,booking_channel_id
                                                ,details,created_by,noOfRooms,no_of_guests,final_amount,enquiry_ref, advancepayment, partpayment, balancepayment, discount, commission, country)
                                                VALUES({0},'{1}','{2}','{3}',{4},'{5}',{6},{7},{8},{9},'{10}',{11},{12},{13},{14},{15}, '{16}');
                                                SELECT LAST_INSERT_ID() AS id;",
                                         model.propertyId,
                                         model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId, model.noOfRooms, model.noOfGuests, model.finalAmount, model.enquiry_ref
                                         , model.advancepayment, model.partpayment, model.balancepayment, model.discount, model.commission, model.country);

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);
                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow r = ds.Tables[0].Rows[0];
                                    model.id = Convert.ToInt32(r["id"].ToString());

                                    for (int i = 0; i < model.rooms.Count; i++)
                                    {
                                        query = string.Format(@"INSERT INTO reserve_rooms(reservation_id,from_date,to_date,room_id,years06, years612, years12, amount)
                                                VALUES({0},'{1}','{2}',{3},{4},{5},{6},{7});",
                                         model.id, model.rooms[i].fromDate.ToString("yyyy-MM-dd"), model.rooms[i].toDate.ToString("yyyy-MM-dd"), model.rooms[i].roomId, model.rooms[i].years06, model.rooms[i].years612, model.rooms[i].years12, model.rooms[i].amount);

                                        DataSet ds1 = sqlHelper.GetDatasetByMySql(query);
                                    }



                                    response.Data = model;
                                    response.ActionStatus = "SUCCESS";

                                }
                            }
                        }
                    }


                    //if (model != null)
                    //{
                    //    if (checkIfInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                    //        , int.Parse(model.noOfRooms)))
                    //    {
                    //        IEnumerable<Claim> claims = User.Claims;
                    //        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;


                    //        //reduce inventory

                    //        if (updateInventory(model.fromDate, model.toDate, 0, int.Parse(model.roomId), int.Parse(model.propertyId)
                    //        , int.Parse(model.noOfRooms)))
                    //        {
                    //            MySqlHelper sqlHelper = new MySqlHelper();

                    //            var query = string.Format(@"INSERT INTO reservation(from_date,to_date,room_id,property_id,cust_name,cust_email,cust_phone,booking_channel_id
                    //                        ,details,created_by,noOfRooms,no_of_guests,final_amount,enquiry_ref, advancepayment, partpayment, balancepayment)
                    //                        VALUES('{0}','{1}',{2},{3},'{4}','{5}','{6}',{7},'{8}',{9},{10},{11},{12},'{13}','{14}','{15}','{16}');
                    //                        SELECT LAST_INSERT_ID() AS id;",
                    //                             model.fromDate.ToString("yyyy-MM-dd"), model.toDate.ToString("yyyy-MM-dd"), model.roomId, model.propertyId,
                    //                             model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId, model.noOfRooms, model.noOfGuests, model.finalAmount, model.enquiry_ref
                    //                             , model.advancepayment, model.partpayment, model.balancepayment);

                    //            DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    //            if (ds != null)
                    //            {
                    //                if (ds.Tables.Count > 0)
                    //                {
                    //                    if (ds.Tables[0].Rows.Count > 0)
                    //                    {
                    //                        DataRow r = ds.Tables[0].Rows[0];
                    //                        model.id = Convert.ToInt32(r["id"].ToString());
                    //                        response.Data = model;
                    //                        response.ActionStatus = "SUCCESS";

                    //                    }
                    //                }
                    //            }

                    //        }

                    //    }
                    //    else
                    //    {
                    //        response.Message = "Inventory not available";
                    //    }


                    //}
                    //else
                    //{
                    //    return BadRequest("Invalid details");
                    //}

                    scope.Complete();
                }
                catch (Exception ex)//
                {
                    response.ActionStatus = "EXCEPTION";
                    response.Message = "Something went wrong";
                }

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



                    var query = string.Format(@"UPDATE reservation SET cust_name='{0}',cust_email='{1}',cust_phone='{2}',booking_channel_id={3},details='{4}'
                                    ,updated_by={5}, updated_on='{6}',no_of_guests={7},final_amount={8},enquiry_ref='{9}',advancepayment='{11}'
                                    ,partpayment='{12}',balancepayment='{13}' WHERE id={10}",
                                    model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId
                                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),model.noOfGuests,model.finalAmount,model.enquiry_ref, id
                                    , model.advancepayment, model.partpayment, model.balancepayment);

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

        [HttpPut, Route("update-new")]
        public IActionResult UpdateNew([FromBody] ReservDataNew model, int id)
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
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;



                    var query = string.Format(@"UPDATE reservation SET cust_name='{0}',cust_email='{1}',cust_phone='{2}',booking_channel_id={3},details='{4}'
                                    ,updated_by={5}, updated_on='{6}',no_of_guests={7},final_amount={8},enquiry_ref='{9}',advancepayment='{11}'
                                    ,partpayment='{12}',balancepayment='{13}',discount={14},commission={15}, country = '{16}' WHERE id={10}",
                                    model.custName, model.custEmail, model.custPhone, model.bookingChannelId, model.details, userId
                                    , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.noOfGuests, model.finalAmount, model.enquiry_ref, id
                                    , model.advancepayment, model.partpayment, model.balancepayment, model.discount, model.commission, model.country);

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


        [Authorize(Roles = "ADMIN")]
        [HttpDelete, Route("delete-new")]
        public IActionResult DeleteNew(int id)
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
                    query = string.Format(@"select t1.id, t2.from_date, t2.to_date, 1 noOfRooms, t1.property_id, t2.room_id 
                     FROM reservation t1
                     LEFT JOIN reserve_rooms t2 on t2.reservation_id = t1.id WHERE t1.id={0}", id);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    if (ds != null)
                    {
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows != null)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                                    {
                                        DataRow r = ds.Tables[0].Rows[i];
                                        var noOfRooms = Convert.ToInt32(r["noOfRooms"].ToString());
                                        var propertyId = Convert.ToInt32(r["property_id"].ToString());
                                        var roomId = Convert.ToInt32(r["room_id"].ToString());
                                        var startDate = DateTime.Parse(r["from_date"].ToString());
                                        var endDate = DateTime.Parse(r["to_date"].ToString());
                                        while (startDate < endDate)
                                        {
                                            query = string.Format(@"update inventory set booked_qty=booked_qty-{0} WHERE is_active='Y' AND property_id={1} AND room_id={2} AND inv_date='{3}'"
                                            , noOfRooms, propertyId, roomId, startDate.ToString("yyyy-MM-dd"));

                                            DataSet ds1 = sqlHelper.GetDatasetByMySql(query);
                                            startDate = startDate.AddDays(1);
                                        }
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
               

                List<ReservData> reservations =(List<ReservData>)(((ApiResponse)((OkObjectResult)this.GetList(roomId, propertyId)).Value).Data);
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
                if (startDate < DateTime.Today)
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userRole = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault().Value;

                    if (userRole != "ADMIN")
                    {
                        return false;
                    }
                    
                }
                bool hasInventory = true;

                
                MySqlHelper sqlHelper = new MySqlHelper();

                while(hasInventory&&(startDate < endDate))
                {
                    //check inventory for the day
                    hasInventory = false;
                    var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id, r.noofrooms	total_qty,i.booked_qty  FROM inventory i
                                                left join room r on r.id = i.room_id WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date='{2}'"
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
                    var query = string.Format(@"select i.id,i.inv_date,i.room_id,i.property_id, r.noofrooms	total_qty,i.booked_qty  FROM inventory i
                                                left join room r on r.id = i.room_id WHERE i.is_active='Y' AND i.property_id={0} AND i.room_id={1} AND i.inv_date='{2}'"
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


        
        [HttpPut, Route("add-doc"), DisableRequestSizeLimit]
        public async Task<IActionResult> AddDoc(int resid,int doctype)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;
            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                var files = Request.Form.Files;
               

                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        var guid = Guid.NewGuid().ToString();
                        var filePath = Path.Combine(_hostingEnvironment.WebRootPath, guid + file.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {

                            await file.CopyToAsync(fileStream);
                        }


                        Account account = new Account(
                             General.GetSettingsValue("cdy_cloud_name"),
                             General.GetSettingsValue("cdy_api_key"),
                             General.GetSettingsValue("cdy_api_secret"));
                        Cloudinary cloudinary = new Cloudinary(account);


                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, filePath),
                            Folder = "reservation/" + resid + "/docs",
                            Overwrite = true,
                            PublicId = guid,
                            Invalidate = true
                        };


                        var uploadResult = cloudinary.Upload(uploadParams);
                        System.IO.File.Delete(filePath);

                        if (uploadResult.Url != null)
                        {
                            var query = string.Format(@"INSERT INTO res_document(res_id,url,public_id,doc_type_id,created_by,file_name) VALUES({0},'{1}','{2}',{3},{4},'{5}')",
                                   resid, uploadResult.SecureUrl, guid,doctype, userId, file.FileName);

                            using (var connection = new MySqlConnection(Startup.conStr))
                            {//

                                var results = connection.Query(query);
                                //response.Data = (List<DocType>)results;
                                response.ActionStatus = "SUCCESS";

                            }

                            
                        }
                    }




                }
                else
                {
                    return BadRequest("invalid file details");
                }
            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong " + ex.Message;
            }

            return new OkObjectResult(response);

        }

        [HttpGet, Route("get-doc-list")]
        public IActionResult GetList(int resid)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {



                var query = string.Format(@"select t1.id,t1.doc_type_id,t1.url,t1.file_name,t2.doc_type_name 
                 FROM res_document t1 LEFT JOIN doc_type t2 ON t1.doc_type_id=t2.id 
                WHERE t1.is_active='Y' AND t1.res_id={0} ORDER BY t1.sort_order"
                                   ,resid);

                using (var connection = new MySqlConnection(Startup.conStr))
                {

                    var results = connection.Query<ResDoc>(query);
                    response.Data = (List<ResDoc>)results;
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

        [HttpDelete, Route("delete-doc")]
        public IActionResult DeleteDoc(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {



                var query = string.Format(@"update  res_document SET is_active='N' WHERE id={0}"
                                   , id);

                using (var connection = new MySqlConnection(Startup.conStr))
                {

                    var results = connection.Query(query);
                    
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


        [HttpPut, Route("update-status")]
        public IActionResult UpdateStatus([FromBody] ReservData model, int id)
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

                    //verify docs
                    if (checkIfDocument(id))
                    {
                        var query = string.Format(@"UPDATE reservation SET res_status='{0}',updated_by={1}, updated_on='{2}' WHERE id={3}",
                                  model.res_status, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), id);


                        using (var connection = new MySqlConnection(Startup.conStr))
                        {

                            var results = connection.Query(query);

                            response.ActionStatus = "SUCCESS";

                        }

                    }
                    else
                    {
                        response.Message = "Please update all required documents";
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

        bool checkIfDocument(int id)
        {

            try
            {

                var idProofCountRequired = 1;
                var advancePaymentCountRequired = 1;
                var finalPaymentCountRequired = 1;

                var idProofCountAvailable = 0;
                var advancePaymentCountAvailable = 0;
                var finalPaymentCountAvailable = 0;

                


                using (var connection = new MySqlConnection(Startup.conStr))
                {

                    var query = string.Format(@"SELECT doc_type_id,count(1) AS doc_count FROM res_document where res_id={0} AND is_active='Y' group by doc_type_id"
                                , id);
                     //idProofCountRequired = connection.ExecuteScalar<int>
                     //   (string.Format("SELECT no_of_guests FROM reservation WHERE id={0}",id));
                    var results = connection.Query<DocCount>(query);
                    var countList = (List<DocCount>)results;
                    foreach (var docCount in countList)
                    {
                        if (docCount.doc_type_id == 1)
                        {
                            idProofCountAvailable+= docCount.doc_count;
                        }
                        else if (docCount.doc_type_id == 4)
                        {
                            advancePaymentCountAvailable += docCount.doc_count;
                        }
                        else if (docCount.doc_type_id == 5)
                        {
                            finalPaymentCountAvailable += docCount.doc_count;
                        }
                    }

                    if((idProofCountRequired <= idProofCountAvailable)
                       && (advancePaymentCountRequired <= advancePaymentCountAvailable)
                        &&(finalPaymentCountRequired <= finalPaymentCountAvailable))
                    {
                        return true;
                    }

                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }
    }
}


