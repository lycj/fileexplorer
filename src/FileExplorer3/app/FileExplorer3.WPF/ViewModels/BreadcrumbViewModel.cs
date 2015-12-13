using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.UserControls;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.Script;
using System.Windows;
using System.Threading;
using FileExplorer.WPF.Utils;
using FileExplorer.Models.Bookmark;

namespace FileExplorer.WPF.ViewModels
{
    public class BreadcrumbViewModel : ViewAttached, IBreadcrumbViewModel,
        IHandle<DirectoryChangedEvent>
    {
        #region Constructor

        public BreadcrumbViewModel(IEventAggregator events)
        {
            _events = events;

            if (events != null)
                events.Subscribe(this);

            Entries = new EntriesHelper<IBreadcrumbItemViewModel>();
            var selection = new TreeRootSelector<IBreadcrumbItemViewModel, IEntryModel>(Entries) { Comparers = new[] { PathComparer.LocalDefault } };
            selection.SelectionChanged += (o, e) =>
                {
                    BroadcastDirectoryChanged(EntryViewModel.FromEntryModel(selection.SelectedValue));
                };
            Selection = selection;
            Commands = new BreadcrumbCommandManager(this, events);

        }

        #endregion

        #region Methods

        protected void BroadcastDirectoryChanged(IEntryViewModel viewModel)
        {
            _events.PublishOnUIThread(new SelectionChangedEvent(
                this, new IEntryViewModel[] { viewModel }));
        }

        protected override void OnViewAttached(object view, object context)
        {
            if (!IsViewAttached)
            {
                _sbox = (view as UserControl).FindName("sbox") as SuggestBoxBase;
                _switch = (view as UserControl).FindName("switch") as FileExplorer.WPF.BaseControls.Switch;
                _bexp = (view as UserControl).FindName("bexp") as DropDownList;

                _bexp.AddValueChanged(ComboBox.SelectedValueProperty, (o, e) =>
                {
                    IEntryViewModel evm = _bexp.SelectedItem as IEntryViewModel;
                    if (evm != null)
                        BroadcastDirectoryChanged(evm);

                    _switch.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        _switch.IsSwitchOn = true;
                    }));
                });

                _switch.AddValueChanged(FileExplorer.WPF.BaseControls.Switch.IsSwitchOnProperty, (o, e) =>
                    {
                        if (!_switch.IsSwitchOn)
                        {
                            _sbox.Dispatcher.BeginInvoke(new System.Action(() =>
                                {
                                    Keyboard.Focus(_sbox);
                                    _sbox.Focus();
                                    _sbox.SelectAll();
                                }), System.Windows.Threading.DispatcherPriority.Background);

                        }
                    });


