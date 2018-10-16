using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeWatcher.Models
{
    public class TRADEOGRE
    {
        [JsonProperty(PropertyName = "success")]
        public bool success { get; set; }
        [JsonProperty(PropertyName = "initialprice")]
        public string initialprice { get; set; }
        [JsonProperty(PropertyName = "price")]
        public string price { get; set; }
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

        public string moeda { get; set; }
    }
}
