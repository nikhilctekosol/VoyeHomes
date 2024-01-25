using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class RoomMeals
    {
        public int id { get; set; }
        public int roomid { get; set; }
        public string mealplan { get; set; }
        public string check { get; set; }
    }
}