                var uiEle = view as System.Windows.UIElement;
                this.Commands.RegisterCommand(uiEle, ScriptBindingScope.Local);
            }
            base.OnViewAttached(view, context);
            //_sbox.AddHandler(TextBlock.LostFocusEvent, (RoutedEventHandler)((s, e) =>
            //{
            //    if (!_switch.IsSwitchOn)
            //    {
            //        _switch.Dispatcher.BeginInvoke(new System.Action(() =>
            //            {
            //                _switch.IsSwitchOn = true;
            //            }));
            //    }

            //}));
        }


        public async Task SelectAsync(IEntryModel value)
        {
            if (_sbox != null)
                _sbox.SetValue(SuggestBoxBase.TextProperty, value.FullPath);


            await Selection.LookupAsync(value,
                RecrusiveSearch<IBreadcrumbItemViewModel, IEntryModel>.LoadSubentriesIfNotLoaded,
                SetSelected<IBreadcrumbItemViewModel, IEntryModel>.WhenSelected,
                SetChildSelected<IBreadcrumbItemViewModel, IEntryModel>.ToSelectedChild);

        }

        void OnSuggestPathChanged()
        {
            if (!EnableBreadcrumb)
            {
                Task.Run(async () =>
                    {
                        foreach (var p in _profiles)
                            if (p.MatchPathPattern(SuggestedPath))
                            {
                                if (String.IsNullOrEmpty(SuggestedPath) && Entries.AllNonBindable.Count() > 0)
                                    SuggestedPath = Entries.AllNonBindable.First().EntryModel.FullPath;

                                var found = await p.ParseThenLookupAsync(SuggestedPath, CancellationToken.None);
                                if (found != null)
                                {
                                    _sbox.Dispatcher.BeginInvoke(new System.Action(() => { SelectAsync(found); }));
                                    EnableBreadcrumb = true;
                                    BroadcastDirectoryChanged(EntryViewModel.FromEntryModel(found));
                                }
                                //else not found
                            }
                    });//.Start();
            }
        }

        void setRootModels(IEntryModel[] rootModels)
        {
            _profiles = rootModels.Select(rm => rm.Profile).Distinct();
            Entries.SetEntries(UpdateMode.Update, rootModels
                .Select(r => new BreadcrumbItemViewModel(_events, this, r, null)).ToArray());
        }

        private void setProfiles(IProfile[] profiles)
        {
            _profiles = profiles;
            if (profiles != null && profiles.Length > 0)
            {
                (Selection as ITreeRootSelector<IBreadcrumbItemViewModel, IEntryModel>)
                    .Comparers = profiles.Select(p => p.HierarchyComparer);
                SuggestSources = profiles.Select(p => p.SuggestSource);
            }

            BookmarkProfile bProfile = profiles.FirstOrDefault(p => p is BookmarkProfile) as BookmarkProfile;
            if (bProfile != null)
                AddBookmarks = new AddBookmarksViewModel(bProfile, null, _events);
        }

        public void Handle(DirectoryChangedEvent message)
        {
            if (message.NewModel != null)
            {
                SelectAsync(message.NewModel);
            }
        }

        #endregion

        #region Data

        private IEnumerable<IProfile> _profiles = new List<IProfile>();
        private string _suggestedPath;
        private bool _showBreadcrumb = true;
        private IEnumerable<ISuggestSource> _suggestSources;
        private IEventAggregator _events;
        private FileExplorer.WPF.BaseControls.Switch _switch;
        private SuggestBoxBase _sbox;
        private DropDownList _bexp;
        //private bool _updatingSuggestBox = false;

        private IEntriesHelper<IBreadcrumbItemViewModel> _entries;
        private ITreeSelector<IBreadcrumbItemViewModel, IEntryModel> _selection;
        private AddBookmarksViewModel _addBookmarks;
        private bool _enableBookmark = false;

        #endregion

        #region Public Properties

        public ICommandManager Commands { get; private set; }
        public AddBookmarksViewModel AddBookmarks
        {
            get { return _addBookmarks; }
            set { _addBookmarks = value; NotifyOfPropertyChange(() => AddBookmarks); }
        }

        public IEntryModel[] RootModels { set { setRootModels(value); } }
        public IProfile[] Profiles { set { setProfiles(value); NotifyOfPropertyChange(() => IsBookmarkVisible); } }

        public ITreeSelector<IBreadcrumbItemViewModel, IEntryModel> Selection { get; set; }
        public IEntriesHelper<IBreadcrumbItemViewModel> Entries { get; set; }

        public bool EnableBreadcrumb
        {
            get { return _showBreadcrumb; }
            set { _showBreadcrumb = value; NotifyOfPropertyChange(() => EnableBreadcrumb); }
        }

        public bool IsBookmarkVisible
        {
            get { return EnableBookmark && _profiles.Any(p => p is BookmarkProfile); }
        }

        public bool EnableBookmark
        {
            get { return _enableBookmark; }
            set
            {
                _enableBookmark = value; 
                NotifyOfPropertyChange(() => EnableBookmark);
                NotifyOfPropertyChange(() => IsBookmarkVisible);
            }
        }

        public IEnumerable<ISuggestSource> SuggestSources
        {
            get { return _suggestSources; }
            set { _suggestSources = value; NotifyOfPropertyChange(() => SuggestSources); }
        }

        public string SuggestedPath
        {
            get { return _suggestedPath; }
            set
            {
                _suggestedPath = value;
                NotifyOfPropertyChange(() => SuggestedPath);
                OnSuggestPathChanged();
            }
        }

        #endregion


    }
}
