using System;
using System.Collections.Generic;

namespace VTravel.Admin.Models
{
    public class ReservDataNew
    {
        public int id { get; set; }
        public string propertyId { get; set; }
        public string customerId { get; set; }
        public string custName { get; set; }
        public string custEmail { get; set; }
        public string custPhone { get; set; }
        public string bookingChannelId { get; set; }
        public string details { get; set; }
        public string isHostBooking { get; set; }
        public string noOfRooms { get; set; }
        public int noOfGuests { get; set; }
        public float finalAmount { get; set; }
        public float advancepayment { get; set; }
        public float partpayment { get; set; }
        public float balancepayment { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public string created_by { get; set; }
        public string updated_by { get; set; }
        public string enquiry_ref { get; set; }
        public string res_status { get; set; }
        public List<ReservedRoomData> rooms { get; set; }
    }
}
