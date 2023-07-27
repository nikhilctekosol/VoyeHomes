using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class PropertyPrice
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public float mrp { get; set; }
        public float price { get; set; }
        public string priceName { get; set; }

    }
}
