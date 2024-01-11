using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class SettlementHistory
    {
        public int id { get; set; }
        public string propertyid { get; set; }
        public string property { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public string html { get; set; }
        public string is_approved { get; set; }
        public decimal host_share { get; set; }
        public string updated_on { get; set; }
        public string approved_on { get; set; }
    }
}
