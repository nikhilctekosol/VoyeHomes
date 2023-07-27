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

namespace VTravel.Admin.Controllers
{

    [Route("api/blog"), Authorize(Roles = "ADMIN")]
    public class BlogController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public BlogController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }




        [HttpPost, Route("sort")]
        public IActionResult Sort([FromBody] SortData model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE blog SET sort_order=sort_order+{0} WHERE sort_order>={1};  
                  UPDATE page SET sort_order={1} WHERE id={2}", model.pushDownValue,
                                     model.sortOrder, model.itemId);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);



                    response.ActionStatus = "SUCCESS";
                    response.Message = "pages sorted";
                }
                else
                {
                    return BadRequest("Invalid sort page");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


        [HttpGet, Route("get-list")]
        public IActionResult GetList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<Blog> blogs = new List<Blog>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,blog_status,sort_order,url_slug,author_name,author_email,author_phone  
                 FROM blog WHERE is_active='Y' ORDER BY sort_order"
                                   );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    blogs.Add(
                        new Blog
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            title = r["title"].ToString(),
                            //content = r["content"].ToString(),
                            urlSlug = r["url_slug"].ToString(),                            
                            blogStatus = r["page_status"].ToString(),
                            sortOrder = Convert.ToInt32(r["sort_order"].ToString())
                        }
                        );

                }


                response.Data = blogs;
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

                SitePage sitePage = new SitePage();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id,title,content,page_status,sort_order,url_slug  
                 ,meta_title,meta_description,meta_keywords,author_name,author_email,author_phone FROM blog WHERE is_active='Y' AND id={0} ORDER BY sort_order", id
                                  );

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    sitePage = new SitePage
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        title = r["title"].ToString(),
                        content = r["content"].ToString(),
                        urlSlug = r["url_slug"].ToString(),
                        pageStatus = r["page_status"].ToString(),
                        sortOrder = Convert.ToInt32(r["sort_order"].ToString()),
                        metaTitle = r["meta_title"].ToString(),
                        metaDescription = r["meta_description"].ToString(),
                        metaKeywords = r["meta_keywords"].ToString()

                    };

                }

                response.Data = sitePage;
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
        public IActionResult Create([FromBody] Blog model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"INSERT INTO blog(title,content) VALUES('{0}','{1}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                     model.title, model.content);

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
                    return BadRequest("Invalid page details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPut, Route("update")]
        public IActionResult Update([FromBody] Blog model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE blog SET title='{0}',meta_title='{1}',meta_description='{2}',meta_keywords='{3}'
                                           ,author_name='{4}',author_email='{5}',author_phone='{6}' WHERE id={7}",
                                     model.title, model.metaTitle,model.metaDescription,model.metaKeywords
                                     ,model.authorName,model.authorEmail,model.authorPhone, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid blog details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPut, Route("update-about")]
        public IActionResult UpdateAbout([FromBody] SitePage model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE blog SET content='{0}' WHERE id={1}",
                           model.content, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid property details");
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
        public IActionResult UpdateStatus([FromBody] SitePage model, int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = string.Format(@"UPDATE blog SET blog_status='{0}' WHERE id={1}",
                           model.pageStatus, id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid page details");
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

                    var query = string.Format(@"UPDATE blog SET is_active='N' WHERE id={0}",
                           id);

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid page details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }


    }//
}


