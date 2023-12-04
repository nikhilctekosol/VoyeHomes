using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class HeroBanner
    {
        public int id { get; set; }
        public string image_url { get; set; }
        public string navigate_url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int property_id { get; set; }
        public int destination { get; set; }
        public string bannertype { get; set; }
        public string offertext { get; set; }
        public string offerclass { get; set; }
        public string couponcode { get; set; }


    }
}
