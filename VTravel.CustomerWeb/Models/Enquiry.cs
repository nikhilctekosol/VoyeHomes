using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class Enquiry
    {
        public int  custId { get; set; }        
        public DateTime checkInDate { get; set; }
        public DateTime checkOutDate { get; set; }
        public int adultsCount { get; set; }
        public int childrenCount { get; set; }
        public int propertyId { get; set; }
        public string otp { get; set; }

    }
}
