using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ExchangeWatcher.Models
{
    public class TRADESATOSHI
    {
        [JsonProperty(PropertyName = "market")]
        public string market { get; set; }
        [JsonProperty(PropertyName = "high")]
        public string high { get; set; }
        [JsonProperty(PropertyName = "low")]
        public string low { get; set; }
        [JsonProperty(PropertyName = "volume")]
        public string volume { get; set; }
        [JsonProperty(PropertyName = "bid")]
        public string bid { get; set; }
        [JsonProperty(PropertyName = "ask")]
        public string ask { get; set; }

    }
}
