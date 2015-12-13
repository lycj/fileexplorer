using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using Cinch;

namespace QuickZip.UserControls.Logic.Tools.IconExtractor
{
    public class LazyLoader<T> : INotifyPropertyChanged
    {
        private bool _loaded = false;
        private T _value = default(T);
        private Func<T> _loadFunc, _loadFunc2;
        private BackgroundTaskManager<Func<T>, T> bgWorker;

        public T Value
        {
            get
            {
                if (!_loaded)
                    StartLoad();
                return _value;
            }
            set { _value = value; NotifyPropertyChanged("Value"); }
        }

        public static implicit operator T(LazyLoader<T> value) { return value.Value; }

        public LazyLoader(Func<T> loadFunc, Func<T> loadFunc2)
        {
            _loadFunc = loadFunc;
            _loadFunc2 = loadFunc2;

            bgWorker = new BackgroundTaskManager<Func<T>, T>(
                (func) =>
                {
                    return func();
                }, 
                    (result) =>
                    {
                        Value = result;
                    });
        }

        public LazyLoader(Func<T> loadFunc)
        {
            _loadFunc = loadFunc;
        }

        public void StartLoad()
        {
            _loaded = true;
            //T value = default(T);

            bgWorker.WorkerArgument = _loadFunc;
            bgWorker.RunBackgroundTask();

            //new Thread(new ThreadStart(() => { Value = _loadFunc(); })).Start();


            //BackgroundWorker bgWorker = new BackgroundWorker();
            //bgWorker.DoWork += (DoWorkEventHandler)
            //    ((o, e) =>
            //    {
            //        value = _loadFunc();
            //    });

            //bgWorker.RunWorkerCompleted += (RunWorkerCompletedEventHandler)
            //    ((o, e) =>
            //    {
            //        Value = value;

            //        //T value2 = default(T);
            //        //if (_loadFunc2 != null)
            //        //{
            //        //    BackgroundWorker bgWorker2 = new BackgroundWorker();
            //        //    bgWorker2.DoWork += (DoWorkEventHandler)
            //        //        ((o2, e2) =>
            //        //        {
            //        //            value2 = _loadFunc2();
            //        //        });

            //        //    bgWorker2.RunWorkerCompleted += (RunWorkerCompletedEventHandler)
            //        //        ((o2, e2) =>
            //        //        {
            //        //            Value = value2;
            //        //        });

            //        //    bgWorker2.RunWorkerAsync();
            //        //}
            //    });

            //bgWorker.RunWorkerAsync();
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }

        #endregion

}
