using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class Country
    {
        public int id { get; set; }
        public string name { get; set; }
        public string nicename { get; set; }
        public int phonecode { get; set; }
        public string iso { get; set; }

    }
}
