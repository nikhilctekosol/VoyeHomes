using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class SuperHost
    {
        public int id { get; set; }
        public string host_name { get; set; }
        public int destination_id { get; set; }
        public string image { get; set; }

        public string destination_title { get; set; }


    }
}
