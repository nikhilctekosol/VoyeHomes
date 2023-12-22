using System;

namespace VTravel.Admin.Models
{
    public class ReservationData
    {
        public string room { get; set; }
        public DateTime checkin { get; set; }
        public DateTime checkout { get; set; }
        public int years06 { get; set; }
        public int years612 { get; set; }
        public int years12 { get; set; }
        public decimal amount { get; set; }
        public string isgst { get; set; }
        public decimal gstperc { get; set; }
        public decimal gstamount { get; set; }
    }
}
