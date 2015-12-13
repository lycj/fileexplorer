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
using System.Collections.ObjectModel;

namespace QuickZip.MVVM
{
    public class CollectionBackgroundTaskManager<T> : AsyncBackgroundTaskManager<T>
    {
        #region Data

        Collection<T> _collection;
        bool _aborted = false;
        #endregion

        #region Public Properties

        public Collection<T> Collection { get { return _collection; } }

        #endregion

        #region Constructor
        public CollectionBackgroundTaskManager(Collection<T> collection, DispatcherPriority priority,
           Func<IEnumerable<T>> taskFunc, Action<IList<T>, Exception> completeAction)
            : base(priority, taskFunc, null,
            (item) => collection.Add(item),
            (item) => collection.Remove(item), 
            completeAction)
        {
            _collection = collection;
        }

        public CollectionBackgroundTaskManager(Collection<T> collection, DispatcherPriority priority,
          IEnumerable<T> taskEnumerable, Action<IList<T>, Exception> completeAction)
            : this(collection, priority, () => { return taskEnumerable; }, completeAction)
        {
            _collection = collection;
        }

        #endregion

        #region Methods

        public override void RunBackgroundTask()
        {
            if (_aborted)
            {
                _originalList.Clear();
                _originalList.AddRange(Collection);
            }
            _aborted = false;
            base.RunBackgroundTask();
        }

        public override void AbortBackgroundTask()
        {
            _aborted = true;
            base.AbortBackgroundTask();
        }

        #endregion

    }
}
