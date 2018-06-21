using APICalls.Configurations;
using APICalls.Entities;
using APICalls.Entities.Interfaces;
using APICalls.Example;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using APICalls.Entities.Contracts;

namespace APIUtilCallDemo
{
    public class Program
    {
        public class ExchangeProgress : IAPIParallelProgress
        {
            public float Percentage { get; set ; }
            public int Tasks { get; set; }
            public string Url { get; set; }
        }

        public class ExchangeCallResults : IAPIParallelResult //IAPIResult
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

            public void Error<T>(T exception, APIConfiguration config, params object[] others) where T: APIException
            {
                if (exception != null)
                    Console.WriteLine($"Error Status({exception.Status})/Message ('{exception.Message}') , URL '{exception.Url}' ");
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

                    //if (_exchangeCountry > 2) config?.Cancel(); //Cancellation Token used.
                    //if (_exchangeCountry > 2) config?.CancelCurrentRepeat(); //Cancellation only for current Repeated API.

                    if (_exchangeCountry < _currencies.Length)
                        config?.UpdateObjectParams(new ExchangeCurrency { ToCurrency = _currencies[_exchangeCountry++] }, 
                                                  new StockQuoteSymbols { Symbols = "DIS,AXP" });
                    config?.InsertObjectParam(res);
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


            //Paralleism
            public object[] ParallelStart()
            {
                return new object[] { new ExchangeCurrency { ToCurrency = _currencies[_exchangeCountry++ -1] } };
            }

            public void ParallelProgress(IAPIParallelProgress progress)
            {
                if (progress == null) return;
                var exch = progress as ExchangeProgress;

                Console.Write($"...{exch?.Percentage}...");
            }            
        }


        static void Main(string[] args)
        {
            //VIA XML
            var options1 = new APIConfigurationOptions
            {
                PathOrContent = @"D:\VisualStudio 2017 Projects\GITHUB\HedgeFundYCInterview\YorkCapitalConsole\APICalls\APIProspectConfiguration.xml",
                ObjectParams = new object[] { new ExchangeCurrency(), new StockQuoteSymbols() },
                Subscriber = new ExchangeCallResults("INR", "PKR", "BDT", "LKR", "MYR", "MVR", "EUR"),
                Progessor = new ExchangeProgress()
            };

            //VIA JSON
            //var options1 = new APIConfigurationOptions
            //{
            //    PathOrContent = @"D:\VisualStudio 2017 Projects\GITHUB\HedgeFundYCInterview\YorkCapitalConsole\APICalls\APIProspectConfiguration.json",
            //    ObjectParams = new object[] { new ExchangeCurrency(), new StockQuoteSymbols() },
            //    Type = "JSON", //--> Very Important as otherwise it would try to check for XML instead of json since Default is XML
            //    Subscriber = new ExchangeCallResults("INR", "PKR", "BDT", "LKR", "MYR", "MVR", "EUR"),
            //    //NoRepeat = true,
            //    Progessor = new ExchangeProgress()
            //};

            #region - Observable
            //Observable
            var config = new APIConfiguration(options1);
            config.ExecuteApisObservable();

            //System.Threading.Thread.Sleep(5000);  //Sleep is required for caching concept to see through, otherwise all overservable will fire at once in async mode.
            //config.ExecuteApisObservable(true); //This is here to see if caching is working.
            #endregion - Observable

            #region - Parallel Processing
            //Parallel Processing 
            //Remember Repeats will not work, and also prameter will only be taken from initial ObjectParams of APICOnfigurationOptions.
            //Task.Run( () => new APIConfiguration(options1).ExecuteApisParallel());
            #endregion - Parallel Processing

            #region - Sequential
            //Synchronous/Sequential
            //var config = new APIConfiguration(options1);
            //foreach (var res in config.ExecuteApis()) ;
            //foreach (var res in config.ExecuteApis(true)) ;
            //
            //                        OR
            //
            //foreach (var prospect in config.ExecuteApis())
            //{
            //    //exchange.Reponses(prospect, config);  //If you donot provide exchange at .ExecuteApis, you need to call it from within for loop.
            //}
            //

            //Task.Run( ()=> new ParallelismTry().DownloadWebSites());
            //new ParallelismTry().DownloadWebSites();
            #endregion - Sequential

            #region - Calling a single API Without Configuration
            /*
            //Calling a single API Without Configuration.

            var api = new APICalls.APIUtil<Tokens>(new APIProspect<Tokens>
            {
                BaseUrl = "https://nycsca.proest.com/external_api/v1",
                APIUri = "login",
                Method = APICalls.Enum.APIMethod.POST,
                ParameterBody = new Dictionary<string, string>
                {
                    { "partner_key", "UtZ5UUsaj3eW-HWyxV6N" },
                    { "company_key", "JzxqMBk43yAUBRWYySC4" }
                },
                RequestHeaders = new APIRequestHeaders { ParameterContentType = "application/json" },
                //Authorization = new APIAuthorization {  }
            });

            var result = api.Call();
            Console.WriteLine($"Is Result = {result.GetType()}");
            */
            #endregion - Calling a single API Without Configuration

            #region - Regex experiment
            //int rcnt = 0;
            //var operand = "(-1 * {RealtimeCurrencyExchangeRate.ExchangeRate}^3) + (2.0/100)".Replace(" ", "");
            //var pattern1 = $"(?<=\\()[^\\)]*(?=\\))";
            //var pattern = $"[*+/-]|{System.Text.RegularExpressions.Regex.Escape("^")}";

            //System.Text.RegularExpressions.Regex.Matches(operand, pattern1).Cast<System.Text.RegularExpressions.Match>().All(m =>
            //{
            //    Console.WriteLine($"{++rcnt} --- {m.ToString()} -- {m.Groups[0].Value}");

            //    return true;
            //});

            //System.Text.RegularExpressions.Regex.Matches(operand, pattern).Cast<System.Text.RegularExpressions.Match>().All(m =>
            //{
            //    Console.WriteLine($"{++rcnt} --- {m.ToString()}");

            //    return true;
            //});
            #endregion - Regex experiment

            Console.ReadKey();
        }
    }

    
}
