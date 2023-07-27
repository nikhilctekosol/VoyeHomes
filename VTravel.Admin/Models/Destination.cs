using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class Destination
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string short_desc { get; set; }
        public string thumbnail { get; set; }
        public string thumbnail_alt { get; set; }

        public string meta_title { get; set; }
        public string meta_keywords { get; set; }
        public string meta_description { get; set; }




    }
}
