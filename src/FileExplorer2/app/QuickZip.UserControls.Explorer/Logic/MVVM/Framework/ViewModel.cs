using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;

namespace QuickZip.Logic
{
    public class ViewModel : INotifyPropertyChanged
    {
        private Dispatcher _dispatcher;

        public ViewModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        private PropertyChangedEventHandler _propertyChangedEvent;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {                
                _propertyChangedEvent += value;
            }
            remove
            {                
                _propertyChangedEvent -= value;
            }
        }

        /// <summary>
        /// Utility function for use by subclasses to notify that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void SendPropertyChanged(string propertyName)
        {            
            if (_propertyChangedEvent != null)
            {
                _propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        protected void SendPropertyChanged(string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
                SendPropertyChanged(propertyName);
        }

        /// <summary>
        /// Debugging utility to make sure functions are called on the UI thread.
        /// </summary>
        [Conditional("Debug")]
        protected void VerifyCalledOnUIThread()
        {
            Debug.Assert(Dispatcher.CurrentDispatcher == this.Dispatcher, "Call must be made on UI thread.");
        }
    }
}
