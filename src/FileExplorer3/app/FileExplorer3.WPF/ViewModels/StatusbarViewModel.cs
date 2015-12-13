using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Caliburn.Micro;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels.Helpers;
using FileExplorer.WPF.Defines;

namespace FileExplorer.WPF.ViewModels
{
#if !WINRT
    [Export(typeof(StatusbarViewModel))]
#endif
    public class StatusbarViewModel : PropertyChangedBase, IStatusbarViewModel,
        IHandle<SelectionChangedEvent>, IHandle<ListCompletedEvent>
    {
        #region Cosntructor

        public StatusbarViewModel(IEventAggregator events)
        {
            _events = events;            
            _displayItems = new BindableCollection<IEntryViewModel>();
            Metadata = new MetadataHelperViewModel(m => m.IsVisibleInStatusbar);

            events.Subscribe(this);
        }

        #endregion

        #region Methods


        private void OnIsExpandedChanged(bool isExpanded)
        {
            Debug.WriteLine(isExpanded);
        }

        public static string NoneSelected = "{0}";
        public static string OneSelected = "{0}";
        public static string ManySelected = "{0} items selected";

        private void updateDisplayItemsAndCaption(IFileListViewModel flvm)
        {
            SelectionCount = flvm.Selection.SelectedItems.Count();

            Metadata.LoadAsync(UpdateMode.Replace, true, flvm);
            DisplayItems.Clear();
            switch (SelectionCount)
            {
                case 0:
                    //Caption = String.Format(NoneSelected, flvm.CurrentDirectory.EntryModel.Label);

                    DisplayItems.Add(EntryViewModel.FromEntryModel(flvm.CurrentDirectory));
                    break;
                case 1:
                    //Caption = String.Format(OneSelected, flvm.SelectedItems.First().EntryModel.Label);
                    DisplayItems.Add(flvm.Selection.SelectedItems.First());
                    break;
                default:
                    //Caption = String.Format(ManySelected, flvm.SelectedItems.Count.ToString());
                    DisplayItems.AddRange(flvm.Selection.SelectedItems.Take(5));
                    break;
            }
        }

        public void Handle(SelectionChangedEvent message)
        {
            if (message.Sender is IFileListViewModel)
            {
                updateDisplayItemsAndCaption(message.Sender as IFileListViewModel);


                //if (SelectionCount == 1)
                //    Caption = message.SelectedViewModels.First()
            }
        }

        public void Handle(ListCompletedEvent message)
        {
            if (message.Sender is IFileListViewModel)
            {
                updateDisplayItemsAndCaption(message.Sender as IFileListViewModel);
                
            }
        }

        #endregion

        #region Data
        
        bool _isExpanded = false;
        int _selectionCount;
        string _caption;
        IObservableCollection<IEntryViewModel> _displayItems;
        string _selectedViewMode = "Icon";
        private IEventAggregator _events;
        private IMetadataHelperViewModel _metadata;
        

        #endregion

        #region Public Properties

   
        public bool IsExpanded { get { return _isExpanded; } set { if (_isExpanded != value) { _isExpanded = value;
        NotifyOfPropertyChange(() => IsExpanded);
            OnIsExpandedChanged(_isExpanded); } } }

        public int SelectionCount { get { return _selectionCount; } set { _selectionCount = value; NotifyOfPropertyChange(() => SelectionCount); } }
        public IObservableCollection<IEntryViewModel> DisplayItems { get { return _displayItems; } }
        public IMetadataHelperViewModel Metadata { get { return _metadata; } private set { _metadata = value; } }


        public string Caption { get { return _caption; } set { _caption = value; NotifyOfPropertyChange(() => Caption); } }

        #endregion



      
    }
}
