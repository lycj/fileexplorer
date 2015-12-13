///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace QuickZip.MVVM
{
    public class AsyncBackgroundTaskManager<T>
    {
        #region Data

        private Dispatcher _dispatcher = null;
        private Thread _workingThread = null;
        private DispatcherPriority _priority = DispatcherPriority.Normal;
        protected List<T> _originalList = new List<T>();
        private Func<IEnumerable<T>> _taskFunc = null;
        private Action _beginAction = null;
        private Action<T> _newItemAction = null;
        private Action<T> _removeItemAction = null;
        private Action<IList<T>, Exception> _completeAction = null;
        #endregion

        public AsyncBackgroundTaskManager(DispatcherPriority priority,
           Func<IEnumerable<T>> taskFunc, Action beginAction, Action<T> newItemAction, Action<IList<T>, Exception> completeAction)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _priority = priority;
            _taskFunc = taskFunc;

            if (beginAction == null) beginAction = () => { };
            if (newItemAction == null) newItemAction = (item) => { };
            if (completeAction == null) completeAction = (itemList, ex) => { };

            _beginAction = () =>
                {
                    InvokeInDispatcher(() => beginAction());
                };
            _newItemAction = (item) =>
                {
                    InvokeInDispatcher(() => newItemAction(item));
                };
            _completeAction = (retList, ex) =>
            {
                InvokeInDispatcher(() => completeAction(retList, ex));
            };
        }

        public AsyncBackgroundTaskManager(DispatcherPriority priority,
            Func<IEnumerable<T>> taskFunc, Action beginAction, Action<T> newItemAction, Action<T> removeItemAction,
            Action<IList<T>, Exception> completeAction)
            : this(priority, taskFunc, beginAction, newItemAction, completeAction)
        {
            if (newItemAction == null) newItemAction = (item) => { };
            if (removeItemAction == null) removeItemAction = (item) => { };
            if (completeAction == null) completeAction = (itemList, ex) => { };

            _newItemAction = (item) =>
            {
                if (!_originalList.Contains(item))
                    InvokeInDispatcher(() => newItemAction(item));
            };

            _removeItemAction = (item) =>
            {
                InvokeInDispatcher(() => removeItemAction(item));
            };
            _completeAction = (retList, ex) =>
            {
                if (ex == null)
                {
                    foreach (T item in filterRemovedItems(_originalList, retList))
                        _removeItemAction(item);
                    _originalList = retList == null ? new List<T>() : new List<T>(retList);
                    InvokeInDispatcher(() => completeAction(retList, ex));
                }
            };
        }

        public AsyncBackgroundTaskManager(DispatcherPriority priority,
           IEnumerable<T> taskEnumerable, Action beginAction, Action<T> newItemAction, Action<T> removeItemAction,
           Action<IList<T>, Exception> completeAction)
            : this(priority, () => { return taskEnumerable; }, beginAction, newItemAction, removeItemAction, completeAction)
        {

        }



        public AsyncBackgroundTaskManager(DispatcherPriority priority,
            IEnumerable<T> taskEnumerable, Action beginAction, Action<T> newItemAction, Action<IList<T>, Exception> completeAction)
            : this(priority, () => { return taskEnumerable; }, beginAction, newItemAction, completeAction)
        {
        }

        #region Methods

        public void InvokeInDispatcher(Action work)
        {
            //try
            //{
                _dispatcher.BeginInvoke(_priority, new ThreadStart(() =>
                {
                    work();
                }));
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine("Error");
            //}
        }

        static IList<T> filterRemovedItems(IList<T> origList, IList<T> newList)
        {
            List<T> retVal = new List<T>();

            if (origList != null)
            {
                foreach (T item in origList)
                    if (!newList.Contains<T>(item))
                        retVal.Add(item);
            }

            return retVal;
        }

        public void ResetOriginalList()
        {
            _originalList = new List<T>();
        }

        public virtual void RunBackgroundTask()
        {
            if (_taskFunc != null && !IsWorking)
            {
                _workingThread = new Thread(new ThreadStart(() =>
                    {
                        try
                        {
                            _beginAction();
                            List<T> retList = new List<T>();
                            IEnumerator<T> enumerator = _taskFunc().GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                _newItemAction(enumerator.Current);
                                retList.Add(enumerator.Current);
                            }

                            _completeAction(retList, null);
                            if (BackgroundTaskCompleted != null)
                                InvokeInDispatcher(() => BackgroundTaskCompleted(this, new EventArgs()));
                        }
                        catch (ThreadAbortException ex)
                        {
                            if (_completeAction != null)
                                _completeAction(null, ex);
                        }
                        catch (Exception ex)
                        {
                            if (_completeAction != null)
                                _completeAction(null, ex);
                        }
                        finally
                        {
                            Thread.Sleep(1000);
                        }
                    }));
                _workingThread.Start();
            }

        }

        public virtual void AbortBackgroundTask()
        {
            if (IsWorking)
                _workingThread.Abort();
        }

        #endregion

        #region Public Properties

        public EventHandler<EventArgs> BackgroundTaskCompleted;

        public AutoResetEvent CompletionWaitHandle { get; set; }

        public bool IsWorking
        {
            get
            {
                return (_workingThread != null &&
                    (_workingThread.ThreadState == System.Threading.ThreadState.Running ||
                    _workingThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin
                    ));
            }
        }

        #endregion
    }
}
