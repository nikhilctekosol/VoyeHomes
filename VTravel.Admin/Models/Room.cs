using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class Room
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public string roomTypeId { get; set; }        
        public string title { get; set; }
        public string description { get; set; }
        public string typeName { get; set; }
        

    }
  
}
