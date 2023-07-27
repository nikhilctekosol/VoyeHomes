using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.Admin.Models
{
    public class EnqData
    {
        public int id { get; set; }
        public int cust_id { get; set; }
        public string cust_name { get; set; }
        public string cust_email { get; set; }
        public string cust_phone { get; set; }
        public int property_id { get; set; }
        public string property_title { get; set; }
        public DateTime created_on { get; set; }
        public int created_by { get; set; }
        public DateTime checkin_date { get; set; }
        public DateTime checkout_date { get; set; }       
        public int adults_count { get; set; }
        public int children_count { get; set; }
        public int booking_channel_id { get; set; }
        public string booking_channel_name { get; set; }
        public int assigned_user { get; set; }
        public string assigned_user_name { get; set; }
        public string assigned_name_of_user { get; set; }
        public long reservation_id { get; set; }
        public string enquiry_status { get; set; }
        
        

    }

    public class RecordCount
    {
        public long total_count { get; set; }
        public int rows_count { get; set; }
        public int page_no { get; set; }


    }


}
