using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class ResDoc
    {
        public int id { get; set; }
        public int res_id { get; set; }
        public int doc_type_id { get; set; }
        public string doc_type_name { get; set; }
        public string url { get; set; }
        public string file_name { get; set; }

    }
  
}
