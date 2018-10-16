using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketObserver.Models
{
    public class USD
    {
        public string price { get; set; }
        public string volume_24h { get; set; }
        public string market_cap { get; set; }
        public string percent_change_1h { get; set; }
        public string percent_change_24h { get; set; }
        public string percent_change_7d { get; set; }
    }
}