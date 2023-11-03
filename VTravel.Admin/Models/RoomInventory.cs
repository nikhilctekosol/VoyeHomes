using System;

namespace VTravel.Admin.Models
{
    public class RoomInventory
    {
        public string roomId { get; set; }
        public string propertyId { get; set; }
        public DateTime invDate { get; set; }
        public int normalocc { get; set; }
        public int maxadults { get; set; }
        public int maxchildren { get; set; }
        public string occupancy { get; set; }
        public int occcount { get; set; }
        public decimal rate { get; set; }
    }
}
