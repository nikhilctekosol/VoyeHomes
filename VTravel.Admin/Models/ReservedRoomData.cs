using System;

namespace VTravel.Admin.Models
{
    public class ReservedRoomData
    {
        public int id { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string roomId { get; set; }
        public string room { get; set; }
        public int years06 { get; set; }
        public int years612 { get; set; }
        public int years12 { get; set; }
        public int noOfGuests { get; set; }
        public int amount { get; set; }
    }
}
