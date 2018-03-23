using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveLesson
{
    public interface ICustomePropertyChangedNotification : INotifyPropertyChanged
    {
        string ChangedPropertyName { get; set; } 
        dynamic ChangedPropertyValue { get; set; }
        dynamic ChangedPropertyValueOld { get; set; }
        void Print();
    }
}
