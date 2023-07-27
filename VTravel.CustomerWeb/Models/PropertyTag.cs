using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class PropertyTag
    {
        public int tagId { get; set; }
        public int propertyId { get; set; }
        public int status { get; set; }
        public string tagName { get; set; }
        
    }
  
}
