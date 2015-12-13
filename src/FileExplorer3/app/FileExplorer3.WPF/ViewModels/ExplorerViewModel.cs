using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Caliburn.Micro;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Utils;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.ViewModels
{
    [Export(typeof(IExplorerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ExplorerViewModel : Screen, IExplorerViewModel,
        IHandle<DirectoryChangedEvent>,
        IHandle<EntryChangedEvent>,
        IHandle<RootChangedEvent>,
        IHandle<BroadcastEvent>,
        ISupportDragHelper, ISupportDropHelper
    {



        #region Cosntructor


        public ExplorerViewModel(IExplorerInitializer initializer)
        {
            _events = initializer.Events;
            _rootModels = initializer.RootModels;
            _windowManager = initializer.WindowManager;
            _initializer = initializer;

            WindowTitleMask = "{0}";
            DisplayName = "";
            //Toolbar = new ToolbarViewModel(events);
            Breadcrumb = new BreadcrumbViewModel(_internalEvents);
            Statusbar = new StatusbarViewModel(_internalEvents);
            Sidebar = new SidebarViewModel(_internalEvents);
            FileList = new FileListViewModel(_windowManager, _internalEvents, Sidebar);
            DirectoryTree = new DirectoryTreeViewModel(_windowManager, _internalEvents);
            Navigation = new NavigationViewModel(_internalEvents);

            DragHelper = NullSupportDrag.Instance;
            DropHelper = NullSupportDrop.Instance;

            Commands = new ExplorerCommandManager(this, 
                FileList, DirectoryTree, Navigation, Breadcrumb);
            setRootModels(_rootModels);

            if (_events != null)
                _events.Subscribe(this);
            _internalEvents.Subscribe(this);

        }

        [ImportingConstructor]
        public ExplorerViewModel(IWindowManager windowManager, IEventAggregator events)
        {
            _events = events;
            _rootModels = new IEntryModel[] { };
            _windowManager = windowManager;
            _initializer = NullExplorerInitializer.Instance;

            WindowTitleMask = "{0}";
            DisplayName = "";
            //Toolbar = new ToolbarViewModel(events);
            Breadcrumb = new BreadcrumbViewModel(_internalEvents);
            Statusbar = new StatusbarViewModel(_internalEvents);
            Sidebar = new SidebarViewModel(_internalEvents);
            FileList = new FileListViewModel(_windowManager, _internalEvents, Sidebar);
            DirectoryTree = new DirectoryTreeViewModel(_windowManager, _internalEvents);
            Navigation = new NavigationViewModel(_internalEvents);

            DragHelper = NullSupportDrag.Instance;
            DropHelper = NullSupportDrop.Instance;

            Commands = new ExplorerCommandManager(this, FileList, DirectoryTree, Navigation, Breadcrumb);
            //setRootModels(_rootModels);

            if (_events != null)
                _events.Subscribe(this);
            _internalEvents.Subscribe(this);
        }


        #endregion

        #region Methods

        protected override void OnInitialize()
        {

            base.OnInitialize();
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);

            if (!_attachedView)
            {
                var uiEle = view as System.Windows.FrameworkElement;

                this.Commands.RegisterCommand(uiEle, ScriptBindingScope.Explorer);
                _initializer.InitializeViewAttachedAsync(this);
                _attachedView = true;
            }
        }



        public async Task GoAsync(IEntryModel entryModel)
        {
            entryModel = entryModel ?? RootModels.FirstOrDefault();
            if (entryModel != null)
            {
                await Task.WhenAll(
                    FileList.SetCurrentDirectoryAsync(entryModel),
                    DirectoryTree.SelectAsync(entryModel),
                    Breadcrumb.Selection.AsRoot().SelectAsync(entryModel));

                NotifyOfPropertyChange(() => CurrentDirectory);
                NotifyOfPropertyChange(() => CurrentDirectory);
            }

        }

        public async Task GoAsync(string gotoPath)
        {
            foreach (var evm in _rootModels)
            {
                var model = await evm.Profile.ParseAsync(gotoPath);
                if (model != null)
                {
                    await GoAsync(model);
                    return;
                }
            }
        }


        public void ChangeView(string viewMode)
        {
            FileList.Parameters.ViewMode = viewMode;
        }

        private void setRootModels(IEntryModel[] rootModels)
        {
            if (rootModels == null || rootModels.Length == 0)
                return;
            _rootModels = rootModels;
            
            RootProfiles = rootModels.Select(m => m.Profile).Distinct().ToArray();

            Breadcrumb.Profiles = RootProfiles; Breadcrumb.RootModels = rootModels;
            DirectoryTree.Profiles = RootProfiles; DirectoryTree.RootModels = rootModels;
            FileList.Profiles = RootProfiles;
        }

        void onEntryChanged(object sender, EntryChangedEvent e)
        {            
            this.Events.PublishOnUIThreadAsync(e);
        }

        public async Task BroascastAsync(EntryChangedEvent message)
        {
            try
            {
                string[] paths = message.ParseNames
                    .Where(p => p != null)
                    .Select(p => p.Contains('\\') ?
                        PathHelper.Disk.GetDirectoryName(p) : PathHelper.Web.GetDirectoryName(p))
                        .Distinct().ToArray();

                foreach (var path in paths)
                {
                    IEntryModel affectedParentEntry = await _rootProfiles.ParseAsync(path);
                    if (affectedParentEntry != null)
                    {
                        await DirectoryTree.Selection.AsRoot().BroascastAsync(affectedParentEntry);
                        await Breadcrumb.Selection.AsRoot().BroascastAsync(affectedParentEntry);
                        if (FileList.CurrentDirectory.Equals(affectedParentEntry))
                            await FileList.ProcessedEntries.EntriesHelper.LoadAsync(UpdateMode.Replace, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


        }

        public void Handle(DirectoryChangedEvent message)
        {
            this.DisplayName = String.Format(WindowTitleMask, message.NewModel.Label);
            _currentDirectoryViewModel = EntryViewModel.FromEntryModel(message.NewModel);
            NotifyOfPropertyChange(() => CurrentDirectory);            
        }

        public void Handle(EntryChangedEvent message)
        {
            if (Application.Current != null)
                Application.Current.Dispatcher.InvokeAsync(() => BroascastAsync(message));
        }

        public void Handle(BroadcastEvent message)
        {
            if (message.EventToBroadcast != null)
            {
                if (message.EventToBroadcast is ExplorerEvent)
                    (message.EventToBroadcast as ExplorerEvent).Sender = this;
                _events.PublishOnUIThread(message.EventToBroadcast);
            }
        }

        public void Handle(RootChangedEvent message)
        {
            Queue<IScriptCommand> cmds = new Queue<IScriptCommand>();

            cmds.Enqueue(Explorer.ChangeRoot(message.ChangeType, message.AppliedRootDirectories));

            if (message.Sender != null && message.Sender != this)
                cmds.Enqueue(UIScriptCommands.ExplorerGoToValue(CurrentDirectory.EntryModel));
            else
                switch (message.ChangeType)
                {
                    case ChangeType.Changed:
                        cmds.Enqueue(UIScriptCommands.ExplorerGoToValue(message.AppliedRootDirectories.First()));
                        break;
                    case ChangeType.Deleted:
                        cmds.Enqueue(UIScriptCommands.ExplorerGoToValue(RootModels.FirstOrDefault()));
                        break;
                }

            Commands.ExecuteAsync(cmds.ToArray());
        }

        #region FileNameFilter

        IEnumerable<FileNameFilter> getFilters(string filterStr)
        {
            string[] filterSplit = filterStr.Split('|');
            List<FileNameFilter> ff = new List<FileNameFilter>();
            for (int i = 0; i < filterSplit.Count() / 2; i++)
                ff.Add(new FileNameFilter(filterSplit[i * 2], filterSplit[(i * 2) + 1]));

            return ff;
        }

        private void setFilter(string value)
        {
            _selectedFilter = value;

            FileList.ProcessedEntries.SetFilters(
                ColumnFilter.CreateNew<IEntryModel>(value, "FullPath",
                e => e.IsDirectory ||
                    PathFE.MatchFileMasks(
                    e.Profile.Path.GetFileName(e.FullPath), value)));
            NotifyOfPropertyChange(() => SelectedFilter);
        }


        Task<IEnumerable<FileNameFilter>> loadFiltersTask()
        {
            return Task.Run(() =>
            {
                string[] filterSplit = _filterStr.Split('|');
                List<FileNameFilter> ff = new List<FileNameFilter>();
                for (int i = 0; i < filterSplit.Count() / 2; i++)
                    ff.Add(new FileNameFilter(filterSplit[i * 2], filterSplit[(i * 2) + 1]));

                return ff as IEnumerable<FileNameFilter>;
            });


        }

        #endregion

        private void setInitializer(IExplorerInitializer value)
        {
            _initializer = value;
            if (_initializer != null)
            {
                _initializer.InitializeModelCreatedAsync(this);
                if (_attachedView)
                    _initializer.InitializeViewAttachedAsync(this);
            }
        }

        public override string ToString()
        {
            return String.Format("ExplorerVM {0}", _id);
        }

        #endregion

        #region Data

        private static int count = 0;
        private int _id = count++;
        private bool _attachedView = false;
        private IEntryModel[] _rootModels;
        private IEventAggregator _events;
        private IEventAggregator _internalEvents = new EventAggregator();
        protected IWindowManager _windowManager = new WindowManager();
        private IProfile[] _rootProfiles = new IProfile[] { };
        private IExplorerInitializer _initializer;
        private IExplorerParameters _parameters = new ExplorerParameters();

        private IEntryViewModel _currentDirectoryViewModel;
        private bool _isDragging = false;
        private string _filterStr;
        private string _selectedFilter = null;
        private bool _isSelected = false;
        private bool _isSelecting = false;

        #endregion

        #region Public Properties


        public IExplorerInitializer Initializer
        {
            get { return _initializer; }
            set
            {
                setInitializer(value);
            }
        }



        public IExplorerParameters Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                NotifyOfPropertyChange(() => Parameters);
            }
        }

        public string WindowTitleMask { get; set; }
        public IEntryViewModel CurrentDirectory { get { return _currentDirectoryViewModel; } }

        public IEntryModel[] RootModels { get { return _rootModels; } set { setRootModels(value); } }

        public IProfile[] RootProfiles
        {
            get { return _rootProfiles; }
            set
            {
                if (_rootProfiles != null)
                    foreach (var p in _rootProfiles)
                        p.OnEntryChanged -= onEntryChanged;

                _rootProfiles = value;

                if (_rootProfiles != null)
                    foreach (var p in _rootProfiles)
                        p.OnEntryChanged += onEntryChanged;
            }
        }

        public ICommandManager Commands { get; private set; }

        public IBreadcrumbViewModel Breadcrumb { get; private set; }
        public IDirectoryTreeViewModel DirectoryTree { get; private set; }
        public IFileListViewModel FileList { get; private set; }
        public ISidebarViewModel Sidebar { get; private set; }
        public IStatusbarViewModel Statusbar { get; private set; }
        public INavigationViewModel Navigation { get; private set; }
        //public IToolbarViewModel Toolbar { get; private set; }

        public IEntriesHelper<FileNameFilter> Filters { get; set; }
        public string FilterStr
        {
            get { return _filterStr; }
            set
            {
                _filterStr = value;
                if (_filterStr != null)
                {
                    Filters = new EntriesHelper<FileNameFilter>(loadFiltersTask);
                    var filters = getFilters(_filterStr).ToArray();
                    Filters.SetEntries(UpdateMode.Replace, filters);
                    if (filters.Length > 0)
                        SelectedFilter = filters.First().Filter;
                }
            }
        }
        public string SelectedFilter { get { return _selectedFilter; } set { setFilter(value); } }

        public ISupportDrag DragHelper { get; set; }
        public ISupportDrop DropHelper { get; set; }

        #region For TabExplorer
        public bool IsDragging
        {
            get { return _isDragging; }
            set
            {
                _isDragging = value;
                NotifyOfPropertyChange(() => IsDragging);
                NotifyOfPropertyChange(() => HeaderOpacity);
            }
        }




        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public bool IsSelecting
        {
            get { return _isSelecting; }
            set
            {
                _isSelecting = value;
                NotifyOfPropertyChange(() => IsSelecting);
            }
        }
        #endregion
        public float HeaderOpacity { get { return _isDragging ? 0.5f : 1f; } }

        public IEventAggregator Events
        {
            get { return _events; }
            set { _events = value; }
        }

        public IEventAggregator InternalEvents
        {
            get { return _internalEvents; }
            set { _internalEvents = value; }
        }

        public IWindowManager WindowManager
        {
            get { return _windowManager; }
            set { _windowManager = value; }
        }


        #endregion
    }
}
