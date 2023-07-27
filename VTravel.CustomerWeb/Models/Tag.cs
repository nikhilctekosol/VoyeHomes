using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    

    public class Tag
    {
        public int id { get; set; }
        public string tagName { get; set; }
        public string banner_url { get; set; }
        public Property[] propertyList { get; set; }
    }
    
}
