using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Cloudinary1
{
    public class CdyRequest
    {
        public string file { get; set; }
        public string api_key { get; set; }
        public string timestamp { get; set; }
        public string signature { get; set; }
        public string public_id { get; set; }
    }
}
