using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class GiftCardEnquiryModel
    {
      
        public double denomination { get; set; }
        public int quantity { get; set; }
        public string delivery_option { get; set; }
        public string delivery_mode { get; set; }
        public string receiver_name { get; set; }
        public string receiver_email { get; set; }
        public string receiver_mobile { get; set; }
        public string message { get; set; }
        public string sender_name { get; set; }
        public string sender_email { get; set; }
        public string sender_mobile { get; set; }
        public string when_to_send { get; set; }

    }
}
