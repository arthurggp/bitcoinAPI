using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeWatcher.Models
{
    public class CREX24
    {
        [JsonProperty(PropertyName = "instrument")]
        public string instrument { get; set; }
        [JsonProperty(PropertyName = "last")]
        public string last { get; set; }
        [JsonProperty(PropertyName = "percentChange")]
        public string percentChange { get; set; }
        [JsonProperty(PropertyName = "high")]
        public string high { get; set; }
        [JsonProperty(PropertyName = "low")]
        public string low { get; set; }
        [JsonProperty(PropertyName = "quoteVolume")]
        public string quoteVolume { get; set; }
        [JsonProperty(PropertyName = "bid")]
        public string bid { get; set; }
        [JsonProperty(PropertyName = "ask")]
        public string ask { get; set; }
        [JsonProperty(PropertyName = "timestamp")]
        public string timestamp { get; set; }
    }
}
