using System;

namespace VTravel.Admin.Models
{
    public class RateplanResult
    {
        public int id { get; set; }
        public string propertyId { get; set; }
        public string invDate { get; set; }
        public int rp_id { get; set; }
        public string rp_name { get; set; }
        public string rp_color { get; set; }
    }
}
