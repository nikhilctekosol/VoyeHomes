using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using VTravel.CustomerWeb.Models;

namespace VTravel.CustomerWeb.Controllers
{

    [ResponseCache(Duration = 30)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        IConfiguration _configuration;
        bool useRedis = false;
        private IDatabase _db;
        ConnectionMultiplexer connectionMultiplexer;
        ConfigurationOptions redisConfiguration;
       
        string redisConnectionString;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            try
            { 
                _logger = logger;
                _configuration = configuration;
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

                    //// Get Redis connection string
                    //var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");
                    //var redisConnectionParts = redisConnectionString.Split(new[] { "://", "@", ":" }, StringSplitOptions.RemoveEmptyEntries);
                    //var redisUser = redisConnectionParts[1];
                    //var redisPassword = redisConnectionParts[2];
                    //var redisHost = redisConnectionParts[3];
                    //var redisPort = Convert.ToInt32(redisConnectionParts[4]);

                    //// Connect to Redis
                    //redisConfiguration = new ConfigurationOptions
                    //{
                    //    EndPoints = { $"{redisHost}:{redisPort}" },
                    //    Password = redisPassword,
                    //    User = redisUser
                    //};


                }



            }
            catch (Exception ex)
            {
                General.LogException(ex);
               
            }
        }

        public async Task<IActionResult> Index()//
        {
            HomeViewModel indexViewModel = new HomeViewModel();
            
            try
            {
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT 
                                    meta_title,meta_keywords,meta_description                           
                                 from page p  
                                 where p.title='{0}' AND  p.is_active='Y'","Home"
                                        );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    ViewData["Title"] = r["meta_title"].ToString();
                    ViewData["Keywords"] = r["meta_keywords"].ToString();
                    ViewData["Description"] = r["meta_description"].ToString();


                }

                
                indexViewModel.tagList = new List<Tag>();
                indexViewModel.bannerList = new List<HeroBanner>();
                indexViewModel.destinationList = new List<Destination>();
                indexViewModel.featureList = new List<Feature>();
                //get top 10 property list grouped by tags in each group for home displayabl tags

                // Attempt to get the data from Redis first
                //var useRedis = bool.Parse(_configuration["UseRedis"]);

               
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
                                        indexViewModel.tagList.Add(JsonConvert.DeserializeObject<Tag>(tag));
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

               

                if(indexViewModel.tagList.Count == 0)
                {
                    // If the key does not exist in Redis, fetch the data from the database
                    query = string.Format(General.HOME_TAG_LIST_QUERY);

                    ds = sqlHelper.GetDatasetByMySql(query);

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        indexViewModel.tagList.Add(JsonConvert.DeserializeObject<Tag>(r[0].ToString()));

                    }
                }



                //query = string.Format(General.HOME_TAG_LIST_QUERY
                //                );

                //ds = sqlHelper.GetDatasetByMySql(query);


                //foreach (DataRow r in ds.Tables[0].Rows)
                //{

                //    indexViewModel.tagList.Add(JsonConvert.DeserializeObject<Tag>(r[0].ToString()));

                //}

                //property sorting
                foreach (Tag tagr in indexViewModel.tagList)
                {
                    if (tagr.propertyList != null)
                    {
                        tagr.propertyList = tagr.propertyList.OrderBy(p => p.sortOrder).ToArray<Property>();
                    }

                }

                query = string.Format(@"SELECT b.id, b.image_url, b.navigate_url, IFNULL(b.title, '') title, IFNULL(b.navigate_url, '') navigate_url, IFNULL(b.description, '') description, IFNULL(b.property_id, 0) property_id, IFNULL(b.destination, 0) dest_id
                                           , IFNULL(p.title, '') property, IFNULL(d.title, '') destination, IFNULL(b.banner_type, 'Promotion') banner_type, IFNULL(b.offer_text, '') offer_text, IFNULL(oc.class_name, '') offer_class, IFNULL(b.coupon_code, '') coupon_code
                                           FROM hero_banner b
                                           left join property p on p.id = b.property_id
                                           left join destination d on d.id = b.destination
                                           left join offer_classes oc on oc.class_name = b.offer_class
                                           where b.is_active='Y' order by b.sort_order",
                                 0);

                ds = null;
                ds = sqlHelper.GetDatasetByMySql(query);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            foreach (DataRow r in ds.Tables[0].Rows)
                            {

                                indexViewModel.bannerList.Add(
                                    new HeroBanner
                                    {
                                        id = Convert.ToInt32(r["id"].ToString()),
                                        image_url = r["image_url"].ToString(),
                                        navigate_url = r["navigate_url"].ToString() == "" ? (r["property_id"].ToString() == "0" ? (r["dest_id"].ToString() == "0" ? "" : 
                                        "destination/"+@General.GetUrlSlug(r["destination"].ToString())+"-"+ @General.EncodeString(r["dest_id"].ToString())) 
                                        : @General.GetUrlSlug(r["property"].ToString()) +"-"+@General.EncodeString(r["property_id"].ToString())) : r["navigate_url"].ToString(),
                                        title = r["title"].ToString(),
                                        description = r["description"].ToString(),
                                        property_id = Convert.ToInt32(r["property_id"].ToString()),
                                        destination = Convert.ToInt32(r["dest_id"].ToString()),
                                        bannertype = r["banner_type"].ToString(),
                                        offertext = r["offer_text"].ToString(),
                                        offerclass = r["offer_class"].ToString(),
                                        couponcode = r["coupon_code"].ToString()

                                    });

                            }


                        }

                    }

                }


                query = string.Format(@"SELECT id, title,description,thumbnail, img_url
                                           FROM destination WHERE  is_active='Y' ORDER BY sort_order");

                ds = null;
                ds = sqlHelper.GetDatasetByMySql(query);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            foreach (DataRow r in ds.Tables[0].Rows)
                            {
                               
                                indexViewModel.destinationList.Add(
                                    new Destination
                                    {
                                        id = Convert.ToInt32(r["id"].ToString()),
                                        img_url = r["img_url"].ToString(),
                                        thumbnail = r["thumbnail"].ToString(),
                                        title = r["title"].ToString(),
                                        description = r["description"].ToString(),
                                        
                                    });

                            }


                        }

                    }

                }


                query = string.Format(@"SELECT id, title,description,thumbnail  
                                           FROM feature WHERE  is_active='Y' ORDER BY sort_order");

                ds = null;
                ds = sqlHelper.GetDatasetByMySql(query);

                if (ds != null)
                {
                    if (ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            foreach (DataRow r in ds.Tables[0].Rows)
                            {

                                indexViewModel.featureList.Add(
                                    new Feature
                                    {
                                        id = Convert.ToInt32(r["id"].ToString()),
                                        thumbnail = r["thumbnail"].ToString(),
                                        title = r["title"].ToString(),
                                        description = r["description"].ToString(),

                                    });

                            }


                        }

                    }

                }
            }
            catch(Exception ex)
            {
                General.LogException(ex);
                return Redirect("Home/Error");
            }

            return View(indexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
