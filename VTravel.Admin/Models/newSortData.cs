using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class newSortData
    {
        public int itemId { get; set; }
        public int presortOrder { get; set; }
        public int cursortOrder { get; set; }
        public int pushDownValue { get; set; }
    }
}
