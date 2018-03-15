using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveLesson
{
    public class ReactiveProducer<T> where T : ICustomePropertyChangedNotification, IDisposable
    {
        //private readonly Subject<T> _paymentSubject = new Subject<T>();
        //private IObservable<T> _Producer;        
        
        public ReactiveProducer(List<T> consumers)
        {
            StartObserving(consumers);
        }

        //Copy Constructor.
        public ReactiveProducer(T consumer) : this(new List<T>() { consumer }) { } 
              

        private void StartObserving(List<T> consumers)
        {
            Observable.Merge(consumers.Select(t => t.OnAnyPropertyChanges())).Subscribe(x => ChangedSomething(x)); //Subscription - that is hooking to ChangeSomething Action.
        }

        private void ChangedSomething(T consumer)
        {
            consumer.Print();
        }        
    }

    public static class NotificationExtensions
    {
        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <returns>Returns an observable sequence of the value of the source when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<T> OnAnyPropertyChanges<T>(this T source)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                                handler => handler.Invoke,
                                h => source.PropertyChanged += h,
                                h => source.PropertyChanged -= h)
                            .Select(_ => source);
        }
    }
}
