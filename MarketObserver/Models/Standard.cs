using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExchangeWatcher.Models
{
    public class Standard
    {
        [JsonProperty(PropertyName = "mercado")]
        public string mercado { get; set; }

        [JsonProperty(PropertyName = "exchange")]
        public string exchange { get; set; }

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
