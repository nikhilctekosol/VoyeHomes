﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VTravel.CustomerWeb.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Web;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;

public class General
{
    public static string passEncKey = "dc3rasg3t456h46jflg9sc002c7af0bc";
    public const string HOME_TAG_LIST_QUERY = @"SET SESSION group_concat_max_len = 100000;
                   SELECT JSON_OBJECT(
                  'id',t.id,'tagName',t.tag_name,'description ',t.description
                 ,'propertyList',(SELECT CAST(CONCAT('[',
                                 GROUP_CONCAT(
                                  JSON_OBJECT(
                                    'id',p.id,'title',p.title,'perma_title',p.perma_title,'sortOrder',pt.sort_order,'destinationId',p.destination_id,'thumbnail',p.thumbnail,'city',c.city_name,'state',s.state_name,'country',cn.country_name
                                ,'priceList',(SELECT CAST(CONCAT('[',
                                GROUP_CONCAT(
                                  JSON_OBJECT(
                                    'id',pr.id,'priceName',pr.price_name,'mrp',pr.mrp,'price',pr.price)),
                                ']')
                         AS JSON) from property_price pr where pr.property_id = p.id ORDER BY pr.sort_order) )),
                                ']')
                         AS JSON) from property p INNER JOIN property_tag pt ON p.id=pt.property_id 
                         INNER JOIN city c ON p.city=c.city_code 
                         INNER JOIN state s ON p.state=s.state_code 
                         INNER JOIN country cn ON p.country=cn.country_code  
                        where pt.tag_id = t.id AND p.is_active='Y' AND p.property_status='ACTIVE' ORDER BY p.sort_order  LIMIT 10)                     
                 ) from tag t  WHERE t.is_active='Y' AND t.show_in_home='Y'  ORDER BY t.sort_order";
    public static void LogException(Exception ex)
    {
        try
        {


            string FileName = string.Format(
                @"{0}\Logs\error_" + DateTime.Today.ToString("yyyMMdd") + ".txt"
                , VTravel.CustomerWeb.Startup.contentRoot);
            if (!CheckIfLogFileExists(FileName))//log file not exists
            {
                CreateLogFile(FileName); //create log file
            }
            if (CheckIfLogFileExists(FileName))
            {
                SaveException(FileName, ex);
            }

        }
        catch
        {
        }
    }

