using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class SearchSuggestion
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public int parent_id { get; set; }
        public string parent_name { get; set; }
        public string parent_slug { get; set; }
        
        public string item_type { get; set; }



    }
  
}
