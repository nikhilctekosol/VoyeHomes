using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class PriceData
    {
        public int id { get; set; }
        public DateTime invDate { get; set; }       
        public string roomId { get; set; }
        public string propertyId { get; set; }
        public double price { get; set; }


    }
}
