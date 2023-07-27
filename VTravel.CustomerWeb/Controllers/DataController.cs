using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using StackExchange.Redis;
using VTravel.CustomerWeb.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VTravel.CustomerWeb.Controllers
{
    [Route("api/data")]   
    public class DataController : Controller
    {
        bool useRedis = false;
        private IDatabase _db;
        ConnectionMultiplexer connectionMultiplexer;
        ConfigurationOptions redisConfiguration;

        string redisConnectionString;

        private readonly IConfiguration _configuration;
        private string DefaultConnectionString;
        public DataController(IConfiguration configuration)
        {

            _configuration = configuration;

            DefaultConnectionString = _configuration.GetConnectionString("DefaultConnectionString");

            useRedis = bool.Parse(configuration["UseRedis"]);
            if (useRedis)
            {

                redisConnectionString = configuration.GetConnectionString("RedisConnectionString");

                redisConfiguration = ConfigurationOptions.Parse(redisConnectionString);
                redisConfiguration.CommandMap = CommandMap.Create(new HashSet<string>
                { // EXCLUDE a few commands
                    "INFO", "CONFIG", "CLUSTER",
                    "PING", "ECHO", "CLIENT"
                }, available: false);


            }
        }

        [HttpGet, Route("get-home-data")]
        public async Task<IActionResult> GetHomeData()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                using (var connection = new MySqlConnection(DefaultConnectionString))
                {
                    var query = string.Format(@"SELECT id, title,description,thumbnail  
                                           FROM destination WHERE  is_active='Y' ORDER BY sort_order");

                    var destinationList = await connection.QueryAsync<dynamic>(query);
                    var homeTagList = new List<Tag>();

                    if (useRedis)
                    {
                        //using (connectionMultiplexer = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("RedisConnectionString")))
                        try
                        {
                            connectionMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);
                            if (connectionMultiplexer != null)
                            {

                                _db = connectionMultiplexer.GetDatabase();
                                if (_db != null)
                                {
                                    RedisValue redisValue = await _db.StringGetAsync("HomeTagList");
                                    if (!redisValue.IsNullOrEmpty)
                                    {
                                        // The data was found in Redis, deserialize it to a list of Tag objects
                                        var json = redisValue.ToString();
                                        var tagList = JsonConvert.DeserializeObject<List<string>>(json);
                                        foreach (var tag in tagList)
                                        {
                                            homeTagList.Add(JsonConvert.DeserializeObject<Tag>(tag));
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            General.LogException(ex);

                        }


                    }



                    if (homeTagList.Count == 0)
                    {
                        // If the key does not exist in Redis, fetch the data from the database
                        query = string.Format(General.HOME_TAG_LIST_QUERY);

                        MySqlHelper sqlHelper = new MySqlHelper();
                        var ds = sqlHelper.GetDatasetByMySql(query);

                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            homeTagList.Add(JsonConvert.DeserializeObject<Tag>(r[0].ToString()));

                        }
                    }

                    response.Data = new
                    {
                        destinationList,
                        tagList= homeTagList

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

        [HttpGet, Route("get-country-list")]
        public IActionResult GetList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Country> countries = new List<Country>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,iso,name,nicename,phonecode  
                 FROM country_dump"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    countries.Add(
                        new Country
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            phonecode = Convert.ToInt32(r["phonecode"].ToString()),
                            name = r["name"].ToString(),
                            iso = r["iso"].ToString(),
                            nicename = r["nicename"].ToString(),
                        }
                        );

                }


                response.Data = countries;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-price-list")]//
        public IActionResult GetPriceList(int propertyId,int roomId, DateTime fromDate, DateTime toDate)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PriceData> prices = new List<PriceData>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT id,inv_date, price FROM inventory WHERE  room_id={0} AND property_id={1} AND inv_date >= '{2}' AND inv_date < '{3}'"
                                   , roomId, propertyId, fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    prices.Add(
                        new PriceData
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            invDate = DateTime.Parse(r["inv_date"].ToString()) ,
                            price = double.Parse(r["price"].ToString())
                        }
                        ); ;

                }


                response.Data = prices;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost, Route("register-customer")]
        public IActionResult RegisterCustomer(Customer model)
        {
            if (ModelState.IsValid)
            {
                ApiResponse response = new ApiResponse();
                response.ActionStatus = "FAILURE";
                response.Message = string.Empty;

                if (model.referralCode == null)
                {
                    model.referralCode = string.Empty;
                }
                try
                {
                    MySqlHelper sqlHelper = new MySqlHelper();

                    //create OTP
                    var otp = General.GenerateOTP();

                    var otpCreatedDate = DateTime.Now;
                    var otpExpiryDate = otpCreatedDate.AddMinutes(5);
                    var custId = 0;
                    //save customer and OTP to db
                    //check if customer exists
                    var query = string.Format(@"SELECT id,cust_name,cust_email FROM customer WHERE cust_phone='{0}' AND cust_email='{1}'"
                                 , model.custPhone,model.custEmail);
                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            custId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                            //update existing customer with OTP
                            query = string.Format(@"UPDATE customer SET cust_name='{0}',otp='{1}',otp_expiry_date='{2}',otp_created_date='{3}' WHERE id={4}"
                                 , model.custName, otp, otpExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),otpCreatedDate.ToString("yyyy-MM-dd HH:mm:ss"), custId);
                            ds = sqlHelper.GetDatasetByMySql(query);
                        }
                        else
                        {
                            //create new customer with OTP
                            query = string.Format(@"INSERT INTO customer(cust_name,cust_email,cust_phone,otp,otp_expiry_date,otp_created_date,referral_code)
                                  VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}');SELECT LAST_INSERT_ID() AS id;"
                                     , model.custName, model.custEmail, model.custPhone, otp, 
                                     otpExpiryDate.ToString("yyyy-MM-dd HH:mm:ss"),otpCreatedDate.ToString("yyyy-MM-dd HH:mm:ss"),model.referralCode.ToUpper());
                            ds = sqlHelper.GetDatasetByMySql(query);
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    custId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                                }
                            }
                        }
                    }
                  

                    //send OTP
                    var smsContent = General.GetSettingsValue("reserve_otp_sms").Replace("#OTP#", otp);

                    if (General.sendSMS(smsContent, model.custPhone))
                    {
                        response.ActionStatus = "SUCCESS";
                        response.Data = new { custId= custId };
                    }
                   
                }
                catch(Exception ex)
                {
                    response.ActionStatus = "EXCEPTION";
                    General.LogException(ex);
                }

                return new OkObjectResult(response);
            }
            else
            {
                return BadRequest();

            }
        }

        [HttpPost, Route("send-enquiry")]
        public IActionResult SendEnquiry(Enquiry model)
        {
            if (ModelState.IsValid)
            {
                ApiResponse response = new ApiResponse();
                response.ActionStatus = "FAILURE";
                response.Message = string.Empty;

                try
                {
                    //validate OTP
                    MySqlHelper sqlHelper = new MySqlHelper();
                    
                    var query = string.Format("SELECT id,cust_email FROM customer WHERE id={0} AND otp='{1}' AND otp_expiry_date>='{2}'",
                        model.custId,model.otp,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                   
                    response.ActionStatus = "OTPERROR";


                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            var custId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                            var custEmail = ds.Tables[0].Rows[0]["cust_email"].ToString();
                            if (custId > 0)
                            {
                                //save enquiry to db

                                query = string.Format(@"INSERT INTO enquiry(cust_id,property_id,checkin_date,checkout_date,adults_count,children_count)
                                  VALUES({0},{1},'{2}','{3}',{4},{5});SELECT LAST_INSERT_ID() AS id;"
                                           , model.custId, model.propertyId,
                                           model.checkInDate.ToString("yyyy-MM-dd HH:mm:ss"), model.checkOutDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                           model.adultsCount, model.childrenCount);
                                ds = sqlHelper.GetDatasetByMySql(query);
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        var enquiryId = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());

                                        response.ActionStatus = "SUCCESS";
                                        //send email to admin

                                        query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='enquiry_email_admin'";
                                             ds = sqlHelper.GetDatasetByMySql(query);
                                        if (ds.Tables.Count > 0)
                                        {
                                            if (ds.Tables[0].Rows.Count > 0)
                                            {
                                                var emailBody = ds.Tables[0].Rows[0]["content"].ToString();

                                                query = string.Format(@"SELECT t1.id,t1.property_id,t1.cust_id,t1.adults_count,t1.children_count,
                                                   t1.checkin_date,t1.checkout_date,t2.cust_name,t2.cust_email,t2.cust_phone,t2.referral_code,
                                                    t3.id AS property_id,t3.thumbnail,t3.title,t3.perma_title,t3.short_description FROM enquiry t1 LEFT JOIN customer t2
                                                    ON t1.cust_id=t2.id LEFT JOIN property t3 ON t1.property_id=t3.id WHERE t1.is_active='Y' AND t1.id={0}", enquiryId);
                                                ds = sqlHelper.GetDatasetByMySql(query);
                                                DataRow r = ds.Tables[0].Rows[0];

                                                emailBody = emailBody.Replace("#ADULTS#", r["adults_count"].ToString())
                                                .Replace("#CHILDREN#", r["children_count"].ToString())
                                                .Replace("#CHECKIN#", DateTime.Parse(r["checkin_date"].ToString()).ToString("yyyy MMM dd"))
                                                .Replace("#CHECKOUT#", DateTime.Parse(r["checkout_date"].ToString()).ToString("yyyy MMM dd"))
                                                .Replace("#NAME#", r["cust_name"].ToString())
                                                .Replace("#EMAIL#", r["cust_email"].ToString())
                                                .Replace("#PHONE#", r["cust_phone"].ToString())
                                                .Replace("#PROPERTY#", r["title"].ToString())
                                                .Replace("#REFERRAL_CODE#", r["referral_code"].ToString());
                                                
                                                

                                                var subject = General.GetSettingsValue("enquiry_email_subject")
                                                    .Replace("#PROPERTY#", r["title"].ToString()).Replace("#ENQUIRYID#", enquiryId.ToString());
                                              
                                                General.SendMailMailgun(subject, emailBody, General.GetSettingsValue("enquiry_email_to"), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("enquiry_from_display_name"));

                                                

                                                var notifySms = General.GetSettingsValue("reserve_notify_sms").Replace("#PROPERTY#", r["title"].ToString());
                                                var notifySmsTo = General.GetSettingsValue("reserve_notify_sms_to");

                                                General.sendSMS(notifySms, notifySmsTo);

                                                //send email to customer
                                                query = @"SELECT content FROM email_template WHERE is_active='Y' AND template_name='enquiry_email_customer'";
                                                ds = sqlHelper.GetDatasetByMySql(query);
                                                var emailBodyCustomer = ds.Tables[0].Rows[0]["content"].ToString();

                                                var propertyUrl = "https://voyehomes.com/"+General.GetUrlSlug(r["perma_title"].ToString()+"-"+General.EncodeString(r["property_id"].ToString()));

                                                emailBodyCustomer = emailBodyCustomer.Replace("#ADULTS#", r["adults_count"].ToString())
                                               .Replace("#CHILDREN#", r["children_count"].ToString())
                                               .Replace("#CHECKIN#", DateTime.Parse(r["checkin_date"].ToString()).ToString("yyyy MMM dd"))
                                               .Replace("#CHECKOUT#", DateTime.Parse(r["checkout_date"].ToString()).ToString("yyyy MMM dd"))
                                               .Replace("#NAME#", r["cust_name"].ToString())
                                               .Replace("#EMAIL#", r["cust_email"].ToString())
                                               .Replace("#PHONE#", r["cust_phone"].ToString())
                                               .Replace("#PROPERTY#", r["title"].ToString())
                                               .Replace("#REFERRAL_CODE#", r["referral_code"].ToString())
                                               .Replace("#ENQUIRYID#", enquiryId.ToString())
                                               .Replace("#THUMBNAIL#", r["thumbnail"].ToString())
                                               .Replace("#SHORT_DESCRIPTION#", r["short_description"].ToString())
                                               .Replace("#PROPERTY_URL#", propertyUrl);

                                                var subjectCustomer = General.GetSettingsValue("enquiry_email_subject_customer");

                                                General.SendMailMailgun(subjectCustomer, emailBodyCustomer, r["cust_email"].ToString(), General.GetSettingsValue("enquiry_from_email"), General.GetSettingsValue("enquiry_from_display_name"));


                                            }
                                        }

                                       

                                    }
                                }
                            }
                        }
                    }

                  
                   
                   
                }
                catch
                {
                    response.ActionStatus = "EXCEPTION";
                }

                return new OkObjectResult(response);
            }
            else
            {
                return BadRequest();
                
            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


    }
}
