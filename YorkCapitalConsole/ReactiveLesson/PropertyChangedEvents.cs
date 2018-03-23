using PostSharp.NotifyPropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveLesson
{
    //[NotifyPropertyChanged(AttributeExclude = true)]
    public abstract class PropertyChangedEvents : ICustomePropertyChangedNotification , IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ChangedPropertyName { get; set; }
        public dynamic ChangedPropertyValue { get; set; }
        public dynamic ChangedPropertyValueOld { get; set; }

        public virtual object OnPropertyChanged(dynamic oldValue, dynamic value, [CallerMemberName]string changePropertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                if (oldValue != value)
                {
                    ChangedPropertyValueOld = oldValue;
                    ChangedPropertyValue = value;
                    ChangedPropertyName = changePropertyName;
                    handler(this, new PropertyChangedEventArgs(changePropertyName));
                }
            }

            return value;
        }

        public abstract void Print();

        public void Dispose()
        {
            //
        }

    }
}
