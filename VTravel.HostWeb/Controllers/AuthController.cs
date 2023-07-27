using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using VTravel.HostWeb.Models;
using Microsoft.AspNetCore.Hosting;

namespace VTravel.HostWeb.Controllers
{

    [Route("api/auth")]
    public class AuthController : Controller
    {
        Encryption enc = new Encryption();

        [HttpPost, Route("sign-in")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid client request");
            }

            bool isValid = false;

            //var ec = enc.Encrypt(model.password);

            //var dc = enc.Decrypt(ec);

            var query = string.Format(
                "SELECT id,name_of_user,user_role FROM partner_user WHERE user_name='{0}' AND password_enc='{1}' AND is_active='Y'",
                model.email, enc.Encrypt(model.password));

            MySqlHelper MySqlHelper = new MySqlHelper();
            DataSet ds = MySqlHelper.GetDatasetByMySql(query);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {

                    isValid = true;

                }

            }

            if (isValid)
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(General.GetSettingsValue("partner_jwtkey")));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                   new Claim("userName", model.email),
                    new Claim(ClaimTypes.Role, ds.Tables[0].Rows[0]["user_role"].ToString().ToUpper()),
                    new Claim("role", ds.Tables[0].Rows[0]["user_role"].ToString().ToUpper()),
                    new Claim("id", ds.Tables[0].Rows[0]["id"].ToString().ToUpper()),
                    new Claim("nameOfUser", ds.Tables[0].Rows[0]["name_of_user"].ToString().ToUpper()),
                              //
                };
                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:62003",
                    audience: "http://localhost:62003",
                    claims: claims,
                    expires: DateTime.Now.AddDays(30),
                    signingCredentials: signinCredentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

                return Ok(new { Token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpDelete, Route("sign-out")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return Ok();
        }

              
        [HttpPost, Route("request-pass")]
        public IActionResult RequestPass([FromBody] RequestPassModel model)
        {
            try
            {
                MySqlHelper MySqlHelper = new MySqlHelper();

                var tokenKey = Guid.NewGuid().ToString();
                var query = string.Format(
                "UPDATE partner_user SET password_token='{0}', password_token_expiry='{1}' WHERE user_name='{2}' AND is_active='Y'",
                tokenKey, DateTime.Now.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss"), model.email);

                DataSet ds = MySqlHelper.GetDatasetByMySql(query);
                string emailSubject = "";
                emailSubject = "Portal Password Reset";

                string body = General.GetSettingsValue("host_passwordreset_email_template");

                body = body.Replace("#RESET_LINK#", General.GetSettingsValue("hostPasswordResetBaseUrl") + "?tokenKey=" + tokenKey);

                var emailResponse = General.SendMailMailgun(emailSubject,
                    body, model.email,
                   General.GetSettingsValue("password_rest_from_email"),
                    General.GetSettingsValue("password_rest_from_display_name"));

                if (emailResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {

            }

            return Unauthorized();


        }

        [HttpPut, Route("reset-pass")]
        public IActionResult ResetPass([FromBody] ResetPassModel model)
        {
            try
            {
                MySqlHelper MySqlHelper = new MySqlHelper();


                var query = string.Format(
                "UPDATE partner_user SET password_enc='{0}' WHERE  password_token_expiry > '{1}' AND password_token='{2}' AND is_active='Y'",
                enc.Encrypt(model.password), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), model.tokenKey);

                DataSet ds = MySqlHelper.GetDatasetByMySql(query);

                return Ok();
            }
            catch (Exception ex)
            {

            }

            return Unauthorized();

        }
    }
}


