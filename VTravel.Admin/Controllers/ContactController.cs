using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using VTravel.Admin.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Transactions;

namespace VTravel.Admin.Controllers
{
    [Route("api/contact"), Authorize(Roles = "ADMIN")]
    public class ContactController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ContactController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            string projectRootPath = _hostingEnvironment.ContentRootPath;
        }


        [HttpGet, Route("get-list")]
        public IActionResult GetList()
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ContactList> contacts = new List<ContactList>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select id, contact_name, contact_no, is_active from alternate_contacts order by is_active desc;");

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    contacts.Add(
                        new ContactList
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            name = r["contact_name"].ToString(),
                            contactno = r["contact_no"].ToString(),
                            isactive = r["is_active"].ToString(),
                        }
                        );

                }


                response.Data = contacts;
                response.ActionStatus = "SUCCESS";



            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpGet, Route("get-active-list")]
        public IActionResult GetActiveList(int id)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                List<ContactList> contacts = new List<ContactList>();
                MySqlHelper sqlHelper = new MySqlHelper();

                var query = string.Format(@"select ac.id, ac.contact_name, ac.contact_no, case when pt.id is null then 0 else 1 end status, ac.is_active from property_contacts pt
                                            inner join alternate_contacts ac on ac.id = pt.contact_id
                                            where pt.property_id = {0}
                                            union 
                                            select ac.id, ac.contact_name, ac.contact_no, 0 status, ac.is_active from alternate_contacts ac
                                            where ac.id not in (select contact_id from property_contacts where property_id = {0})", id);

                DataSet ds = sqlHelper.GetDatasetByMySql(query);


                foreach (DataRow r in ds.Tables[0].Rows)
                {

                    contacts.Add(
                        new ContactList
                        {
                            id = Convert.ToInt32(r["id"].ToString()),
                            name = r["contact_name"].ToString(),
                            contactno = r["contact_no"].ToString(),
                            isactive = r["is_active"].ToString(),
                            status = Convert.ToInt32(r["status"]),
                        }
                        );

                }


                response.Data = contacts;
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
        public IActionResult Create([FromBody] ContactList model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        MySqlHelper sqlHelper = new MySqlHelper();
                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        var query = string.Format(@"select id, contact_name, contact_no, is_active from alternate_contacts where contact_no = '{0}'", model.contactno);

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);

                        if (ds != null)
                        {
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    query = string.Format(@"update alternate_contacts set contact_name = '{0}', updated_by = {1}, updated_on = '{2}' where id = {3}"
                                                , model.name, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ds.Tables[0].Rows[0]["id"]);

                                    ds = sqlHelper.GetDatasetByMySql(query);
                                }
                            }
                        }

                        query = string.Format(@"INSERT INTO alternate_contacts(contact_name, contact_no, created_by, created_on, is_active) VALUES('{0}','{1}',{2},'{3}','{4}');
                                         SELECT LAST_INSERT_ID() AS id;",
                                         model.name, model.contactno, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 'Y');

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
                        scope.Complete();
                    }

                }
                else
                {
                    return BadRequest("Invalid Contact details");
                }

            }
            catch (Exception ex)
            {
                response.ActionStatus = "EXCEPTION";
                response.Message = "Something went wrong";
            }
            return new OkObjectResult(response);


        }

        [HttpPost, Route("update")]
        public IActionResult Update([FromBody] ContactList model)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (model != null)
                {
                    using (var scope = new TransactionScope())
                    {
                        MySqlHelper sqlHelper = new MySqlHelper();
                        IEnumerable<Claim> claims = User.Claims;
                        var userId = claims.Where(c => c.Type == "id").FirstOrDefault().Value;

                        var query = string.Format(@"update alternate_contacts set contact_name = '{0}', contact_no = '{1}', updated_by = {2}, updated_on = '{3}' where id = {4}"
                                                , model.name, model.contactno, userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.id);

                        DataSet ds = sqlHelper.GetDatasetByMySql(query);
                        response.ActionStatus = "SUCCESS";

                        scope.Complete();
                    }

                }
                else
                {
                    return BadRequest("Invalid Contact details");
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
        public IActionResult Delete(int id, int mode)
        {
            ApiResponse response = new ApiResponse();
            response.ActionStatus = "FAILURE";
            response.Message = string.Empty;

            try
            {

                if (id > 0)
                {

                    MySqlHelper sqlHelper = new MySqlHelper();

                    var query = "";
                    if (mode == 1)
                    {
                        query = string.Format(@"UPDATE alternate_contacts SET is_active='N' WHERE id={0}", id);
                    }
                    else
                    {
                        query = string.Format(@"UPDATE alternate_contacts SET is_active='Y' WHERE id={0}", id);
                    }

                    DataSet ds = sqlHelper.GetDatasetByMySql(query);
                    response.ActionStatus = "SUCCESS";

                }
                else
                {
                    return BadRequest("Invalid Owner details");
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
