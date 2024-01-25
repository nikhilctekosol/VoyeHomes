using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class Room
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public string roomTypeId { get; set; }        
        public string title { get; set; }
        public string description { get; set; }
        public string typeName { get; set; }
        public int noofrooms { get; set; }
        public decimal base_rate { get; set; }
        

    }
  
}
