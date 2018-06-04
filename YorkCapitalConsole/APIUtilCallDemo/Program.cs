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
            public void Final(IAPIProspect result)
            {
                Console.WriteLine();
                Console.WriteLine("API Call is completely done.");
            }

            public void Post(IEnumerable<IAPIProspect> results)
            {
                
            }

            public void Reponses(IAPIProspect resultProspect, APIConfiguration config)
            {
                ExchangeRate(resultProspect);
                StockQuotes(resultProspect);
            }


            //Respones in private methods.
            internal void ExchangeRate(IAPIProspect result)
            {
                if (result is RealtimeCurrencyExchange)
                {
                    var res = (result as RealtimeCurrencyExchange).RealtimeCurrencyExchangeRate;

                    Console.WriteLine("Exchange Currency Information Received...");
                    Console.WriteLine($@"     >> From {res?.FromCurrencyName}({res?.FromCurrencyCode}) to {res?.ToCurrencyName}({res?.ToCurrencyCode}) => Exchange Rate: {res?.ExchangeRate}");
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
            .ExecuteApisObservable(new ExchangeCallResults());


            //Synchronouse.
            //foreach(var prospect in new APIXmlConfiguration(filename, new ExchangeCurrency()).ExecuteApis())
            //{
            //    new ExchangeCallResults().Reponses( prospect, null );
            //}


            Console.ReadKey();
        }
    }
}
