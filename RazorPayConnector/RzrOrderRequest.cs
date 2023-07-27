using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RazorPayConnector
{
  
    public class RzrOrderRequest
    {
        public int amount { get; set; }
        public string currency { get; set; }
        public string receipt { get; set; }
        public int payment_capture { get; set; }
    }

}
