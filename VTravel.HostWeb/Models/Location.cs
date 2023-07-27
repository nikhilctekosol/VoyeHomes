using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.HostWeb.Models
{
    public class Country
    {
        public int id { get; set; }
        public string countryCode { get; set; }
        public string countryName{ get; set; }

    }

    public class State
    {
        public int id { get; set; }
        public string countryCode { get; set; }
        public string stateCode { get; set; }
        public string stateName { get; set; }

    }
    public class City
    {
        public int id { get; set; }
        public string countryCode { get; set; }
        public string stateCode { get; set; }
        public string cityCode { get; set; }
        public string cityName { get; set; }

    }
}
