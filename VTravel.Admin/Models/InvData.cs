using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class InvData
    {
        public int id { get; set; }
        public DateTime invDate { get; set; }       
        public string roomId { get; set; }
        public string propertyId { get; set; }
        public int totalQty { get; set; }
        public int bookedQty { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public double price { get; set; }
        public double extraBedPrice { get; set; }
        public double childPrice { get; set; }
        public string mode { get; set; }
        
    }
}
