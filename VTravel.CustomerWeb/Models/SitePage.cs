using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class SitePage
    {
        public int id { get; set; }       
        public string title { get; set; }
        public string pageStatus { get; set; }
        public int sortOrder { get; set; }
        public string content { get; set; }       
        public string metaTitle { get; set; }
        public string metaKeywords { get; set; }
        public string metaDescription { get; set; }
   
    }
}
