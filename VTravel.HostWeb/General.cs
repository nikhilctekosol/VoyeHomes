using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VTravel.HostWeb.Models;
using Microsoft.AspNetCore.Authorization;
using RestSharp;
using RestSharp.Authenticators;

public class General
{
    public static string passEncKey = "dcd90e2bbe8f42189ed58c002c7af0bc";
    public static void LogException(Exception ex)
    {
        try
        {
            string FileName = "\\Logs\\error_" + DateTime.Today.ToString("yyyMMdd") + ".txt";
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
            string FileName = "\\Logs\\reqresp_" + DateTime.Today.ToString("yyyMMdd") + ".txt";
            if (!CheckIfLogFileExists(FileName))//log file not exists
            {
                CreateLogFile(FileName); //create log file
            }
            if (CheckIfLogFileExists(FileName))
            {
                SaveReqResp(FileName, content,type);
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

                sw.WriteLine("type:"+type+ "\n"+"DateTime: "+DateTime.Now.ToString()
                    +"\nLog: " + content);

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

    public static string ComputeSHA256Hash(string text)
    {
        using (var sha256 = new SHA256Managed())
        {
            return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "");
        }
    }

    public static IRestResponse SendMailMailgun(string subject, string body, string to, string fromEmail, string fromDisplayName)
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
            foreach (var mid in toIds)
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
            var emailResponse = client.Execute(request);
            return emailResponse;
        }
        catch
        {

        }
        return null;
    }

}