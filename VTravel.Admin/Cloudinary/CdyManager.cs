using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace VTravel.Admin.Cloudinary1
{
    public class CdyManager
    {
        public CdyResponse Upload(CdyRequest request, string cloud_name,string resource_type)
        {
            CdyResponse cdyResponse = new CdyResponse();
            try
            {
                var BaseUrl = General.GetSettingsValue("cdy_upload_url");
                

                //var myContent = JsonConvert.SerializeObject(request);
                //var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                //var byteContent = new ByteArrayContent(buffer);

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
              
                HttpResponseMessage response = client.PostAsJsonAsync
                    (cloud_name+"/"+ resource_type + "/upload", request).Result;
                //response.EnsureSuccessStatusCode();

                string result = response.Content.ReadAsStringAsync().Result;


                //cdyResponse
                var rsp= JsonConvert.DeserializeObject<CdyResponse>(result);



            }
            catch (Exception ex)
            {
                General.LogException(ex);
            }
            return cdyResponse;
        }
    }
}