    public static void LogReqResp(string content, string type)
    {
        try
        {
            string FileName = string.Format(
               @"{0}\Logs\reqresp_" + DateTime.Today.ToString("yyyMMdd") + ".txt"
               , VTravel.CustomerWeb.Startup.contentRoot);

            if (!CheckIfLogFileExists(FileName))//log file not exists
            {
                CreateLogFile(FileName); //create log file
            }
            if (CheckIfLogFileExists(FileName))
            {
                SaveReqResp(FileName, content, type);
            }

        }
        catch
        {
        }
    }
    private static bool SaveReqResp(string fileName, string content, string type)
    {
        try
        {
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(DateTime.Now.ToString());

                StringBuilder s = new StringBuilder();

                sw.WriteLine("type:" + type + "\n" + "DateTime: " + DateTime.Now.ToString()
                    + "\nLog: " + content);

                sw.WriteLine();
                sw.WriteLine();

            }
            return true;
        }
        catch
        {
        }
        return false;
    }
    private static bool SaveException(string fileName, Exception ex)
    {
        try
        {
            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(DateTime.Now.ToString());
                sw.WriteLine("ex: " + ex.ToString());
                sw.WriteLine("ex.Message: " + ex.Message);
                sw.WriteLine("ex.StackTrace: " + ex.StackTrace);
                sw.WriteLine("ex.Source: " + ex.Source);
                StringBuilder s = new StringBuilder();
                while (ex != null)
                {
                    s.AppendLine("Exception type: " + ex.GetType().FullName);
                    s.AppendLine("Message       : " + ex.Message);
                    s.AppendLine("Stacktrace:");
                    s.AppendLine(ex.StackTrace);
                    s.AppendLine();
                    ex = ex.InnerException;
                }
                sw.WriteLine("innerexceptions: " + s);

                sw.WriteLine();
                sw.WriteLine();

            }
            return true;
        }
        catch
        {
        }
        return false;
    }
    private static bool CreateLogFile(string fileName)
    {
        try
        {
            File.CreateText(fileName).Close();
            return true;
        }
        catch (Exception ex)
        {
        }
        return false;
    }
    private static bool CheckIfLogFileExists(string fileName)
    {
        try
        {
            return File.Exists(fileName);

        }
        catch
        {
        }
        return false;
    }


    public static string GetSettingsValue(string key)
    {
        try
        {
            MySqlHelper sqlHelper = new MySqlHelper();
            DataSet ds = sqlHelper.GetDatasetByMySql(string.Format
                (@"SELECT settings_value FROM system_settings WHERE settings_name='{0}'", key));


            if (ds.Tables.Count > 0)
            {
                return ds.Tables[0].Rows[0]["settings_value"].ToString();
            }
            return null;
        }
        catch (Exception ex)
        {
            // General.LogException(ex);
            return null;
        }
    }

    public static string SetSettingsValue(string key, string value)
    {
        try
        {
            MySqlHelper sqlHelper = new MySqlHelper();

            sqlHelper.GetExecuteNonQueryByCommand("SP_SetSettingsValue");



            return "success";
        }
        catch (Exception ex)
        {
            // General.LogException(ex);

            return null;
        }
    }

    public static string Encrypt(string text, string key)
    {
        var _key = Encoding.UTF8.GetBytes(key);

        using (var aes = Aes.Create())
        {
            using (var encryptor = aes.CreateEncryptor(_key, aes.IV))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(text);
                        }
                    }

                    var iv = aes.IV;

                    var encrypted = ms.ToArray();

                    var result = new byte[iv.Length + encrypted.Length];

                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }
    }

    public static string Decrypt(string encrypted, string key)
    {
        var b = Convert.FromBase64String(encrypted);

        var iv = new byte[16];
        var cipher = new byte[16];

        Buffer.BlockCopy(b, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(b, iv.Length, cipher, 0, iv.Length);

        var _key = Encoding.UTF8.GetBytes(key);

        using (var aes = Aes.Create())
        {
            using (var decryptor = aes.CreateDecryptor(_key, iv))
            {
                var result = string.Empty;
                using (var ms = new MemoryStream(cipher))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            result = sr.ReadToEnd();
                        }
                    }
                }

                return result;
            }
        }
    }

    public static string RemoveAccents(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        text = text.Normalize(NormalizationForm.FormD);
        char[] chars = text
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
            != UnicodeCategory.NonSpacingMark).ToArray();

        return new string(chars).Normalize(NormalizationForm.FormC);
    }

    public static string GetUrlSlug(string phrase)
    {
        // Remove all accents and make the string lower case.  
        string output = RemoveAccents(phrase).ToLower();

        // Remove all special characters from the string.  
        output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

        // Remove all additional spaces in favour of just one.  
        output = Regex.Replace(output, @"\s+", " ").Trim();

        // Replace all spaces with the hyphen.  
        output = Regex.Replace(output, @"\s", "-");

        // Return the slug.  
        return output;
    }

  
    public static string alphas = "huijkylmacdezfguvnwbj";
    public static string EncodeString(string inputString)
    {
        try
        {
            string encryptedString=string.Empty;   
            
            for(int i=0;i< inputString.Length;i++)
            {
                int alphaPos= int.Parse(inputString[i].ToString());
                encryptedString += alphas[alphaPos];
            }

            return encryptedString;
        }
        catch (Exception ex)
        {
            // General.LogException(ex);
            return null;
        }
    }

    public static string DecodeString(string inputString)
    {
        try
        {
            string encryptedString = string.Empty;

            for (int i = 0; i < inputString.Length; i++)
            {
                int alphaPos = alphas.IndexOf(inputString[i].ToString());
                encryptedString += alphaPos.ToString();
            }

            return encryptedString;
        }
        catch (Exception ex)
        {
            // General.LogException(ex);
            return null;
        }
    }

    public static string GenerateOTP()
    {
        string sixDigitNumber ="845363";
        try
        {
            Random r = new Random();
            int randNum = r.Next(1000000);
            sixDigitNumber = randNum.ToString("D6");
            
        }
        catch(Exception ex){
            
        }
        return sixDigitNumber;
    }

    public static bool SendEmail(string subject, string body, string to, string bcc, string fromEmail, string fromDisplayName,
          string SmtpUser, string SmtpPass, string SmtpHost, string SmtpPort, List<MailAttachment> attachments)
    {


        try
        {
            #region PrepareEmail



            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(to);
            if ((bcc.Length > 2) && (bcc.Contains('@')))
            {
                mailMessage.Bcc.Add(bcc);
            }
            mailMessage.From = new MailAddress(fromEmail, fromDisplayName);

            // mailMessage.From = new MailAddress(General.GetSettingsValue("SmtpUser"));

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            if (attachments != null)
            {
                foreach (MailAttachment mailAttachment in attachments)
                {

                    try
                    {
                        Attachment attachment = new Attachment(mailAttachment.FullPath);
                        attachment.ContentDisposition.FileName = mailAttachment.AttachmentDisplayName;
                        mailMessage.Attachments.Add(attachment);
                    }
                    catch
                    {

                    }
                }
            }





            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(SmtpUser, SmtpPass);

            client.Port = Convert.ToInt32(SmtpPort);
            client.Host = SmtpHost;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Send(mailMessage);
            #endregion




            return true;

        }
        catch (Exception ex)
        {
            //General.LogException(ex);
            return false;
        }
    }

    public static bool sendSMS(string smsContent,string number)
    {
        try
        {
            String message = HttpUtility.UrlEncode(smsContent);
            using (var wb = new WebClient())
            {
                byte[] response = wb.UploadValues(General.GetSettingsValue("txtlocal_url"),
                    new NameValueCollection()
                {
                {"apikey" , General.GetSettingsValue("txtlocal_key")},
                {"numbers" , number},
                {"message" , message},
                {"sender" , General.GetSettingsValue("txtlocal_sender")}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);

                TxtLocalResponse resp = JsonConvert.DeserializeObject<TxtLocalResponse>(result);
                if (resp.status == "success")
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            General.LogException(ex);
        }
        return false;
    }

    public static async Task<IRestResponse> SendMailMailgun(string subject, string body, string to,  string fromEmail, string fromDisplayName)
    {
        try
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri(General.GetSettingsValue("mailgun_base_url"));
            client.Authenticator =
                new HttpBasicAuthenticator("api",
                                            General.GetSettingsValue("mailgun_api_key"));
            RestRequest request = new RestRequest();
            request.AddParameter("domain", General.GetSettingsValue("mailgun_domain"), ParameterType.UrlSegment);
            request.Resource = General.GetSettingsValue("mailgun_domain") + "/messages";
            request.AddParameter("from", fromDisplayName + " <" + fromEmail + ">");

            var toIds = to.Split(',');
            foreach(var mid in toIds)
            {
                request.AddParameter("to", mid);
            }
            
            //request.AddParameter("cc", "baz@example.com");
            //request.AddParameter("bcc", "bar@example.com");
            request.AddParameter("subject", subject);
            //request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("html", body);
            //request.AddFile("attachment", Path.Combine("files", "test.jpg"));
            //request.AddFile("attachment", Path.Combine("files", "test.txt"));
            request.Method = Method.POST;
            var emailResponse = await client.ExecuteAsync(request);
            return emailResponse;
        }
        catch
        {

        }
        return null;
    }

    public static List<SitePage> GetSitePages()
    {
        List<SitePage> pages = new List<SitePage>();
        try
        {

           
            MySqlHelper sqlHelper = new MySqlHelper();

            var query = string.Format(@"select id,title,content,page_status,sort_order,url_slug  
                 FROM page WHERE is_active='Y' AND page_status='ACTIVE' AND show_in_menu='Y' ORDER BY sort_order"
                               );

            DataSet ds = sqlHelper.GetDatasetByMySql(query);


            foreach (DataRow r in ds.Tables[0].Rows)
            {

                pages.Add(
                    new SitePage
                    {
                        id = Convert.ToInt32(r["id"].ToString()),
                        title = r["title"].ToString(),
                        content = r["content"].ToString(),                        
                        pageStatus = r["page_status"].ToString(),
                        sortOrder = Convert.ToInt32(r["sort_order"].ToString())
                    }
                    );

            }


        }
        catch (Exception ex)
        {
           
        }

        return pages;
    }

}

public class MailAttachment
{
    public string AttachmentName { get; set; }
    public string AttachmentDisplayName { get; set; }
    public string FullPath { get; set; }

}