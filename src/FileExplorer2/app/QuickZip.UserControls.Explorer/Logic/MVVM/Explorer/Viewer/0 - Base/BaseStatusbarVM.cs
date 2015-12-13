using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.MVVM;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM.ViewModel;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace QuickZip.UserControls.MVVM
{
    public abstract class BaseStatusbarViewModel : ExplorerRootModelBase
    {

        #region Constructor

        public BaseStatusbarViewModel()
        {
            //setupItemList();

            //StatusItemList = new ObservableCollection<MetadataViewModel>(from mm in innerGetMetadataModel() select mm.ToViewModel());
            //StatusItemList = new AsyncObservableCollection<MetadataViewModel>(
            // from mm in innerGetMetadataModel() select mm.ToViewModel()
            // );
            //StatusItemList.Load();
        }

        #endregion

        #region Methods


        public virtual void UpdateStatusbar()
        {
            StatusItemList = new ObservableCollection<MetadataViewModel>(from mm in innerGetMetadataModel() select mm.ToViewModel());
        }        


        private void setupItemList()
        {
            //_statusItemList = new AsyncObservableCollection<MetadataViewModel>();                
        }

        protected virtual IEnumerable<GenericMetadataModel> getMetadataModel()
        {
            yield break;
        }

        protected virtual IEnumerable<GenericMetadataModel> innerGetMetadataModel()
        {
            if (IsSimpleStatusbar)
                yield return new GenericMetadataModel<string>(StatusText, "KEY_SimpleStatus");
            else
                using (IEnumerator<GenericMetadataModel> enumerator = getMetadataModel().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                        yield return enumerator.Current;
                }
                    
        }

        #endregion

        #region Data

        private ObservableCollection<MetadataViewModel> _statusItemList = null;
        private bool _isSimpleStatusbar = false;
        private string _statusText = "";

        #endregion

        #region Public Properties
        public ObservableCollection<MetadataViewModel> StatusItemList
        {
            get { return _statusItemList; }
            protected set { _statusItemList = value; NotifyPropertyChanged("StatusItemList"); }
        }

        public string StatusText
        {
            get { return _statusText; }
            set { _statusText = value; NotifyPropertyChanged("StatusText"); UpdateStatusbar(); }
        }
        
        public bool IsSimpleStatusbar
        {
            get { return _isSimpleStatusbar; }
            set { _isSimpleStatusbar = value; NotifyPropertyChanged("IsSimpleStatusbar"); }
        }

        #endregion

    }
}
