using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VTravel.CustomerWeb.Models
{
    public class PageViewModel
    {
       public SitePage sitePage { get; set; }

        public List<SuperHost> hostList { get; set; }
    }

}
