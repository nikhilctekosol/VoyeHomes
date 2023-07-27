using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class ReservationViewModel
    {
       public Property property { get; set; }
       public string terms { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string banner_url { get; set; }


       
        public List<Country> countryList { get; set; }
    }

}
