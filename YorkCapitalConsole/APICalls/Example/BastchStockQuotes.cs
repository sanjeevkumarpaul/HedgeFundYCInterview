using APICalls.Entities;
using APICalls.Entities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Example
{
    public partial class StockQuoteMaster : IAPIProspect
    {
        [JsonProperty("Meta Data")]
        public MetaData MetaData { get; set; }

        [JsonProperty("Stock Quotes")]
        public StockQuote[] StockQuotes { get; set; }
    }

    public partial class MetaData
    {
        [JsonProperty("1. Information")]
        public string Information { get; set; }

        [JsonProperty("2. Notes")]
        public string Notes { get; set; }

        [JsonProperty("3. Time Zone")]
        public string TimeZone { get; set; }
    }

    public partial class StockQuote
    {
        [JsonProperty("1. symbol")]
        public string Symbol { get; set; }

        [JsonProperty("2. price")]
        public string Price { get; set; }

        [JsonProperty("3. volume")]
        public string Volume { get; set; }

        [JsonProperty("4. timestamp")]
        public DateTime Timestamp { get; set; }
    }


}
