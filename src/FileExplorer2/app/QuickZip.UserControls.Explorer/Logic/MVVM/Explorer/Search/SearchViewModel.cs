using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;
using System.Collections.ObjectModel;
using QuickZip.MVVM;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using QuickZip.UserControls.MVVM.Model;

namespace QuickZip.UserControls.MVVM
{
    public class SearchViewModel<FI, DI, FSI> : RootModelBase
        where FI : FSI
        where DI : FSI
    {
        #region Constructor
        public SearchViewModel(Profile<FI, DI, FSI> profile)
        {
            _profile = profile;
            setupProperties();
        }
        #endregion

        #region Methods

        public IEnumerable<Suggestion> GetSuggestions()
        {
            string parseName = SearchParseName;
            IEnumerator<Suggestion> enumerator = _profile.Lookup(parseName).GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }


        void setupProperties()
        {

            Suggestions = new ObservableCollection<Suggestion>();



            //Suggestions = new AsyncObservableCollection<Suggestion>(
            // GetSuggestions(),
            //  () => { },
            //  (item) => { HasSuggestions = true; },
            //  (Item) => { },
            //  (result, ex) =>
            //  {
            //      HasSuggestions = (ex == null) && (result.Count > 0);
            //  }

            //  );

        }


        public void UpdateSearch()
        {
            //if (!String.IsNullOrEmpty(SearchParseName))
            //{
            //    //_suggestions.Abort();
            //   _suggestions.Load();
            //}

            //_suggestions.Add(new Suggestion("AAA"));

            if (_searchThread != null && _searchThread.ThreadState == System.Threading.ThreadState.Running)
                _searchThread.Abort();

            _searchThread = new Thread(new ThreadStart(delegate
            {
                try
                {
                    string workingParseName = SearchParseName;
                    IEnumerator<Suggestion> suggestions = _profile.Lookup(workingParseName).GetEnumerator();

                    if (Thread.CurrentThread.ThreadState == System.Threading.ThreadState.Running)
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
                        new ThreadStart(delegate
                        {
                            _suggestions.Clear();
                            while (suggestions.MoveNext() && workingParseName == SearchParseName)
                            {
                                //if (!(String.Equals(suggestions.Current.Header, workingParseName, StringComparison.CurrentCultureIgnoreCase)))
                                _suggestions.Add(suggestions.Current);
                            }
                            HasSuggestions = _suggestions.Count > 0;
                        }));

                }
                catch { }



            }));

            _searchThread.Start();

        }

        private bool changeDirectory(string parseName)
        {
            DirectoryChangedEventArgs args = null;
            FSI entry = _profile.ConstructEntry(parseName);
            if (entry is DI)
            {
                DirectoryModel<FI, DI, FSI> newDirModel = _profile.ConstructDirectoryModel((DI)entry);
                args = new DirectoryChangedEventArgs<FI, DI, FSI>(newDirModel);

            }
            else
            {
                args = new DirectoryChangedEventArgs(parseName);
            }

            DirectoryChanged(this, args);
            return args.ChangeAllowed;
        }

        #endregion

        #region Data

        private Thread _searchThread;
        private Profile<FI, DI, FSI> _profile;
        private string _searchText, _confirmedParseName;//, _workingText;
        private EntryModel<FI, DI, FSI> _selectedModel;
        //private bool _isVisible = false;
        private bool _hasSuggestions = false;
        private ObservableCollection<Suggestion> _suggestions;


        #endregion

        #region Public Properties

        public EventHandler<DirectoryChangedEventArgs> DirectoryChanged = (o, args) => { };
        public ObservableCollection<Suggestion> Suggestions
        {
            get { return _suggestions; }
            private set { _suggestions = value; NotifyPropertyChanged("Suggestions"); }
        }

        public bool HasSuggestions
        {
            get { return _hasSuggestions; }
            set { _hasSuggestions = value; NotifyPropertyChanged("HasSuggestions"); }
        }

        public string SearchParseName
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                { _searchText = value; NotifyPropertyChanged("SearchParseName"); UpdateSearch(); }
            }
        }


        public EntryModel<FI, DI, FSI> SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                _selectedModel = value;
                HasSuggestions = false;
                ConfirmedParseName = _selectedModel.ParseName;                
                NotifyPropertyChanged("SelectedModel");
            }
        }

        public string ConfirmedParseName
        {
            get { return _confirmedParseName; }
            set
            {
                _confirmedParseName = value;
                NotifyPropertyChanged("ConfirmedParseName");
                NotifyPropertyChanged("UIConfirmedParseName");
                HasSuggestions = false;
            }
        }

        public string UIConfirmedParseName
        {
            get { return _confirmedParseName; }
            set
            {
                if (_confirmedParseName != value)
                {
                    if (changeDirectory(value))
                    {
                        _confirmedParseName = value;
                        HasSuggestions = false;
                        NotifyPropertyChanged("ConfirmedParseName");
                        NotifyPropertyChanged("UIConfirmedParseName");
                    }
                }
            }
        }

        #endregion

    }
}
