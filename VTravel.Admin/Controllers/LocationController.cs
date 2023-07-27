using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;


namespace VTravel.Admin.Controllers
{
    
    [Route("api/location"), Authorize(Roles = "ADMIN")]
    public class LocationController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public LocationController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }
        
            
       
        [HttpGet, Route("get-country-list")]
        public IActionResult GetCountryList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Country> countries = new List<Country>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,country_code,country_name 
                              FROM country"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    countries.Add(
                        new Country
                        {
                            id=Convert.ToInt32(r["id"].ToString()),
                            countryCode = r["country_code"].ToString(),
                            countryName = r["country_name"].ToString()
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

        [HttpGet, Route("get-state-list")]
        public IActionResult GetStateList(string countryCode)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<State> states = new List<State>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,state_code,state_name,country_code  
                              FROM state WHERE country_code='{0}'", countryCode
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    states.Add(
                        new State
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            countryCode = r["country_code"].ToString(),
                            stateCode = r["state_code"].ToString(),
                            stateName = r["state_name"].ToString()
                        }
                        );

                }


                response.Data = states;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-city-list")]
        public IActionResult GetCityList(string countryCode,string stateCode)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<City> cities = new List<City>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,city_code,state_code,country_code, city_name 
                              FROM city WHERE country_code='{0}'
                   AND  state_code='{1}' AND is_active='Y'  ORDER BY city_name", countryCode,stateCode
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    cities.Add(
                        new City
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            countryCode = r["country_code"].ToString(),
                            stateCode = r["state_code"].ToString(),
                            cityCode = r["city_code"].ToString(),
                            cityName = r["city_name"].ToString()
                        }
                        );

                }


                response.Data = cities;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-city-list-all")]
        public IActionResult GetCityListAll()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<City> cities = new List<City>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,city_code,state_code,country_code, city_name 
                              FROM city WHERE is_active='Y' ORDER BY city_name"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    cities.Add(
                        new City
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            countryCode = r["country_code"].ToString(),
                            stateCode = r["state_code"].ToString(),
                            cityCode = r["city_code"].ToString(),
                            cityName = r["city_name"].ToString()
                        }
                        );

                }


                response.Data = cities;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPost, Route("create-city")]
        public IActionResult CreateCity([FromBody] City model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO city(city_name,city_code,state_code,country_code) VALUES('{0}','{1}','{2}','{3}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.cityName, model.cityCode,model.stateCode,model.countryCode);

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
                else
                {
                    return BadRequest("Invalid Tag details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPut, Route("update-city")]
        public IActionResult UpdateCity([FromBody] City model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE city SET city_name='{0}',city_code='{1}',state_code='{2}',country_code='{3}' WHERE id={4}",
                                     model.cityName, model.cityCode, model.stateCode, model.countryCode, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Tag details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpDelete, Route("delete-city")]
        public IActionResult DeleteCity(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (id > 0)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE city SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Tag details");
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


