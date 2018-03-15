using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveLesson
{
    public class ReactiveOrder : PropertyChangedEvents
    {
        private DateTime? _paidDate;

        public void MarkPaid(DateTime paidDate)
        {
            PayDate = paidDate;            
        }

        public DateTime? PayDate { get { return _paidDate; } private set { _paidDate = value; OnPropertyChanged("PayDate"); } }


        public override void Print()
        {
            Console.WriteLine("Change Print");
        }

    }
}
