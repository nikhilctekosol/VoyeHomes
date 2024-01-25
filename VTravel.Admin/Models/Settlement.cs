using System;

namespace VTravel.Admin.Models
{
    public class Settlement
    {
        public string res_id { get; set; }
        public string created_date { get; set; }
        //public int propertyId { get; set; }
        public string Property { get; set; }
        //public int channelId { get; set; }
        public string channelName { get; set; }
        //public int destId { get; set; }
        public string destination { get; set; }
        public string custName { get; set; }
        public string Nationality { get; set; }
        public string custMail { get; set; }
        public string custPhone { get; set; }
        public string resStatus { get; set; }
        public decimal agreedRent { get; set; }
        public decimal bookingAmount { get; set; }
        public decimal commission { get; set; }
        public string isGst { get; set; }
        public decimal gst { get; set; }
        public int noOfGuests { get; set; }
        public int noOfUnits { get; set; }
        public decimal rentAfterOTA { get; set; }
        public decimal higherorEligible { get; set; }
        public decimal shortage { get; set; }
        public decimal hostShare { get; set; }
        public decimal voyeCommission { get; set; }
        public decimal discount { get; set; }
        public decimal voyeShare { get; set; }
        public decimal advance { get; set; }
        public decimal part { get; set; }
        public decimal balance { get; set; }
    }
}
