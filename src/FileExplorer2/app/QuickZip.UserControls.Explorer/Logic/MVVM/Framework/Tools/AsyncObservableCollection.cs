///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows;
using System.Collections.Specialized;
using System.Threading;

namespace QuickZip.MVVM
{



    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        #region Data
        private AsyncBackgroundTaskManager<T> _bgWorker;
        bool _aborted = false;

        #endregion

        #region Constructor
        public AsyncObservableCollection(
            DispatcherPriority priority,
            IEnumerable<T> taskFunc,
            Action beginAction,
            Action<T> newItemAction,
            Action<T> removeItemAction,
            Action<IList<T>, Exception> completeAction)
            : base()
        {
            if (newItemAction == null) newItemAction = (item) => { };
            if (removeItemAction == null) removeItemAction = (item) => { };
            if (beginAction == null) beginAction = () => { };
            if (completeAction == null) completeAction = (itemList, ex) => { };

            _bgWorker = new AsyncBackgroundTaskManager<T>(priority, taskFunc,
                () => { beginAction(); },
                (item) => { this.Add(item); newItemAction(item); },
                (item) => { this.Remove(item); removeItemAction(item); },
                (list, ex) => { completeAction(list, ex); });
        }

        public AsyncObservableCollection(
            IEnumerable<T> taskFunc,
            Action beginAction,
            Action<T> newItemAction,
            Action<T> removeItemAction,
            Action<IList<T>, Exception> completeAction)
            : this(DispatcherPriority.Normal, taskFunc, beginAction, newItemAction, removeItemAction, completeAction)
        {

        }

        public AsyncObservableCollection(IEnumerable<T> taskFunc,
          Action beginAction, Action<IList<T>, Exception> completeAction)
            : this(taskFunc, beginAction, null, null, completeAction)
        {

        }

        public AsyncObservableCollection(IEnumerable<T> taskFunc,
           Action<IList<T>, Exception> completeAction)
            : this(taskFunc, null, null, null, completeAction)
        {

        }

        public AsyncObservableCollection(IEnumerable<T> taskFunc)
            : this(taskFunc, null)
        {

        }

        public AsyncObservableCollection()
            : this(null, null)
        {

        }
        #endregion

        #region Data

        Action _beginAction = null;

        #endregion

        #region Public Properties

        public object Arguments { get; set; }
        public bool IsWorking { get { return _bgWorker.IsWorking; } }

        #endregion

        #region Methods
        public void Load(bool refresh, Action taskAfterCompleted)
        {
            if (_bgWorker.IsWorking)
            {
                return;
            }

            if (_aborted || refresh)
            {
                //Exception - "This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread."
                //if (_aborted)                    
                //    this.Clear();

                _bgWorker.InvokeInDispatcher(() =>
                    {
                        this.Clear();
                    });
                _bgWorker.ResetOriginalList();
            }
            _aborted = false;

            if (taskAfterCompleted != null)
            {
                EventHandler<EventArgs> handler = null;
                handler = (EventHandler<EventArgs>)delegate(object sender, EventArgs args)
                {
                    taskAfterCompleted();
                    _bgWorker.BackgroundTaskCompleted -= handler;
                };

                _bgWorker.BackgroundTaskCompleted += handler;
            }


            _bgWorker.RunBackgroundTask();
        }

        public void Load(bool refresh)
        {
            Load(refresh, null);
        }

        public void Load()
        {
            Load(false);
        }

        public void Abort()
        {
            if (_bgWorker.IsWorking)
            {
                _aborted = true;
                _bgWorker.AbortBackgroundTask();
            }
        }

        //private Dictionary<NotifyCollectionChangedEventHandler, Thread>
        //    handlersThread = new Dictionary<NotifyCollectionChangedEventHandler, Thread>();

        //public override event NotifyCollectionChangedEventHandler CollectionChanged
        //{
        //    add
        //    {
        //        //base.CollectionChanged += value;
        //        this.handlersThread.Add(value, Thread.CurrentThread); //We know that Current thread must be UI thread
        //        //of that specific control other while expection
        //        //will be thrown at registering time.
        //    }
        //    remove
        //    {
        //        //base.CollectionChanged -= value;
        //        this.handlersThread.Remove(value);
        //    }
        //}

        //protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        //{
        //    //base.OnCollectionChanged(e);
        //    foreach (NotifyCollectionChangedEventHandler handler in this.handlersThread.Keys)
        //    {
        //        Dispatcher dispatcher = Dispatcher.FromThread(this.handlersThread[handler]);
        //        dispatcher.Invoke(DispatcherPriority.Send, handler, this, e);
        //    }
        //}


        #endregion

    }
}
