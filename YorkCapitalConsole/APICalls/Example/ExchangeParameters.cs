using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICalls.Example
{
    public class ExchangeCurrency
    {
        public string FromCurrency { get; set; } = "USD";
        public string ToCurrency { get; set; } = "INR";
    }

    public class StockQuoteSymbols
    {
        public string Symbols { get; set; } = "MSFT,GOOG";
    }
}
