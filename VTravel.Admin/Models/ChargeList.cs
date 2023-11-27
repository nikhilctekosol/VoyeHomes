using System;

namespace VTravel.Admin.Models
{
    public class ChargeList
    {
        public int id { get; set; }
        public int propertyid { get; set; }
        public string name { get; set; }
        public string chargetype { get; set; }
        public decimal amount { get; set; }
        public decimal percentage { get; set; }
        public DateTime effective { get; set; }
    }
}
