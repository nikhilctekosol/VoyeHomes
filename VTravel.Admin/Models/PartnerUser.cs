﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class PartnerUser
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string userRole { get; set; }
        public string nameOfUser { get; set; }
        public int[] userProperties { get; set; }
        
    }
  
    
}
