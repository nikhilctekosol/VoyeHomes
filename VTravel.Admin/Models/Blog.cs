using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class Blog
    {
        public int id { get; set; }        
        public string title { get; set; }       
        public string content { get; set; }
        public string blogStatus { get; set; }
        public int sortOrder { get; set; }
        public string urlSlug { get; set; }
        public string metaTitle { get; set; }
        public string metaKeywords { get; set; }
        public string metaDescription { get; set; }
        public string authorName { get; set; }
        public string authorEmail { get; set; }
        public string authorPhone { get; set; }
    }

    

}
