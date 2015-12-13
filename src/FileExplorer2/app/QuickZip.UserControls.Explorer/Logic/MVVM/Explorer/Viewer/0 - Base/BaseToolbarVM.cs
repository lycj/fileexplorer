using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.MVVM;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Command.Model;
using QuickZip.UserControls.MVVM.Command.ViewModel;
using System.Collections.ObjectModel;

namespace QuickZip.UserControls.MVVM
{
    public abstract class BaseToolbarViewModel : BaseStatusbarViewModel
    {

        #region Constructor

        public BaseToolbarViewModel()
        {
                        
        }

        #endregion

        #region Methods

        public void UpdateToolbar()
        {

            ToolbarItemList = new ObservableCollection<CommandViewModel>(
              from am in getActionModel() select am.ToViewModel()
              ); 
        }        

        protected virtual IEnumerable<CommandModel> getActionModel()
        {
            yield break;
        }

        protected virtual void OnIsPreviewerVisibleChanged()
        {

        }

        #endregion

        #region Data

        private ObservableCollection<CommandViewModel> _toolbarItemList = null;
        private bool _isPreviewerVisible = false;
        private string _previewerSource = null;

        #endregion

        #region Public Properties
        public ObservableCollection<CommandViewModel> ToolbarItemList
        {
            get { return _toolbarItemList; }
            protected set { _toolbarItemList = value; NotifyPropertyChanged("ToolbarItemList"); }
        }

        public bool IsPreviewerVisible
        {
            get { return _isPreviewerVisible; }
            set
            {
                if (_isPreviewerVisible != value)
                {
                    _isPreviewerVisible = value; NotifyPropertyChanged("IsPreviewerVisible");
                    OnIsPreviewerVisibleChanged();
                }
            }
        }

        public string PreviewerSource
        {
            get { return _previewerSource; }
            protected set { _previewerSource = value; NotifyPropertyChanged("PreviewerSource"); }
        }

        #endregion

    }
}
