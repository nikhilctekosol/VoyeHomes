﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class RatePlans
    {
        public int id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string propertyid { get; set; }
        public string active { get; set; }
    }
}
