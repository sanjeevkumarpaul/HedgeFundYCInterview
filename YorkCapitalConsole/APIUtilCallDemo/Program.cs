using APICalls.Configurations;
using APICalls.Entities;
using APICalls.Example;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;

namespace APIUtilCallDemo
{
    public class Program
    {

        public class ExchangeCallResults : IAPIResult
        {
            private int _exchangeCountry = 1;
            private string[] _currencies;

            public ExchangeCallResults(params string[] currencyCodes)
            {
                _currencies = currencyCodes;
            }

            #region ^IAPIResult Methods, where the APIConfig will Push the results
            public void Final(IAPIProspect result)
            {
                Console.WriteLine();
                Console.WriteLine("API Call is completely done.");
            }

            public void Post(IEnumerable<IAPIProspect> results)
            {
                ///TO DO:
            }

            public void Reponses(IAPIProspect resultProspect, APIConfiguration config)
            {
                ExchangeRate(resultProspect, config);
                StockQuotes(resultProspect);
            }
            #endregion ~IAPIResult Methods, where the APIConfig will Push the results

            /// <summary>
            /// To be used in Sequential calls as it requires user to interntact with results. 
            /// We can code whatever we need to here.
            /// </summary>
            /// <param name="config"></param>
            public void Result(APIConfiguration config)
            {
                Final(config.ProspectResults.Last());

                //...
            }

            //Respones in private methods.
            internal void ExchangeRate(IAPIProspect result, APIConfiguration config)
            {
                if (result is RealtimeCurrencyExchange)
                {
                    var res = (result as RealtimeCurrencyExchange).RealtimeCurrencyExchangeRate;

                    Console.WriteLine("Exchange Currency Information Received...");
                    Console.WriteLine($@"     >> From {res?.FromCurrencyName}({res?.FromCurrencyCode}) to {res?.ToCurrencyName}({res?.ToCurrencyCode}) => Exchange Rate: {res?.ExchangeRate}");

                    if (res?.ExchangeRate.ToDouble() < 10.00) config.Cancell(); //Cancellation Token used.

                    if (_exchangeCountry < _currencies.Length)
                        config.UpdateObjectParams(new ExchangeCurrency { ToCurrency = _currencies[_exchangeCountry++] }, 
                                                  new StockQuoteSymbols { Symbols = "DIS,AXP" });                    
                }
            }

            internal void StockQuotes(IAPIProspect result)
            {
                if (result is StockQuoteMaster)
                {
                    var res = (result as StockQuoteMaster).StockQuotes;

                    Console.WriteLine("Stock Quotes Information Received...");
                    Console.WriteLine($@"     >> Price Volume: {res?.Sum(r => r.Price?.ToDouble())} ");
                }
            }
        }


        static void Main(string[] args)
        {
            //VIA XML
            //var options = new APIConfigurationOptions
            //{
            //    Path = @"D:\VisualStudio 2017 Projects\GITHUB\HedgeFundYCInterview\YorkCapitalConsole\APICalls\APIProspectConfiguration.xml",
            //    ObjectParams = new object[] { new ExchangeCurrency(), new StockQuoteSymbols() }
            //};

            //VIA JSON
            var options1 = new APIConfigurationOptions
            {
                PathOrContent = @"D:\VisualStudio 2017 Projects\GITHUB\HedgeFundYCInterview\YorkCapitalConsole\APICalls\APIProspectConfiguration.json",
                ObjectParams = new object[] { new ExchangeCurrency(), new StockQuoteSymbols() },
                Type = "JSON" //--> Very Important as otherwise it would try to check for XML instead of json since Default is XML
            };

            new APIConfiguration(options1)
            .ExecuteApisObservable(new ExchangeCallResults( "INR", "BDT", "PKR", "LKR", "MYR", "MVR", "EUR" ));


            //Synchronouse.
            //foreach(var prospect in new APIXmlConfiguration(filename, new ExchangeCurrency()).ExecuteApis())
            //{
            //    new ExchangeCallResults().Reponses( prospect, null );
            //}


            Console.ReadKey();
        }
    }
}
