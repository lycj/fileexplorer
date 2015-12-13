using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace QuickZip.Logic
{
    public class DataModel : INotifyPropertyChanged
    {
        public DataModel()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            this.State = ModelState.Invalid;
        }

        /// <summary>
        /// Possible states for a DataModel.
        /// </summary>
        public enum ModelState
        {
            Invalid,    // The model is in an invalid state
            Fetching,   // The model is being fetched
            Valid       // The model has fetched its data
        }

        /// <summary>
        /// Is the model active?
        /// </summary>
        public bool IsActive
        {
            get
            {
                VerifyCalledOnUIThread();
                return _isActive;
            }

            private set
            {
                VerifyCalledOnUIThread();
                if (value != _isActive)
                {
                    _isActive = value;
                    SendPropertyChanged("IsActive");
                }
            }
        }

        /// <summary>
        /// Activate the model.
        /// </summary>
        public void Activate()
        {
            VerifyCalledOnUIThread();

            if (!_isActive)
            {
                this.IsActive = true;
                OnActivated();
            }
        }


        /// <summary>
        /// Override to provide behavior on activate.
        /// </summary>
        protected virtual void OnActivated()
        {
        }

        /// <summary>
        /// Deactivate the model.
        /// </summary>
        public void Deactivate()
        {
            VerifyCalledOnUIThread();

            if (_isActive)
            {
                this.IsActive = false;
                OnDeactivated();
            }
        }

        /// <summary>
        /// Override to provide behavior on deactivate.
        /// </summary>
        protected virtual void OnDeactivated()
        {
        }

        //protected virtual void OnDisableLoaded()
        //{
        //}

        /// <summary>
        /// PropertyChanged event for INotifyPropertyChanged implementation.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                VerifyCalledOnUIThread();
                _propertyChangedEvent += value;
            }
            remove
            {
                VerifyCalledOnUIThread();
                _propertyChangedEvent -= value;
            }
        }        

        public event ActionRequestedEventHandler ActionRequested
        {
            add
            {
                VerifyCalledOnUIThread();
                _actionRequestedEvent += value;
            }
            remove
            {
                VerifyCalledOnUIThread();
                _actionRequestedEvent -= value;
            }
        }

        /// <summary>
        /// Gets or sets current state of the model.
        /// </summary>
        public ModelState State
        {
            get
            {
                VerifyCalledOnUIThread();
                return _state;
            }

            set
            {
                VerifyCalledOnUIThread();
                if (value != _state)
                {
                    _state = value;
                    SendPropertyChanged("State");
                    SendPropertyChanged("IsLoading");
                }
            }
        }

        /// <summary>
        /// Whether Subentry is fetching.
        /// </summary>
        public bool IsLoading { 
            get { VerifyCalledOnUIThread(); 
                return State == ModelState.Fetching; 
            } }

        /// <summary>
        /// Whether Subentry completed loading.
        /// </summary>
        public bool IsLoaded
        {
            get { VerifyCalledOnUIThread();
                return State == ModelState.Valid;
            }
        }

        /// <summary>
        /// The Dispatcher associated with the model.
        /// </summary>
        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
        }

        /// <summary>
        /// Utility function for use by subclasses to notify that a property has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void SendPropertyChanged(string propertyName)
        {
            VerifyCalledOnUIThread();
            if (_propertyChangedEvent != null)
            {
                _propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void DispatcherSendPropertyChanged(string propertyName)
        {
            VerifyCalledOnUIThread();
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                    new ThreadStart(delegate
                    { SendPropertyChanged(propertyName); }));

        }

        protected void SendPropertyChanged(string[] propertyNames)
        {
            foreach (string propertyName in propertyNames)
                SendPropertyChanged(propertyName);
        }

        protected void SendActionRequested(string actionName)
        {
            VerifyCalledOnUIThread();
            if (_actionRequestedEvent != null)
            {
                _actionRequestedEvent(this, new ActionRequestedEventArgs(actionName));
            }
        }


        /// <summary>
        /// Debugging utility to make sure functions are called on the UI thread.
        /// </summary>
        [Conditional("Debug")]
        protected void VerifyCalledOnUIThread()
        {
            Debug.Assert(Dispatcher.CurrentDispatcher == this.Dispatcher, "Call must be made on UI thread.");
        }

        private ModelState _state;
        private Dispatcher _dispatcher;
        private PropertyChangedEventHandler _propertyChangedEvent;
        private ActionRequestedEventHandler _actionRequestedEvent;
        private bool _isActive;        
    }

}
