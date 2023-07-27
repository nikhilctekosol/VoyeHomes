using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RazorPayConnector
{
    public class RzrManager
    { 
    
        public string BaseUrl { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public RzrManager(string baseUrl, string key, string secret)
        {
            this.BaseUrl = baseUrl;
            this.Key = key;
            this.Secret = secret;

        }

        public RzrOrderResponse CreateOrder(RzrOrderRequest request)
        {
            RzrOrderResponse response = new RzrOrderResponse();//
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUrl);
                var byteArray = Encoding.ASCII.GetBytes
                    (string.Format(@"{0}:{1}",
                    Key, Secret));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                HttpResponseMessage rzRes = client.PostAsync
                    ("orders", httpContent).Result;
                rzRes.EnsureSuccessStatusCode();

                string result = rzRes.Content.ReadAsStringAsync().Result;

                response = JsonConvert.DeserializeObject<RzrOrderResponse>(result);


            }
            catch(Exception ex)
            {

            }
            return response;
        }

        public RzrOrderResponse GetOrder(string orderId)
        {
            RzrOrderResponse response = new RzrOrderResponse();
            try
            {
                //var BaseUrl = General.GetSettingsValue("rzr_base_url"); 
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUrl);
                var byteArray = Encoding.ASCII.GetBytes
                    (string.Format(@"{0}:{1}",
                    Key, Secret));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage rzRes = client.GetAsync
                        ("orders/" + orderId).Result;
                rzRes.EnsureSuccessStatusCode();

                string result = rzRes.Content.ReadAsStringAsync().Result;

                response = JsonConvert.DeserializeObject<RzrOrderResponse>(result);


            }
            catch (Exception ex)
            {

            }
            return response;
        }
    }
}
