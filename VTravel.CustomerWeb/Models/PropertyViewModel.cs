using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class PropertyViewModel
    {
       public Property property { get; set; }
       public string terms { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string short_desc { get; set; }
        public string banner_url { get; set; }


        public List<Property> propertyList { get; set; }
        public List<Property> promoPropertyList { get; set; }
    }

}
