using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.HostWeb.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Security.Claims;
using System.Linq;

namespace VTravel.HostWeb.Controllers
{

    [Route("api/property"), Authorize]
    public class PropertyController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public PropertyController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }



        [HttpGet, Route("get-list-sorted-by-name")]
        public IActionResult GetListSortName()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                List<Property> properties = new List<Property>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,thumbnail,address,city,property_status,sort_order 
                 FROM property WHERE is_active='Y' and property_status='ACTIVE' 
                and id in(select property_id from partner_user_property where partner_user_id={0}) 
                 ORDER BY title"
                                  , userId);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    properties.Add(
                        new Property
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            thumbnail = r["thumbnail"].ToString(),
                            address = r["address"].ToString(),
                            city = r["city"].ToString(),
                            propertyStatus = r["property_status"].ToString(),
                            sortOrder = Convert.ToInt32(r["sort_order"].ToString())
                        }
                        );

                }


                response.Data = properties;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get")]
        public IActionResult Get(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;


                Property property = new Property();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,reserve_alert,reserve_allowed,destination_id,thumbnail,address,city,property_status,sort_order 
                 ,short_description,long_description,latitude,longitude,state,country,property_type_id
                 ,display_radius,meta_title,meta_keywords,meta_description,email,phone,max_occupancy  
                 ,room_count,bathroom_count FROM property WHERE is_active='Y' AND id={0} and id in(select property_id from partner_user_property where partner_user_id={1}) ORDER BY sort_order", id, userId);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);//


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    property = new Property
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        title = r["title"].ToString(),
                        thumbnail = r["thumbnail"].ToString(),
                        address = r["address"].ToString(),
                        city = r["city"].ToString(),
                        propertyStatus = r["property_status"].ToString(),
                        sortOrder = Convert.ToInt32(r["sort_order"].ToString()),
                        shortDescription = r["short_description"].ToString(),
                        longDescription = r["long_description"].ToString(),
                        latitude = Double.Parse(r["latitude"].ToString()),
                        longitude = Double.Parse(r["longitude"].ToString()),
                        state = r["state"].ToString(),
                        country = r["country"].ToString(),
                        displayRadius = float.Parse(r["display_radius"].ToString()),
                        metaTitle = r["meta_title"].ToString(),
                        metaKeywords = r["meta_keywords"].ToString(),
                        metaDescription = r["meta_description"].ToString(),
                        propertyTypeId = r["property_type_id"].ToString(),
                        destinationId = r["destination_id"].ToString(),
                        email = r["email"].ToString(),
                        phone = r["phone"].ToString(),
                        maxOccupancy = Convert.ToInt32(r["max_occupancy"].ToString()),
                        roomCount = Convert.ToInt32(r["room_count"].ToString()),
                        bathroomCount = Convert.ToInt32(r["bathroom_count"].ToString()),

                        reserveAlert = r["reserve_alert"].ToString(),
                        reserveAllowed = r["reserve_allowed"].ToString(),

                    };

                }

                response.Data = property;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

       
       

        [HttpGet, Route("get-amenity-list")]
        public IActionResult GetAmenityList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyAmenity> amenityList = new List<PropertyAmenity>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT t1.id,t1.amenity_name,
                                    CASE  
                                     WHEN t2.property_id <> 'null' THEN 1
                                     ELSE 0
                                    END AS status
                                    FROM amenity t1
                                    LEFT OUTER JOIN property_amenity t2 ON t1.id=t2.amenity_id AND t2.property_id={0}
                                     WHERE t1.is_active='Y' 
                                    ORDER BY t1.sort_order, t1.amenity_name", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    amenityList.Add(
                        new PropertyAmenity
                        {
                            amenityId = Convert.ToInt32(r["id"].ToString()),
                            amenityName = r["amenity_name"].ToString(),
                            status = Convert.ToInt32(r["status"].ToString())
                        }
                        );

                }


                response.Data = amenityList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-price-list")]
        public IActionResult GetPriceList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyPrice> priceList = new List<PropertyPrice>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT id,price_name,mrp,price FROM property_price WHERE property_id={0} 
                                    ORDER BY sort_order, price_name", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    priceList.Add(
                        new PropertyPrice
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            priceName = r["price_name"].ToString(),
                            mrp=float.Parse(r["mrp"].ToString()),
                            price = float.Parse(r["price"].ToString())
                        }
                        );

                }


                response.Data = priceList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        

        [HttpGet, Route("get-image-list")]
        public IActionResult GetImageList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyImage> imageList = new List<PropertyImage>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT id,url FROM property_image WHERE property_id={0}  
                                    ORDER BY sort_order, id", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    imageList.Add(
                        new PropertyImage
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            url = r["url"].ToString(),
                        }
                        );

                }


                response.Data = imageList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

       

        [HttpGet, Route("get-tag-list")]
        public IActionResult GetTagList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<PropertyTag> tagList = new List<PropertyTag>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT t1.id,t1.tag_name,
                                    CASE  
                                     WHEN t2.property_id <> 'null' THEN 1
                                     ELSE 0
                                    END AS status
                                    FROM tag t1
                                    LEFT OUTER JOIN property_tag t2 ON t1.id=t2.tag_id AND t2.property_id={0} 
                                    WHERE t1.is_active='Y'
                                    ORDER BY t1.sort_order, t1.tag_name", id
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    tagList.Add(
                        new PropertyTag
                        {
                            tagId = Convert.ToInt32(r["id"].ToString()),
                            tagName = r["tag_name"].ToString(),
                            status = Convert.ToInt32(r["status"].ToString())
                        }
                        );

                }


                response.Data = tagList;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

       

        [HttpGet, Route("get-room-list")]
        public IActionResult GetRoomList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {
                IEnumerable<Claim> claims = User.Claims;
                var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                List<Room> roomList = new List<Room>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"SELECT room.id,title,room_type_id,room.description,type_name FROM room INNER JOIN room_type ON room.room_type_id=room_type.id WHERE property_id={0}
                           
                           and property_id in(select property_id from partner_user_property where partner_user_id={1})
                           ORDER BY room.sort_order, title",id, userId
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    roomList.Add(
                        new Room
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            roomTypeId = r["room_type_id"].ToString(),
                            description = r["description"].ToString(),
                            typeName= r["type_name"].ToString()
                        }
                        );

                }


                response.Data = roomList;
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


