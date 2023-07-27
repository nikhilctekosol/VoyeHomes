using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.HostWeb.Models
{
    public class PropertyImage
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public string url { get; set; }
    }
  
}
