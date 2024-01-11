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
using System.Transactions;
using System.Globalization;

namespace VTravel.Admin.Controllers
{
    [Route("api/reports"), Authorize(Roles = "ADMIN,SUB_ADMIN,OPERATIONS,MARKETING")]
    public class ReportsController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ReportsController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }

        [HttpGet, Route("availability")]
        public IActionResult GetAvailability(int propid, int room, DateTime fromdate)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT
                            GROUP_CONCAT(DISTINCT CONCAT(
                            'SUM(CASE WHEN inv_date = ""', inv_date, '"" THEN total_qty - booked_qty ELSE 0 END)AS ', DATE_FORMAT(inv_date, ""%M_%d_%Y"")
                            )
                            )
                            INTO @sql
                            FROM inventory where inv_date between '{0}' and '{1}';

                            SET @sql = CONCAT('SELECT p.title property, r.title room, ', @sql, 
                            ' FROM inventory i
                            inner join property p on p.id = i.property_id
                            inner join room r on r.id = i.room_id 
                            where (i.property_id = {2} or {2} = 0) and (i.room_id = {3} or {3} = 0)
                            GROUP BY p.title, r.title');
  
                            PREPARE stmt FROM @sql;
                            EXECUTE stmt;
                            DEALLOCATE PREPARE stmt;", Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd"), Convert.ToDateTime(fromdate).AddDays(6).ToString("yyyy-MM-dd"), propid, room
                );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                var html = "";

                html = html + "<thead>" +
                    "<tr>" +
                    "<th scope=\"col\">Property</th>" +
                    "<th scope=\"col\">Room</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[2].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[3].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[4].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[5].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[6].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[7].ColumnName + "</th>" +
                    "<th scope=\"col\">" + ds.Tables[0].Columns[8].ColumnName + "</th>" +
                    "</tr>" +
                    "</thead>";


                foreach (DataRow r in ds.Tables[0].Rows)
                {


                    html = html + "<tbody>" +
                        "<tr>" +
                        "<td scope=\"row\">" + r[0].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[1].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[2].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[3].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[4].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[5].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[6].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[7].ToString() + "</td  >" +
                        "<td scope=\"row\">" + r[8].ToString() + "</td  >" +
                        "</tr>" +
                        "</tbody>";

                }


                response.Data = html;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("settlement")]
        public IActionResult GetSettlement(int propid, string fromdate, string todate)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                List<Settlement> settlementList = new List<Settlement>();
                MySqlHelper sqlHelper = new MySqlHelper();



                sqlHelper.AddSetParameterToMySqlCommand("propertyId", MySqlDbType.Int32, Convert.ToInt32(propid));
                sqlHelper.AddSetParameterToMySqlCommand("dtfrom", MySqlDbType.VarChar, fromdate);
                sqlHelper.AddSetParameterToMySqlCommand("dtto", MySqlDbType.VarChar, todate);

                DataSet ds = sqlHelper.GetDatasetByCommand("settlement_report");


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    settlementList.Add(
                        new Settlement
                        {
                            res_id = r["reservation_id"].ToString(),
                            created_date = r["created_on"].ToString(),
                            Property = r["property"].ToString(),
                            channelName = r["channel_name"].ToString(),
                            destination = r["destination"].ToString(),
                            custName = r["cust_name"].ToString(),
                            Nationality = r["nationality"].ToString(),
                            custMail = r["cust_email"].ToString(),
                            custPhone = r["cust_phone"].ToString(),
                            resStatus = r["res_status"].ToString(),
                            agreedRent = Convert.ToDecimal(r["agreed_rent"]),
                            bookingAmount = Convert.ToDecimal(r["booking_amount"]),
                            commission = Convert.ToDecimal(r["commission"]),
                            isGst = r["is_gst"].ToString(),
                            gst = Convert.ToDecimal(r["gst"]),
                            noOfGuests = Convert.ToInt32(r["no_of_guests"]),
                            noOfUnits = Convert.ToInt32(r["no_of_units"]),
                            rentAfterOTA = Convert.ToDecimal(r["rent_after_ota"]),
                            higherorEligible = Convert.ToDecimal(r["higher_eligible"]),
                            shortage = Convert.ToDecimal(r["shortage"]),
                            hostShare = Convert.ToDecimal(r["host_share"]),
                            voyeCommission = Convert.ToDecimal(r["voye_comm"]),
                            discount = Convert.ToDecimal(r["discount"]),
                            voyeShare = Convert.ToDecimal(r["voye_share"])
                        }
                        );

                }


                response.Data = settlementList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = ex.Message;
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("reservationdata")]
        public IActionResult GetReservationDetails(int resid)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ReservationData> resList = new List<ReservationData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select rr.reservation_id, rr.from_date, rr.to_date, r.title, years06, years612, years12, rr.amount, case when IFNULL(p.is_gst, 0) = 0 then 'No' else 'Yes' end is_gst, ts.tax
                                            , ROUND(rr.amount * ts.tax / (100 + ts.tax),2) gst
                                            from reserve_rooms rr
                                            left join reservation rs on rs.id = rr.reservation_id
                                            left join property p on p.id = rs.property_id
                                            left join room r on r.id = rr.room_id
                                            left join tax_slab ts on ts.from <= (rr.amount / datediff(rr.to_date, rr.from_date)) and ts.to >= (rr.amount / datediff(rr.to_date, rr.from_date)) and ts.is_active = 'Y'
                                            where rr.reservation_id = {0}", resid);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    resList.Add(
                        new ReservationData
                        {
                            room = r["title"].ToString(),
                            checkin = DateTime.Parse(r["from_date"].ToString()),
                            checkout = DateTime.Parse(r["to_date"].ToString()),
                            years06 = Convert.ToInt32(r["years06"]),
                            years612 = Convert.ToInt32(r["years612"]),
                            years12 = Convert.ToInt32(r["years12"]),
                            amount = Convert.ToDecimal(r["amount"]),
                            isgst = r["is_gst"].ToString(),
                            gstperc = Convert.ToDecimal(r["tax"]),
                            gstamount = Convert.ToDecimal(r["gst"])
                        }
                        );

                }

                response.Data = resList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("sheet")]
        public IActionResult GetSheet(int propid, string fromdate, string todate)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                List<Settlement> settlementList = new List<Settlement>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var html = "";

                html = gethtml(propid, fromdate, todate);
                var hostshare = html.Split("&&")[1];
                html = html.Split("&&")[0];
                if (html != "")
                {
                    IEnumerable<Claim> claims = User.Claims;
                    var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;


                    var hist_query = string.Format(@"select id, is_approved from settlement_history where from_date = '{1}' and property_id = {0}", propid, fromdate);

                    DataSet dshistory = sqlHelper.GetDatasetByMySql(hist_query);

                    var query1 = "";
                    if (dshistory.Tables[0].Rows.Count > 0)
                    {
                        if (dshistory.Tables[0].Rows[0]["is_approved"].ToString() != "Y")
                        {
                            query1 = string.Format(@"update settlement_history set html_content = '{2}', updated_by = {3}, updated_on = '{4}', is_approved = 'N', host_share = {5} where property_id = {0} and from_date = '{1}';"
                                                    , propid, fromdate, html, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDecimal(hostshare));

                            DataSet ds = sqlHelper.GetDatasetByMySql(query1);
                        }
                    }
                    else
                    {
                        query1 = string.Format(@"insert into settlement_history(property_id,from_date,to_date,html_content, is_approved, created_by, created_on, host_share) 
                                            VALUES({0},'{1}','{2}','{3}','N',{4},'{5}', {6});", propid, fromdate, todate, html, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDecimal(hostshare));

                        DataSet ds = sqlHelper.GetDatasetByMySql(query1);
                    }
                }
                response.Data = html;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = ex.Message;
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("getmonths")]
        public IActionResult Getmonths()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<MonthList> monthList = new List<MonthList>();


                for(int i = 0; i >= -12; i--)
                {
                    DateTime d = DateTime.Now.AddMonths(i);
                    monthList.Add(new MonthList { 
                        id = d.Date.ToString("MM") + " " + d.Date.ToString("yyyy"),
                        monthname = d.Date.ToString("MMM") + " " + d.Date.ToString("yyyy"),
                    });
                }


                response.Data = monthList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPut, Route("update-sheet-history")]
        public IActionResult UpdateHistory(HistoryData model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                List<Settlement> settlementList = new List<Settlement>();
                MySqlHelper sqlHelper = new MySqlHelper();
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                var html = gethtml(Convert.ToInt32(model.propertyid), model.fromDate, model.toDate);


                var query = string.Format(@"select id from settlement_history where property_id = {0} and from_date = '{1}';", model.propertyid, model.fromDate);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0]["is_approved"].ToString() != "Y")
                    {
                        query = string.Format(@"update settlement_history set html_content = {2}, updated_by = {3}, updated_on = '{4}', is_approve = 'N' where property_id = {0} and from_date = '{1}';"
                                                , model.propertyid, model.fromDate, html, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                else
                {
                    query = string.Format(@"insert into settlement_history(property_id,from_date,to_date,html_content, is_approved, created_by, created_on) 
                                            VALUES({0},'{1}','{2}','{3}','N',{4},'{5}');", model.propertyid, model.fromDate, model.toDate, html, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                ds = sqlHelper.GetDatasetByMySql(query);

                //response.Data = html;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = ex.Message;
            }
            return new OkObjectResult(response);


        }

        public string gethtml(int propid, string fromdate, string todate)
        {
            var html = "";


            List<Settlement> settlementList = new List<Settlement>();
            MySqlHelper sqlHelper = new MySqlHelper();


            var query = string.Format(@"select p.id,p.title,p.address,p.city,p.state,p.country,p.property_type_id,p.email,p.phone,d.title destination
                 ,p.room_count FROM property p 
                 left join destination d on d.id = p.destination_id 
                 WHERE p.id={0} ORDER BY p.sort_order;
                 select name, charge_type, amount, percentage from app_charges ac
                 where property_id = {0} and effective_from between '{1}' and '{2}'; 
                 select channel_name from booking_channel where is_active = 'Y';               
                 select 'TDS' name, 'percentage' charge_type, 0 amount, t.percentage
                 from property p
                 left join owner_master o on o.id = p.owner
                 left join tds_details t on t.ownership_type = o.ownership_type
                 where p.id = {0} and t.effective_from between '{1}' and '{2}';", propid, fromdate, todate);

            DataSet dsproperty = sqlHelper.GetDatasetByMySql(query);


            sqlHelper.AddSetParameterToMySqlCommand("propertyId", MySqlDbType.Int32, Convert.ToInt32(propid));
            sqlHelper.AddSetParameterToMySqlCommand("dtfrom", MySqlDbType.VarChar, fromdate);
            sqlHelper.AddSetParameterToMySqlCommand("dtto", MySqlDbType.VarChar, todate);

            DataSet ds = sqlHelper.GetDatasetByCommand("settlement_report");
            if (ds.Tables[0].Rows.Count > 0)
            {

                decimal hostshare = Convert.ToDecimal(ds.Tables[0].Compute("Sum(host_share)", ""));
                decimal balance = Convert.ToDecimal(ds.Tables[0].Compute("Sum(balancepayment)", ""));
                decimal amount = 0;
                decimal tds = 0;

                for (int i = 0; i < dsproperty.Tables[1].Rows.Count; i++)
                {

                    if (dsproperty.Tables[1].Rows[i]["charge_type"].ToString() == "Amount")
                    {
                        amount = amount + Convert.ToDecimal(dsproperty.Tables[1].Rows[i]["amount"]);
                    }
                    else
                    {
                        amount = amount + (hostshare * Convert.ToDecimal(dsproperty.Tables[1].Rows[i]["percentage"]) / 100);
                    }
                }

                html = html + "<html>   <head>      <title>Settlement Sheet - " + ds.Tables[0].Rows[0]["property"].ToString() + " - " + Convert.ToDateTime(fromdate).ToString("MMM yy") + "</title>      <style type=\"text/css\"> * {margin:0; padding:0; text-indent:0; } body {  font-family: Calibri, sans-serif;   font-style: normal; font-weight: normal;    text-decoration: none;  font-size: 11pt;    margin: 0pt;    }   @page {      size: A4;      margin: 1cm;    }   .first tr td,th {   border: 1pt solid black;    }   .first tr td p {    padding-top: 2pt;   padding-left: 5pt;  text-indent: 0pt;   line-height: 11pt;  text-align: left;   }   .first tr td .amount {  padding-top: 2pt;   padding-right: 4pt; text-indent: 0pt;   line-height: 11pt;  text-align: right;   }   body h1 {   padding-top: 2pt;   padding-left: 40pt; text-indent: 0pt;   text-align: center; font-size: 14pt;    }   body .lines {   padding-top: 2pt;   padding-left: 40pt; text-indent: 0pt;   line-height: 111%;  text-align: left;  }   body .content { padding-left: 40pt; text-indent: 0pt;   line-height: 114%;  text-align: justify;    }   .header {height:100px;  padding: 50px;  width: 750px;  text-align: right;  color: white;  font-size: 30px;position: fixed;top: 0;}.footer {height:75px;  padding: 40px;  width: 780px;  text-align: center;  background-color: #203864;  color: white;  font-size: 15px;  position: fixed;  bottom: 0;} body  { counter-reset: page; } #pageFooter { page-break-before: always; padding-right: 40px; }   #pageFooter:after   {   display: block; text-align: right;  counter-increment: page;    content: \"Page \" counter(page);   }   #pageFooter.first.page  {   page-break-before: avoid;   }  </style>  </head>   " +
                    "<body style=\"width:750px\">       " +
                    "<div class=\"header\">" +
                    "<img src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAJAAAAA9CAYAAABCxOCIAAAACXBIWXMAAAsTAAALEwEAmpwYAAAGgWlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDYwLCAyMDIwLzA1LzEyLTE2OjA0OjE3ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjEuMiAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIxLTEwLTA4VDE4OjM2OjU1KzA0OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMS0xMC0xMlQyMzo0Nzo1MSswNDowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMS0xMC0xMlQyMzo0Nzo1MSswNDowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDoyYTYxMDBkMy04MjhiLTcwNGItYTQ4Zi05MDQxYWExNjc3Y2IiIHhtcE1NOkRvY3VtZW50SUQ9ImFkb2JlOmRvY2lkOnBob3Rvc2hvcDo2YTRlYTE2Yy00N2ZhLTljNGYtODc4ZS0xZDE5NWVkOWFlMGQiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDo5YTVlOWE0OS0xMzE2LWEzNGItOTQ2YS0wMTk3NmI4MzhjY2YiPiA8cGhvdG9zaG9wOkRvY3VtZW50QW5jZXN0b3JzPiA8cmRmOkJhZz4gPHJkZjpsaT54bXAuZGlkOjVmN2Q5Nzk4LTFmMjQtNGMyNS04MWIwLTNiNGNlNzgzNjAxNTwvcmRmOmxpPiA8L3JkZjpCYWc+IDwvcGhvdG9zaG9wOkRvY3VtZW50QW5jZXN0b3JzPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOjlhNWU5YTQ5LTEzMTYtYTM0Yi05NDZhLTAxOTc2YjgzOGNjZiIgc3RFdnQ6d2hlbj0iMjAyMS0xMC0wOFQxODozNjo1NSswNDowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIxLjIgKFdpbmRvd3MpIi8+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJzYXZlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDoyYTYxMDBkMy04MjhiLTcwNGItYTQ4Zi05MDQxYWExNjc3Y2IiIHN0RXZ0OndoZW49IjIwMjEtMTAtMTJUMjM6NDc6NTErMDQ6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMS4yIChXaW5kb3dzKSIgc3RFdnQ6Y2hhbmdlZD0iLyIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz5xpyFVAAAShElEQVR4nO2dfZhfRXXHP/PbzQuJhI0QXqIEgUAEEzCAhljFQIwILQTFKG+KIPqE14aXaGktgo1oCgKpCCjWEqym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\" />" +
                    "</div>       " +
                    "<div class=\"footer\">  " +
                    "<p>VOYE HOMES C/O voye.in Travel Automations India Pvt Ltd</p> " +
                    "<p>Registered Office:First Floor, Neospace II, Kinfra Techno Industrial Park Kakkanchery, Malappuram - 673634</p>" +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>" +
                    "<p>www.voyehomes.com |voyehomes@gmail.com |hr@voye.in | 9539690660    </p> " +
                    //"<div id=\"pageFooter\"`>  " +
                    ////"&p; of &P; Pages"+
                    //"</div>" +
                    "</div>" +
                    "<div style=\"width:100%; height:125px;margin-left:40pt;\"></div>" +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<h1>Reservation Chart &amp; Settlements – " + Convert.ToDateTime(fromdate).ToString("MMMM yy") + "</h1>       " +
                    "<p class=\"break\" style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p class=\"lines\">Service Partner Name <b>: " + ds.Tables[0].Rows[0]["property"].ToString() + " </b></p>       " +
                    "<p class=\"lines\">Address <b>: " + dsproperty.Tables[0].Rows[0]["address"].ToString() + " </b></p>       " +
                    "<p class=\"lines\">Partner Code<b>: VOYE Homes </b></p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p class=\"lines\">Dear Host,</p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p class=\"lines\">Greetings from VOYE HOMES!</p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p class=\"content\">Thank you for making us as your business partner. We would appreciate that you respond to our request soon and work constantly to provide great travel experiences to our guests. Your trust in us is greatly appreciated.</p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p class=\"content\">Please find the below Reservations chart and Settlement Report for " + Convert.ToDateTime(fromdate).ToString("MMMM yy") + ".</p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<h3 class=\"lines\">Yours sincerely,</h3>       " +
                    "<h3 class=\"lines\">Finance & Accounts</h3>       " +
                    "<h3 class=\"lines\">accounts@voye.in</h3>       " +
                    "<h3 class=\"lines\">+91-79944 40934</h3>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>       " +
                    "<p class=\"content\">Please ensure that payment is confirmed by the 10th of each month. Failure to do so will result in the transfer of the settlement amount specified in this sheet on the following working day.</p>       " +
                    "<p style=\"text-indent: 0pt;text-align: left;page-break-after: always;\"><br /></p>       " +
                    "<div style=\"width:100%; height:125px;margin-left:40pt;\"></div>" +
                    "<table class=\"first\" style=\"border-collapse:collapse; margin-left: 20pt; margin-right: 20pt; width:100%;\" cellspacing=\"0\">           " +
                    "<tr>               " +
                    "<th colspan=\"2\" bgcolor=\"#001F5F\">                   " +
                    "<p style=\"text-align: center; color:white;\">Payment Summary - " + Convert.ToDateTime(fromdate).ToString("MMMM yy") + "</p>               " +
                    "</th>           " +
                    "</tr>           " +
                    "<tr>               " +
                    "<td colspan=\"2\" bgcolor=\"#F1F1F1\">                   " +
                    "<p><b>VOYE HOMES C/0 VOYE.IN TRAVEL AUTOMATIONS INDIA PVT LTD</b></p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p><b>Corporate Office: </b></p>  <br />             " +
                    "<p>Room I – 4, Neospace II</p>  <br />             " +
                    "<p>KINFRA Techno Industrial Park </p>  <br />             " +
                    "<p>Calicut, Kerala - 673634</p>  <br />             " +
                    "</td>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>Payment Cycle: " + Convert.ToDateTime(fromdate).ToString("dd/MM/yy") + " - " + Convert.ToDateTime(todate).ToString("dd/MM/yy") + "</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>Property Name</p>               " +
                    "</td>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>" + dsproperty.Tables[0].Rows[0]["title"].ToString() + "</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>Address</p>               " +
                    "</td>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>" + dsproperty.Tables[0].Rows[0]["address"].ToString() + "</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:15pt\">               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>Cluster</p>               " +
                    "</td>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>" + dsproperty.Tables[0].Rows[0]["destination"].ToString() + "</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:15pt\">               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>Number of Rooms</p>               " +
                    "</td>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>" + dsproperty.Tables[0].Rows[0]["room_count"].ToString() + "</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:15pt\">               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>Personal Bookings/ Room</p>               " +
                    "</td>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>" + ds.Tables[0].Select($"channel_name = 'Property Owner'").Length + " (Not included in our calculations)</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:15pt\">               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p>Total Occupancy Rate</p>               " +
                    "</td>               " +
                    "<td bgcolor=\"#F1F1F1\">                   " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:31pt\">               " +
                    "<td colspan=\"2\">                   " +
                    "<p>Accounts Receivable</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:15pt\">               " +
                    "<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                    "<p style=\" color: white;\">Total Booking Amount</p>               " +
                    "</td>               " +
                    "<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                    "<p class=\"amount\" style=\" color: white;\">₹ " + hostshare.ToString("0.##") + "</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:26pt\">               " +
                    "<td colspan=\"2\">                   " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>                   " +
                    "<p style=\"padding-left: 5pt;text-indent: 0pt;line-height: 11pt;text-align: left;\">Accounts Payable</p>               " +
                    "</td>           " +
                    "</tr>           " +
                    "<tr style=\"height:25pt\">               " +
                    "<td style=\"border:1pt solid black;\">                   " +
                    "<p style=\"padding-right: 4pt;text-indent: 0pt;text-align: right;\">Amount Collected at Property</p>               " +
                    "</td>               " +
                    "<td style=\"border:1pt solid black;\">                   " +
                    "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>                   " +
                    "<p class=\"amount\">₹ " + balance.ToString("0.##") + "</p>               " +
                    "</td>           " +
                    "</tr>           ";

                for (int i = 0; i < dsproperty.Tables[1].Rows.Count; i++)
                {

                    if (dsproperty.Tables[1].Rows[i]["charge_type"].ToString() == "Amount")
                    {
                        html = html + "<tr style=\"height:15pt\">               " +
                                            "<td style=\"border:1pt solid black;\">                   " +
                                            "<p class=\"s4\" style=\"padding-top: 1pt;padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">" + dsproperty.Tables[1].Rows[i]["name"].ToString() + "</p>               " +
                                            "</td>               " +
                                            "<td style=\"border:1pt solid black;\">                   " +
                                            "<p class=\"amount\">₹ " + Convert.ToDecimal(dsproperty.Tables[1].Rows[i]["amount"]).ToString("0.##") + "</p>               " +
                                            "</td>           " +
                                            "</tr>           ";
                    }
                    else
                    {
                        html = html + "<tr style=\"height:15pt\">               " +
                        "<td style=\"border:1pt solid black;\">                   " +
                        "<p class=\"s4\" style=\"padding-top: 1pt;padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">" + dsproperty.Tables[1].Rows[i]["name"].ToString() + "@" + dsproperty.Tables[1].Rows[i]["percentage"].ToString() + "%</p>               " +
                        "</td>               " +
                        "<td style=\"border:1pt solid black;\">                   " +
                        "<p class=\"amount\">₹ " + (hostshare * Convert.ToDecimal(dsproperty.Tables[1].Rows[i]["percentage"]) / 100).ToString("0.##") + "</p>               " +
                        "</td>           " +
                        "</tr>           ";
                    }
                }

                html = html + "<tr style=\"height:15pt\">               " +
                "<td style=\"border:1pt solid black;\">                   " +
                "<p style=\"padding-top: 2pt;padding-right: 4pt;text-indent: 0pt;line-height: 11pt;text-align: right;\">Sub Total</p>               " +
                "</td>               " +
                "<td style=\"border:1pt solid black;\">                   " +
                "<p class=\"amount\">₹ " + (balance + amount).ToString("0.##") + "</p>               " +
                "</td>           " +
                "</tr>           ";

                if (dsproperty.Tables[3].Rows.Count > 0)
                {
                    tds = (hostshare * Convert.ToDecimal(dsproperty.Tables[3].Rows[0]["percentage"]) / 100);
                    html = html + "<tr style=\"height:15pt\">               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p class=\"s4\" style=\"padding-top: 1pt;padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">" + dsproperty.Tables[3].Rows[0]["name"].ToString() + "@" + dsproperty.Tables[3].Rows[0]["percentage"].ToString() + "%</p>               " +
                            "</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p class=\"amount\">₹ " + tds.ToString("0.##") + "</p>               " +
                            "</td>           " +
                            "</tr>           ";
                }
                html = html + "<tr style=\"height:16pt\">               " +
                "<td style=\"border:1pt solid black;\">                   " +
                "<p style=\"padding-top: 2pt;padding-left: 5pt;text-indent: 0pt;line-height: 11pt;text-align: left;\">Total Payable</p>               " +
                "</td>               <td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                "<p class=\"amount\" style=\" color: white;\">₹ " + (hostshare - balance - amount - tds).ToString("0.##") + "</p>               " +
                "</td>           " +
                "</tr>       " +
                "</table>       " +
                "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>                   " +
                "<h3 style=\"text-align: center;\">In case of any queries regarding reservations chart, please call: +91-79944 40934</h3>           " +
                "<p style=\"text-indent: 0pt;text-align: left;page-break-after: always;\"><br /></p>       " +
                "<div style=\"width:100%; height:125px;margin-left:40pt;\"></div>" +
                "<u><h1 style=\"font-size:20px;\">Channelwise Breakup</h1></u>       ";

                for (int i = 0; i < dsproperty.Tables[2].Rows.Count; i++)
                {
                    if (ds.Tables[0].Select("channel_name='" + dsproperty.Tables[2].Rows[i]["channel_name"].ToString() + "'").Count() > 0)
                    {
                        html = html + "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p> " +
                            "<span style=\"margin-left: 20pt; font-size:18px;\"><b>" + dsproperty.Tables[2].Rows[i]["channel_name"].ToString() + "</b></span> <br/>" +
                            "<table class=\"first\" style=\"border-collapse: collapse; margin-left: 20pt; margin-right: 20pt; width: 100%;\" cellspacing=\"0\">           " +
                        "<tr style=\"height:31pt;\">               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">Sl No</p>               " +
                        "</th>               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">Guest Name</p>               " +
                        "</th>               " +
                        //"<th bgcolor=\"#001F5F\">                   " +
                        //"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>                   " +
                        //"<p style=\" color: white;\">Channel</p>               " +
                        //"</th>               " +
                        //"<th bgcolor=\"#001F5F\">                   " +
                        //"<p style=\" color: white;\">Check in</p>               " +
                        //"</th>               " +
                        //"<th bgcolor=\"#001F5F\">                   " +
                        //"<p style=\" color: white;\">Check out</p>               " +
                        //"</th>               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">No. of guests</p>               " +
                        "</th>               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">Used Nights</p>               " +
                        "</th>               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">Total Units</p>               " +
                        "</th>               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">GST Amount</p>               " +
                        "</th>               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">Booking Amount</p>               " +
                        "</th>               " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">Payment to Host</p>               " +
                        "</th>           " +
                        "<th bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white;\">Payment @ Property</p>               " +
                        "</th>           " +
                        "</tr>           ";

                        DataView dv = new DataView(ds.Tables[0]);
                        dv.RowFilter = "channel_name='" + dsproperty.Tables[2].Rows[i]["channel_name"].ToString() + "'";
                        decimal ba = 0;
                        decimal hs = 0;
                        decimal gst = 0;
                        decimal bp = 0;

                        for (int j = 0; j < dv.ToTable().Rows.Count; j++)
                        {
                            html = html + "<tr style=\"height:27pt\">               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-left: 5pt;text-indent: 0pt;line-height: 9pt;text-align: left;\">" + (j + 1).ToString() + "</p>               " +
                            "</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-left: 5pt;text-indent: 0pt;line-height: 12pt;display: table-cell;vertical-align: middle;text-align: left;\">" + dv.ToTable().Rows[j]["cust_name"].ToString() + "</p>               " +
                            "</td>               " +
                            //"<td style=\"border:1pt solid black;\">                   " +
                            //"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>                   " +
                            //"<p style=\"padding-left: 5pt;text-indent: 0pt;line-height: 12pt;text-align: left;\">" + dv.ToTable().Rows[j]["channel_name"].ToString() + "</p>               " +
                            //"</td>               " +
                            //"<td style=\"border:1pt solid black;\">                   " +
                            //"<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 13pt;text-align: right;\">01-09-23</p>               " +
                            //"</td>               " +
                            //"<td style=\"border:1pt solid black;\">                   " +
                            //"<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 13pt;text-align: right;\">02-09-23</p>               " +
                            //"</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">" + dv.ToTable().Rows[j]["no_of_guests"].ToString() + "</p>               " +
                            "</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">" + dv.ToTable().Rows[j]["no_of_nights"].ToString() + "</p>               " +
                            "</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">" + dv.ToTable().Rows[j]["no_of_units"].ToString() + "</p>               " +
                            "</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">₹" + Convert.ToDecimal(dv.ToTable().Rows[j]["gst"]).ToString("F2") + "</p>               " +
                            "</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">₹" + Convert.ToDecimal(dv.ToTable().Rows[j]["booking_amount"]).ToString("F2") + "</p>               " +
                            "</td>               " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">₹" + Convert.ToDecimal(dv.ToTable().Rows[j]["host_share"]).ToString("F2") + "</p>               " +
                            "</td>           " +
                            "<td style=\"border:1pt solid black;\">                   " +
                            "<p style=\"padding-right: 4pt;text-indent: 0pt;line-height: 12pt;text-align: right;\">₹" + Convert.ToDecimal(dv.ToTable().Rows[j]["balancepayment"]).ToString("F2") + "</p>               " +
                            "</td>           " +
                            "</tr>           ";

                            ba = ba + Convert.ToDecimal(dv.ToTable().Rows[j]["booking_amount"]);
                            hs = hs + Convert.ToDecimal(dv.ToTable().Rows[j]["host_share"]);
                            gst = gst + Convert.ToDecimal(dv.ToTable().Rows[j]["gst"]);
                            bp = bp + Convert.ToDecimal(dv.ToTable().Rows[j]["balancepayment"]);
                        }
                        html = html + "<tr style=\"height:16pt\">               " +
                        "<td style=\"border:1pt solid black;\" colspan=\"4\" bgcolor=\"#001F5F\">                   " +
                        "<p style=\" color: white; text-align:center\">Total</p>               " +
                        "</td>               " +
                        //"<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                        //"<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>               " +
                        //"</td>               " +
                        "<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                        "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>               " +
                        "</td>               " +
                        "<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                        "<p class=\"amount\" style=\" color: white;\">₹" + gst.ToString("F2") + "</p>               " +
                        "</td>               " +
                        "<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                        "<p class=\"amount\" style=\" color: white;\">₹" + ba.ToString("F2") + "</p>               " +
                        "</td>               " +
                        "<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                        "<p class=\"amount\" style=\" color: white;\">₹" + hs.ToString("F2") + "</p>               " +
                        "</td>           " +
                        "<td style=\"border:1pt solid black;\" bgcolor=\"#001F5F\">                   " +
                        "<p class=\"amount\" style=\" color: white;\">₹" + bp.ToString("F2") + "</p>               " +
                        "</td>           " +
                        "</tr>       " +
                        "</table>       ";
                    }
                }

                html = html + "<p style=\"text-indent: 0pt;text-align: left;\"><br /></p>                   " +
                        "<h4 style=\"text-align: center;\">***End of the report***</h3>           " + "&&" + hostshare.ToString();
            }
            return html;
        }

        [HttpGet, Route("get-sheet-history")]
        public IActionResult GetHistory(int propid)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                List<SettlementHistory> historylist = new List<SettlementHistory>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select h.id, h.property_id, p.title prpoperty, h.from_date, h.to_date, h.html_content, h.is_approved, h.host_Share, IFNULL(h.updated_on, h.created_on) updated_on, IFNULL(h.approved_on, '') approved_on 
                                            from settlement_history h
                                            left join property p on h.property_id = p.id
                                            where property_id = {0}
                                            order by id desc;", propid);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    historylist.Add(
                        new SettlementHistory
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            propertyid = r["property_id"] == DBNull.Value ? "0" : r["property_id"].ToString(),
                            property = r["prpoperty"] == DBNull.Value ? "" : r["prpoperty"].ToString(),
                            fromDate = Convert.ToDateTime(r["from_date"].ToString()).ToString("yyyy-MM-dd"),
                            toDate = Convert.ToDateTime(r["to_date"].ToString()).ToString("yyyy-MM-dd"),
                            html = r["html_content"].ToString(),
                            is_approved = r["is_approved"].ToString(),
                            host_share = Convert.ToDecimal(r["host_Share"]),
                            updated_on = r["updated_on"].ToString() == "" ? "" : Convert.ToDateTime(r["updated_on"].ToString()).ToString("dd-MM-yyyy"),
                            approved_on = r["approved_on"].ToString() == "" ? "" : Convert.ToDateTime(r["approved_on"].ToString()).ToString("dd-MM-yyyy")
                        });

                }

                response.Data = historylist;
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
        [HttpPut, Route("approve-history")]
        public IActionResult ApproveHistory(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"update settlement_history set is_approved = 'Y', approved_on = '{1}'
                                        where id = {0}", id, DateTime.Now.ToString("yyyy-MM-dd"));

                DataSet ds = sqlHelper.GetDatasetByMySql(query);
                response.ActionStatus = "SUCCESS";




            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-sheet-status")]
        public IActionResult SheetStatus(int id, string fromdate)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id, is_approved, html_content from settlement_history where property_id = {0} and from_date = '{1}'", id, fromdate);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    response.Data = ds.Tables[0].Rows[0]["id"].ToString() + "&&" + ds.Tables[0].Rows[0]["is_approved"].ToString() + "&&" + ds.Tables[0].Rows[0]["html_content"].ToString();
                }
                else
                {
                    response.Data = "0&&N&&";
                }
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
