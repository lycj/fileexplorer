using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel.Composition;
using FileExplorer.UIEventHub;

namespace FileExplorer.WPF.ViewModels
{

    public class TabbedExplorerViewModel : Conductor<IScreen>.Collection.OneActive,
        ITabbedExplorerViewModel, ISupportDragHelper, IHandle<RootChangedEvent>
    {


        #region Constructors

        [ImportingConstructor]
        public TabbedExplorerViewModel(IWindowManager windowManager, IEventAggregator events)
        {
            DragHelper = new TabControlDragHelper<IExplorerViewModel>(this);

            _events = events;
            _windowManager = windowManager;

            events.Subscribe(this);
            Commands = new TabbedExplorerCommandManager(this, _events);

            Commands.SetCommandToDictionary(OnTabExplorerAttachedKey,
                UIScriptCommands.TabExplorerNewTab());

        }

        #endregion

        #region Methods

        public IExplorerViewModel OpenTab(IEntryModel model = null)
        {
            var initializer = _initializer.Clone();

            if (initializer is ExplorerInitializer)
            {
                ExplorerInitializer eInit = initializer as ExplorerInitializer;
                if (model != null)
                    eInit.Initializers.Add(ExplorerInitializers.StartupDirectory(model));
            }
            else
                if (initializer is ScriptCommandInitializer)
                {
                    ScriptCommandInitializer sInit = initializer as ScriptCommandInitializer;

                    sInit.OnViewAttached = (model != null) ?
                        ScriptCommands.Assign("{StartupPath}", model.FullPath, false,
                        UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot()) :
                        UIScriptCommands.ExplorerGotoStartupPathOrFirstRoot();

                    sInit.OnModelCreated = ScriptCommands.Assign("{TabbedExplorer}", this, false,
                            UIScriptCommands.ExplorerAssignScriptParameters("{Explorer}", "{TabbedExplorer}",
                            sInit.OnModelCreated));
                }


            ExplorerViewModel expvm = new ExplorerViewModel(_windowManager, _events) { Initializer = initializer };

            //expvm.Commands.ParameterDicConverter.AddAdditionalParameters(new ParameterDic()
            //    {
            //        {"TabbedExplorer", this }
            //    });
            expvm.DropHelper = new TabDropHelper<IExplorerViewModel>(expvm, this);

            //expvm.FileList.Commands.CommandDictionary.CloseTab =
            //    UIScriptCommands.TabExplorerCloseTab("{TabbedExplorer}", "{Explorer}");
            ////ScriptCommands.ReassignToParameter("{Explorer}", TabbedExplorer.CloseTab(this));
            expvm.FileList.Commands.CommandDictionary.OpenTab =
                ScriptCommands.Assign("{TabbedExplorer}", this, false,                
                    FileList.AssignSelectionToParameter(
                    UIScriptCommands.TabExplorerNewTab("{TabbedExplorer}", "{Parameter}", null)));
            expvm.DirectoryTree.Commands.CommandDictionary.OpenTab =
                ScriptCommands.Assign("{TabbedExplorer}", this, false,
                    DirectoryTree.AssignSelectionToParameter(
                    UIScriptCommands.TabExplorerNewTab("{TabbedExplorer}", "{Parameter}", null)));

            ActivateItem(expvm);
            checkTabs();

            return expvm;
        }

        public void CloseTab(IExplorerViewModel evm)
        {
            DeactivateItem(evm, true);
            checkTabs();
        }

        private void checkTabs()
        {
            ShowTabs = base.Items.Count() == 1 ? EnableTabsWhenOneTab : true;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            var uiEle = view as System.Windows.UIElement;
            this.Commands.RegisterCommand(uiEle, ScriptBindingScope.Application);
            Commands.ExecuteAsync(OnTabExplorerAttachedKey);
            //uiEle.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new System.Action(() => OpenTab()));

            //uiEle.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, 
            //    delegate () =>
            //    {
            //        OpenTab();
            //    });
            //OpenTab();
        }

        public int GetTabIndex(IExplorerViewModel evm)
        {
            return Items.IndexOf(evm);
        }

        public void MoveTab(int srcIdx, int targetIdx)
        {
            if (srcIdx < Items.Count())
            {
                IExplorerViewModel srcTab = Items[srcIdx] as IExplorerViewModel;
                if (srcTab != null)
                {
                    Items.RemoveAt(srcIdx);
                    Items.Insert(targetIdx, srcTab);
                }
            }
        }

        public void Handle(RootChangedEvent message)
        {
            switch (message.ChangeType)
            {
                case ChangeType.Changed:
                    _initializer.RootModels = message.AppliedRootDirectories;
                    break;
                case ChangeType.Created:
                    List<IEntryModel> rootModels = _initializer.RootModels.ToList();
                    rootModels.AddRange(message.AppliedRootDirectories);
                    _initializer.RootModels = rootModels.ToArray();
                    break;
                case ChangeType.Deleted:
                    List<IEntryModel> rootModels2 = _initializer.RootModels.ToList();
                    foreach (var d in message.AppliedRootDirectories)
                        if (rootModels2.Contains(d))
                            rootModels2.Remove(d);
                    _initializer.RootModels = rootModels2.ToArray();
                    break;
            }
        }


        #endregion

        #region Data

        //private ObservableCollection<ITabItemViewModel> _tabs;
        //private ITabItemViewModel _selectedTab;
        private IExplorerInitializer _initializer;

        #endregion

        #region Public Properties

        public string OnTabExplorerAttachedKey = "{OnTabExplorerAttached}";
        private IWindowManager _windowManager;
        private IEventAggregator _events;
        private bool _showTabs = false;
        private bool _enableTabsWhenOneTab = true;

        public IExplorerInitializer Initializer
        {
            get { return _initializer; }
            set { _initializer = value; }
        }

        public bool EnableTabsWhenOneTab { get { return _enableTabsWhenOneTab; }
            set { _enableTabsWhenOneTab = value; checkTabs(); }
        }

        public bool ShowTabs
        {
            get { return _showTabs; }
            set { _showTabs = value; NotifyOfPropertyChange(() => ShowTabs); }
        }

        //public ObservableCollection<ITabItemViewModel> Tabs { get { return _tabs; } }
        public ICommandManager Commands { get; private set; }

        public ISupportDrag DragHelper { get; private set; }
        //public ITabItemViewModel SelectedTab { get { return _selectedTab; } 
        //    set { _selectedTab = value; NotifyOfPropertyChange(() => SelectedTab); } }

        public int SelectedIndex
        {
            get { return Items.IndexOf(ActiveItem); }
            set { ActivateItem(Items[value]); NotifyOfPropertyChange(() => SelectedIndex); NotifyOfPropertyChange(() => SelectedItem); }
        }

        public IExplorerViewModel SelectedItem
        {
            get { return ActiveItem as IExplorerViewModel; }
            set { ActivateItem(value); NotifyOfPropertyChange(() => SelectedIndex); NotifyOfPropertyChange(() => SelectedItem); }
        }

        #endregion

    }
}
