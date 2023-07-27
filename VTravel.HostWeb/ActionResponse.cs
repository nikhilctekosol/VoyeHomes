using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.HostWeb
{
    public class ActionResponse
    {
        public string ActionStatus { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
