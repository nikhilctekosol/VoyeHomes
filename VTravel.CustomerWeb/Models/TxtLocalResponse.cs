using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class TxtLocalResponse
    {
        public int balance { get; set; }
        public long batch_id { get; set; }
        public int cost { get; set; }
        public int num_messages { get; set; }
        public Message message { get; set; }
        public Message1[] messages { get; set; }
        public string status { get; set; }
    }

    public class Message
    {
        public int num_parts { get; set; }
        public string sender { get; set; }
        public string content { get; set; }
    }

    public class Message1
    {
        public string id { get; set; }
        public long recipient { get; set; }
    }

}
