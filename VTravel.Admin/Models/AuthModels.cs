using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class LoginModel
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class RequestPassModel
    {
        public string email { get; set; }

    }

    public class ResetPassModel
    {
        public string tokenKey { get; set; }
        public string password { get; set; }
    }

    
}
