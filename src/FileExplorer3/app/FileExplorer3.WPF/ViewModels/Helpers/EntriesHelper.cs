using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer.WPF.Utils;
using FileExplorer.Defines;
using MetroLog;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    public class EntriesHelper<VM> : NotifyPropertyChanged, IEntriesHelper<VM>
    {
        #region Constructor

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<EntriesHelper<VM>>();

        public EntriesHelper(Func<bool, object, Task<IEnumerable<VM>>> loadSubEntryFunc)
        {
            _loadSubEntryFunc = loadSubEntryFunc;

            All = new FastObservableCollection<VM>();
            All.Add(default(VM));
        }

        public EntriesHelper(Func<bool, Task<IEnumerable<VM>>> loadSubEntryFunc)
            : this((b, __) => loadSubEntryFunc(b))
        {

        }

        public EntriesHelper(Func<Task<IEnumerable<VM>>> loadSubEntryFunc)
            : this(_ => loadSubEntryFunc())
        {
        }

        public EntriesHelper(params VM[] entries)
        {
            _isLoaded = true;
            All = new FastObservableCollection<VM>();
            _subItemList = entries;
            (All as FastObservableCollection<VM>).AddItems(entries);
            //foreach (var entry in entries)
            //    All.Add(entry);
        }

        #endregion

        #region Methods

        public async Task UnloadAsync()
        {
            _lastCancellationToken.Cancel(); //Cancel previous load.                
            using (var releaser = await _loadingLock.LockAsync())
            {
                _subItemList = new List<VM>();
                All.Clear();
                _isLoaded = false;
            }
        }

        public async Task<IEnumerable<VM>> LoadAsync(UpdateMode updateMode = UpdateMode.Replace, bool force = false, object parameter = null)
        {
            if (_loadSubEntryFunc != null) //Ignore if contructucted using entries but not entries func
            {
                _lastCancellationToken.Cancel(); //Cancel previous load.                
                using (var releaser = await _loadingLock.LockAsync())
                {
                    _lastCancellationToken = new CancellationTokenSource();
                    if (!_isLoaded || force)
                    {
                        if (_clearBeforeLoad)
                            All.Clear();

                        try
                        {
                            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                            IsLoading = true;
                            await _loadSubEntryFunc(_isLoaded, parameter).ContinueWith((prevTask, _) =>
                            {
                                IsLoaded = true;
                                IsLoading = false;
                                if (!prevTask.IsCanceled && !prevTask.IsFaulted)
                                {
                                    SetEntries(updateMode, prevTask.Result.ToArray());
                                    _lastRefreshTimeUtc = DateTime.UtcNow;
                                }
                            }, _lastCancellationToken, scheduler);
                        }
                        catch (InvalidOperationException ex) { logger.Error("Cannot obtain SynchronizationContext", ex); }


                    }
                }
            }
            return _subItemList;
        }


        private void updateEntries(params VM[] viewModels)
        {
            FastObservableCollection<VM> all = All as FastObservableCollection<VM>;
            all.SuspendCollectionChangeNotification();

            var removeItems = all.Where(vm => !viewModels.Contains(vm)).ToList();
            var addItems = viewModels.Where(vm => !all.Contains(vm)).ToList();

            foreach (var vm in removeItems)
                all.Remove(vm);

            foreach (var vm in addItems)
                all.Add(vm);

            _subItemList = all.ToArray().ToList();
            all.NotifyChanges();

            if (EntriesChanged != null)
                EntriesChanged(this, EventArgs.Empty);
        }

        public void SetEntries(UpdateMode updateMode = UpdateMode.Replace, params VM[] viewModels)
        {
           switch (updateMode)
           {
               case UpdateMode.Update: updateEntries(viewModels); break;
               case UpdateMode.Replace: setEntries(viewModels); break;
               default: throw new NotSupportedException("UpdateMode");
           }
        }

        private void setEntries(params VM[] viewModels)
        {
            _subItemList = viewModels.ToList();
            FastObservableCollection<VM> all = All as FastObservableCollection<VM>;
            all.SuspendCollectionChangeNotification();
            all.Clear();
            all.NotifyChanges();
            all.AddItems(viewModels);
            all.NotifyChanges();

            if (EntriesChanged != null)
                EntriesChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Data

        private CancellationTokenSource _lastCancellationToken = new CancellationTokenSource();
        private bool _clearBeforeLoad = false;
        private readonly AsyncLock _loadingLock = new AsyncLock();
        //private bool _isLoading = false;
        private bool _isLoaded = false;
        private bool _isExpanded = false;
        private bool _isLoading = false;
        private IEnumerable<VM> _subItemList = new List<VM>();
        protected Func<bool, object, Task<IEnumerable<VM>>> _loadSubEntryFunc;
        private ObservableCollection<VM> _subItems;
        private DateTime _lastRefreshTimeUtc = DateTime.MinValue;

        #endregion

        #region Public Properties

        public bool ClearBeforeLoad
        {
            get { return _clearBeforeLoad; }
            set { _clearBeforeLoad = value; }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value && !_isExpanded) LoadAsync();
                _isExpanded = value;
                NotifyOfPropertyChanged(() => IsExpanded);
            }
        }

        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; NotifyOfPropertyChanged(() => IsLoaded); }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; NotifyOfPropertyChanged(() => IsLoading); }
        }

        public DateTime LastRefreshTimeUtc { get { return _lastRefreshTimeUtc; } }

        public event EventHandler EntriesChanged;


        public IEnumerable<VM> AllNonBindable { get { return _subItemList; } }

        public ObservableCollection<VM> All { get { return _subItems; } private set { _subItems = value; } }

        public AsyncLock LoadingLock { get { return _loadingLock; } }

        #endregion








    }

}
