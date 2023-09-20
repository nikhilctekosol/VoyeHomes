using System;

namespace VTravel.Admin.Models
{
    public class RPData
    {
        public int id { get; set; }
        public string propertyId { get; set; }
        public string roomId { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string mode { get; set; }
        public int rateplan { get; set; }
    }
}
