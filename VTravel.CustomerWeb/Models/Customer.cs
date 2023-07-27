using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class Customer
    {
        public string custName { get; set; }
        public string custEmail { get; set; }
        public string custPhone { get; set; }
        public string otp { get; set; }
        public int custId { get; set; }
        public string referralCode { get; set; }
    }
}
