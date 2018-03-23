using PostSharp.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveLesson
{
    //[NotifyPropertyChanged]       
    public class ReactiveOrder : PropertyChangedEvents
    {
        private DateTime? _paidDate;

        public void MarkPaid(DateTime paidDate)
        {
            PayDate = paidDate;            
        }
       
        public DateTime? PayDate { get { return _paidDate; } private set { _paidDate = OnPropertyChanged(_paidDate, value) as DateTime?; } }


        public override void Print()
        {
            Console.WriteLine("Change Print");
            Console.WriteLine($">> {this.ChangedPropertyName} >> {this.ChangedPropertyValueOld} >> {this.ChangedPropertyValue} >>");
            Console.WriteLine();
        }
    }
}
