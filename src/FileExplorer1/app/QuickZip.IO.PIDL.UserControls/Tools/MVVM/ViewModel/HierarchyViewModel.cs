using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickZip.IO.PIDL.UserControls.ViewModel
{
    public abstract class HierarchyViewModel : ExViewModel
    {

        #region Constructor

        internal HierarchyViewModel()
            : base()
        {

        }

        public HierarchyViewModel(RootModelBase rootModel, Model.ExModel model)
            : base(rootModel, model)
        {
            _parent = null;
        }

        public HierarchyViewModel(Model.ExModel model) : base(model)             
        {
            _parent = null;
        }

        public HierarchyViewModel(RootModelBase rootModel, HierarchyViewModel parentModel, Model.ExModel model)
            : base(rootModel, model)
        {
            _parent = parentModel;
        }

        #endregion

        #region Methods        

        protected virtual void OnExpanded()
        {

        }

        protected virtual void OnCollapsed()
        {

        }

        public void CollapseTree()
        {
            this.IsExpanded = false;
            if (Parent != null)
                Parent.CollapseTree();
        }

        #endregion

        #region Data
        private HierarchyViewModel _parent;
        private bool _isExpanded = false;
        private bool _isSelected = false;
        #endregion

        #region Properties

        //http://www.codeproject.com/KB/WPF/TreeViewWithViewModel.aspx
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                    if (value)
                        OnExpanded();
                    else OnCollapsed();
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        public virtual void OnSelected()
        {

        }

        public virtual void OnUnselected()
        {

        }


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    if (value && this.Parent != null)
                        this.Parent.IsExpanded = true;
                    NotifyPropertyChanged("IsSelected");
                    if (value)
                        OnSelected();
                    else OnUnselected();
                }
            }
        }

        public HierarchyViewModel Parent
        {
            get { return _parent; }
            protected set { _parent = value; NotifyPropertyChanged("Parent"); }
        }

        #endregion
    }


}
