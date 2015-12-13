using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using FileExplorer.Script;

namespace FileExplorer.WPF.ViewModels
{
    public class SidebarViewModel : ViewAware, ISidebarViewModel,
        IHandle<SelectionChangedEvent>, IHandle<ListCompletedEvent>
    {
        #region Constructors

        public SidebarViewModel(IEventAggregator events)
        {
            Commands = new SidebarCommandManager(this, events);
            Metadata = new MetadataHelperViewModel(m => m.IsVisibleInSidebar);

        

            events.Subscribe(this);
        }

        #endregion

        #region Methods

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            var uiEle = view as System.Windows.UIElement;
            Commands.RegisterCommand(uiEle, ScriptBindingScope.Local);

            CollectionView collectionView = (CollectionView)CollectionViewSource.GetDefaultView(Metadata.All);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("MetadataModel.Category"));
        }

        public void Handle(ListCompletedEvent message)
        {
            Metadata.LoadAsync(UpdateMode.Replace, true, message.Sender as IFileListViewModel);
        }

        public void Handle(SelectionChangedEvent message)
        {
            Metadata.LoadAsync(UpdateMode.Replace, true, message.Sender as IFileListViewModel);
        }

        #endregion

        #region Data

        private bool _isVisible = false;
        private IMetadataHelperViewModel _metadata;

        #endregion

        #region Public Properties

        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; NotifyOfPropertyChange(() => IsVisible);}}
        public ICommandManager Commands { get; private set; }
        public IMetadataHelperViewModel Metadata { get { return _metadata; } private set { _metadata = value; } }

        #endregion



      
    }
      
}
