using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class ReservModel
    {
        public int propertyId { get; set; }

        public int roomId { get; set; }
        public string roomName { get; set; }

        public string custName { get; set; }
        public string custEmail { get; set; }
        public string countryCode { get; set; }
        public string custPhone { get; set; }
        public string daterange { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int adultsCount { get; set; }
        public int childrenCount { get; set; }
        public string referralCode { get; set; }
        public string agreeTerms { get; set; }

        public string priceListJson { get; set; }

        public double totalPricePerRoom { get; set; }
    }
}
