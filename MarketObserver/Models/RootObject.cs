using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MarketObserver.Models
{
    public class RootObject
    {
        public Data data { get; set; }
        public Metadata metadata { get; set; }
    }
}