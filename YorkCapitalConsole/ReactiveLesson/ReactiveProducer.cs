using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using PostSharp.NotifyPropertyChanged;
using PostSharp.Reflection;

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
            Observable.Merge(consumers.Select(t => t.OnAnyPropertyChanges()))
                      .Subscribe(x => ChangedSomething(x)); //Subscription - that is hooking to ChangedSomething Action.
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

    //[NotifyPropertyChanged]
    //public class ObservableCollectionEx<T> : ObservableCollection<T>
    //{
    //    public object NotifyPropertyChangedServices { get; private set; }

    //    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    //    {
    //        if (e.Action == NotifyCollectionChangedAction.Remove)
    //        {
    //            foreach (T item in e.OldItems)
    //            {
    //                ((INotifyPropertyChanged)item).PropertyChanged -= OnItemPropertyChanged;
    //            }
    //        }
    //        else if (e.Action == NotifyCollectionChangedAction.Add)
    //        {
    //            foreach (T item in e.NewItems)
    //            {
    //                ((INotifyPropertyChanged)item).PropertyChanged += OnItemPropertyChanged;
    //            }
    //        }

    //        base.OnCollectionChanged(e);
    //    }

    //    protected void OnPropertyChanged(string propertyName)
    //    {
    //        base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    //    }

    //    protected void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        //NotifyPropertyChangedServices.SignalPropertyChanged(this, "Item[]");

    //        NotifyCollectionChangedEventArgs collectionChangedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    //        base.OnCollectionChanged(collectionChangedEventArgs);
    //    }

    //USAGE
        //When you use this custom collection class, the collection will raise an event when a property of an item changes.Now you can also tell PostSharp to propagate this notification as a change of the collection property itself using [AggregateAllChanges] attribute applied to the collection property(e.g.Parameters, machines).
        //[AggregateAllChanges]
        //public ObservableCollectionEx<Parameter> Parameters { get; set; }
        //[AggregateAllChanges]
        //public ObservableCollectionEx<Machine> machines { get; set; }

    //}
}


