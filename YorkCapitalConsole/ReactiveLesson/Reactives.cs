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

            //#region ^Subscriber Event Observable with Notifications
            //var order1 = new ReactiveOrder();
            //new ReactiveProducer<ReactiveOrder>(order1); //This class is for subscription.
            //order1.MarkPaid(DateTime.Now);
            //order1.MarkPaid(DateTime.Now.AddDays(10));
            //#endregion ~Subscriber Event Observable with Notifications

            DifferentThread();

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

            IDisposable subscription = ob.Subscribe(i => Console.WriteLine(i));  //subscription 1
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

        private static void DifferentThread()
        {
            var nums = new int[] { 5, 10, 15,0, 20, 25 };

            Console.WriteLine($"Main Thread: {Thread.CurrentThread.ManagedThreadId}");

            IObservable<int> onums = nums.Select(n => n / 5)
                                         .ToObservable()
                                         .ObserveOn(NewThreadScheduler.Default)
                                         .SubscribeOn(NewThreadScheduler.Default)
                                         .Finally(() => { Console.WriteLine($"I am always there, even error occurs -  on {Thread.CurrentThread.ManagedThreadId}"); });

            onums.Subscribe<int>((val) => { Console.WriteLine($"{val} on {Thread.CurrentThread.ManagedThreadId}"); }, 
                                 (e) => { Console.WriteLine($"{e.Message} on {Thread.CurrentThread.ManagedThreadId}"); }, 
                                 () => { Console.WriteLine($"Completed  on {Thread.CurrentThread.ManagedThreadId}"); });
        }

        /// <summary>
        /// Observabble with parallel Task library hand in hand
        /// Explanation:
        /// If you use TASK, it has always to return a single value that is Task<IList<Row>> and if you are reading database
        /// you need to reall all rows and process all rows inmemory in a loop or so on.
        /// If you use IObservable, you are actually reading streams of columns and each time a row is read your Subscription 
        /// can process them individually, which is efficient and scalable for in-memory.
        /// To wrap up, I would suggest to use Task whenever you can, 
        /// and use IObservable if you get into a more complex situation where you need an entire collection to be turned asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        public static Task<IList<T>> BufferAllAsync<T>(this IObservable<T> observable)
        {
            List<T> result = new List<T>();
            object gate = new object();
            TaskCompletionSource<IList<T>> finalTask = new TaskCompletionSource<IList<T>>();

            observable.Subscribe(
                value =>
                {
                    lock (gate)
                    {
                        result.Add(value);
                    }
                },
                exception => finalTask.TrySetException(exception),
                () => finalTask.SetResult(result.AsReadOnly())
            );

            return finalTask.Task;
        }
    }
}


/*
 * 
 * A foreach statement for IObservable
 * 
 * 
 Fortunately, it is not hard to recreate. The secret is to transform the IObservable<T> into a IAsyncEnumerable<T>, 
 then use await to enumerate asynchronously over it. IAsyncEnumerable<T> is part of the interactive side of the Reactive Extensions. 
 You will need to add a reference to System.Interactive.Async, which you can find in the Ix_Experimental-Async NuGet package. IAsyncEnumerable<T> 
 is basically a replica of the IEnumerable<T> interface, the only difference is that MoveNext returns Task<bool> on the IAsyncEnumerator instead of 
 bool on the normal IEnumerator. This makes perfect sense, as in the asynchronous case, MoveNext should only complete when the source IObservable has a 
 new value to provide, when the observable completes, or if it throws.

Let’s assume we have an observable that we want to iterate over:

IObservable<string> io = Observable.Interval(TimeSpan.FromSeconds(1)).Select(v => "Tick " + v);
EnumerateAsync(io);

We can implement an async foreach by copying the expanded form of foreach, and adding the await keyword when calling MoveNext. The result is this:

private static async Task EnumerateAsync(IObservable<string> observable)
{
    using (var enumerator = observable.ToAsyncEnumerable().GetEnumerator())
    {
        while (await enumerator.MoveNext())
        {
            await Task.Yield();

            var item = enumerator.Current;

            // Your code here
            Console.WriteLine(value);
        }
    }
}
The Task.Yield statement is necessary because of a bug in the implementation of ToAsyncEnumerable. Using that construct, you can process observables in a pull fashion. 
That means you can wrap the loop in a try/catch statement, use break from the loop, and the disposal of the subscription is automatically handled. That 
also guarantees that each iteration of the loop will be done one after the other, in a sequential fashion, making your code thread-safe by design.

 */
