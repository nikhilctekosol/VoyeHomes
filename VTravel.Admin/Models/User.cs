using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class User
    {
        public string phone { get; set; }
        public string idToken { get; set; }
        public string uid { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool tc { get; set; }
        public bool nl{ get; set; }
        public string userToken { get; set; }
        public string id { get; set; }
        public float walletAmount { get; set; }

    }
}
