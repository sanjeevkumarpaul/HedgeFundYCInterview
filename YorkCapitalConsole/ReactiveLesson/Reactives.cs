using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Reactive.Threading.Tasks;

namespace ReactiveLesson
{
    public class Reactives
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Lets do Reactive Programing...");


            //ParallelExecutionTest(); //This is async , and will return when await is found and will continue to next method.
            //Background();

            //CancellationTest();
            //Task.Run(async () => 
            //        {
            //            //await WaitForFirstResultAndReturnResultWithTimeOut();
            //            var res = await WaitForAllResultsAndReturnCombinedResult();
            //            Console.WriteLine(res);
            //        }); 


            //#region ^Subscriber Event Observable
            //var order = new ProducerOrder();
            //order.Paid.Subscribe(_ => Console.WriteLine($"Paid on : ${order.PayDate}")); // Subscribe (Kind of an Event Handler, handle is attached now. )
            //order.MarkPaid(DateTime.Now);
            //#endregion ~Subscriber Event Observable

            #region ^Subscriber Event Observable with Notifications
            var order1 = new ReactiveOrder();
            new ReactiveProducer<ReactiveOrder>(order1); //This class is for subscription.
            order1.MarkPaid(DateTime.Now);
            #endregion ~Subscriber Event Observable with Notifications

            Console.ReadKey();
        }

        #region ^Beginners
        private static void Background()
        {
            var o = Observable.Start(() =>
            {
                //This starts on a background thread.
                Console.WriteLine("From background thread. Does not block main thread.");
                Console.WriteLine("Calculating...");
                Thread.Sleep(3000);
                Console.WriteLine("Background work completed.");
            })
            .Finally(() => Console.WriteLine("Main thread completed."));

            Console.WriteLine("\r\n\t In Main Thread...\r\n");
            o.Wait();   // Wait for completion of background operation.
        }

        private static async void ParallelExecutionTest()
        {
            var o = Observable.CombineLatest(
                Observable.Start(() => { Console.WriteLine("Executing 1st on Thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(5000); return "Result A"; }),
                Observable.Start(() => { Console.WriteLine("Executing 2nd on Thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(5000); return "Result B"; }),
                Observable.Start(() => { Console.WriteLine("Executing 3rd on Thread: {0}", Thread.CurrentThread.ManagedThreadId); Thread.Sleep(5000); return "Result C"; })
            ).Finally(() => Console.WriteLine("Done!"));

            foreach (string r in await o.FirstAsync())
                Console.WriteLine(r);
        }
        #endregion ~Beginners

        #region ^Cancellation Test
        private static void CancellationTest()
        {
            IObservable<int> ob = Observable.Create<int>(o =>
            {
                var cancel = new CancellationDisposable(); // internally creates a new CancellationTokenSource
                NewThreadScheduler.Default.Schedule(() =>
                {
                    int i = 0;
                    for (; ; )
                    {
                        Thread.Sleep(200);  // here we do the long lasting background operation
                        if (!cancel.Token.IsCancellationRequested)    // check cancel token periodically
                            o.OnNext(i++);    //----> PUBLISHING TO SUbSCRIBER.
                        else
                        {
                            Console.WriteLine("Aborting because cancel event was signaled!");
                            o.OnCompleted(); // will not make it to the subscriber
                            return;
                        }
                    }
                });

                return cancel;
            });

            IDisposable subscription = ob.Subscribe(i => Console.WriteLine(i) );  //subscription 1
            IDisposable subscription1 = ob.Subscribe(SubscriberAction);           //subscription 2 with different implementation,

            Console.WriteLine("Press any key to cancel");
            Console.ReadKey();

            //Both subscription has to be cancelled.
            subscription.Dispose();
            subscription1.Dispose();
            Console.WriteLine("Press any key to quit");           
        }

        private static void SubscriberAction(int value)
        {
            Console.WriteLine($"Method Subscription: {value}");
        }
        #endregion ~Cancellation Test

        #region ^Task Combining 
        private static async Task<string> WaitForFirstResultAndReturnResultWithTimeOut()
        {
            Task<string> task1 = GetHelloString();
            Task<string> task2 = GetWorldString();

            Console.WriteLine(DateTime.Now);

            return await await Task
                .WhenAny(task1, task2)
                .ToObservable()
                //.Delay(TimeSpan.FromMilliseconds(10)).Finally(() => Console.WriteLine($"Time Now: {DateTime.Now}"))
                .Timeout(TimeSpan.FromMilliseconds(1000)).Finally(() => Console.WriteLine($"Timed Out - {DateTime.Now}"))
                .FirstAsync();
            
        }

        private static async Task<string> WaitForAllResultsAndReturnCombinedResult()
        {
            IObservable<string> observable1 = GetHelloString().ToObservable();
            IObservable<string> observable2 = GetWorldString().ToObservable();

            return await observable1.Zip(observable2, (x1, x2) => string.Join(" ", x1, x2));
        }

        private static Task<string> GetHelloString()
        {
            Thread.Sleep(500);
            return Task.FromResult<string>("Get Hello String");
        }

        private static Task<string> GetWorldString()
        {
            Thread.Sleep(500);
            return Task.FromResult<string>("Get World String");
        }
        #endregion ~Task Combining 

    }
}
