using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class RateplanDetails
    {
        public int id { get; set; }
        public int roomid { get; set; }
        public string room { get; set; }
        public int mealid { get; set; }
        public string meal { get; set; }
        public int occid { get; set; }
        public string occupancy { get; set; }
        public string rate { get; set; }
    }
}
