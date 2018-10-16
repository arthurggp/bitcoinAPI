using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketObserver.Models
{
    public class Data
    {
        public int id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string website_slug { get; set; }
        public int rank { get; set; }
        public string circulating_supply { get; set; }
        public string total_supply { get; set; }
        public string max_supply { get; set; }
        public Quotes quotes { get; set; }
        public int last_updated { get; set; }
    }
}