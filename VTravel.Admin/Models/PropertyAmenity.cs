using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class PropertyAmenity
    {
        public int amenityId { get; set; }
        public int propertyId { get; set; }
        public int status { get; set; }
        public string amenityName { get; set; }
        
    }
  
}
