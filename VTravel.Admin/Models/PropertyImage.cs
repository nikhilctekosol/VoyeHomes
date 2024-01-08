using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class PropertyImage
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public string url { get; set; }

        public string image_alt { get; set; }
        public string categoryid { get; set; }
        public string category { get; set; }
        public string subcategoryid { get; set; }
        public string subcategory { get; set; }
        public string room { get; set; }
        public string roomid { get; set; }
    }
  
}
