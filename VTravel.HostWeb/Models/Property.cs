using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.HostWeb.Models
{
    public class Property
    {
        public int id { get; set; }
        public string propertyTypeId { get; set; }
        public string destinationId { get; set; }
        public string title { get; set; }       
        public string thumbnail { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string propertyStatus { get; set; }
        public int sortOrder { get; set; }
        public string shortDescription { get; set; }
        public string longDescription { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public float displayRadius { get; set; }
        public string metaTitle { get; set; }
        public string metaKeywords { get; set; }
        public string metaDescription { get; set; }
        public int maxOccupancy { get; set; }
        public int roomCount { get; set; }
        public int bathroomCount { get; set; }
        public string phone { get; set; }
        public string email { get; set; }

        public string reserveAllowed { get; set; }
        public string reserveAlert { get; set; }
    }

    

}
