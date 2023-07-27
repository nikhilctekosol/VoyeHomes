using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class HomeViewModel
    {
       public List<Tag> tagList { get; set; }
        public List<HeroBanner> bannerList { get; set; }
        public List<Destination> destinationList { get; set; }
        public List<Feature> featureList { get; set; }
    }

}
