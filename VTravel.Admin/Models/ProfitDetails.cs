namespace VTravel.Admin.Models
{
    public class ProfitDetails
    {
        public int id { get; set; }
        public int propertyId { get; set; }
        public string room { get; set; }
        public int channelId { get; set; }
        public string channel { get; set; }
        public string mode { get; set; }
        public decimal price { get; set; }
        public decimal percentage { get; set; }
        public string include_food { get; set; }
        public string include_extra { get; set; }
        public string taxless_amount { get; set; }
    }
}
