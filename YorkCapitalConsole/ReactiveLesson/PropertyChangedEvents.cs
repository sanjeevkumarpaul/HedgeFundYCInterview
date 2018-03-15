using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveLesson
{
    public abstract class PropertyChangedEvents : ICustomePropertyChangedNotification, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        
        public void Dispose()
        {
            //
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract void Print();    

    }
}
