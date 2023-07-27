using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.HostWeb.Models
{
    public class PropertyAttribute
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public string attributeId { get; set; }
        public string attributeName { get; set; }
        public string longDescription { get; set; }

    }
  
}
