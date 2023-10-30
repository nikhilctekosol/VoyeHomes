using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class RoomOccupancy
    {
        public int id { get; set; }
        public int roomid { get; set; }
        public string occupancy { get; set; }
        public string check { get; set; }
        public int occcount { get; set; }
    }
}
