using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveLesson
{
    public class ProducerOrder
    {
        private DateTime? _paidDate;

        private readonly Subject<ProducerOrder> _paidSubj = new Subject<ProducerOrder>();
        public IObservable<ProducerOrder> Paid { get { return _paidSubj.AsObservable(); } }

        public void MarkPaid(DateTime paidDate)
        {
            _paidDate = paidDate;
            _paidSubj.OnNext(this); // Raise PAID event
        }

        public DateTime? PayDate { get { return _paidDate; } }
    }
}
