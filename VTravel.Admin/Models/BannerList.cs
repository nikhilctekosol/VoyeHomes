using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class BannerList
    {
        public int id { get; set; }
        public string image_url { get; set; }
        public string image_alt { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string navigate_url { get; set; }
        public string property_id { get; set; }
        public string destination_id { get; set; }
        public string property { get; set; }
        public string destination { get; set; }
        public string show_in_home { get; set; }
        public string active { get; set; }
    }
}
