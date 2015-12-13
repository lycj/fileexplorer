using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace QuickZip.UserControls.MVVM
{
    public class BaseW3ViewerViewModel : ViewerBaseVM
    {



        #region Constructor

        public BaseW3ViewerViewModel(Uri initialUri)
        {
            WebAddress = initialUri;
            IsBreadcrumbVisible = false;
            CurrentViewerMode = ViewerMode.W3;
        }

        #endregion


        #region Methods

        public override string ToString()
        {
            return "W3ViewerVM;" + _uri.ToString();
        }

        public virtual void ChangeAddress(Uri value)
        {
            if (_uri == null || !_uri.Equals(value))
            {
                _uri = value;                
                NotifyPropertyChanged("WebAddress");
            }

        }

        public override void Expand()
        {
            throw new NotImplementedException();
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        public override void OnUnload()
        {

        }

        protected override string getLabel()
        {
            return this.WebAddress.AbsoluteUri;
        }

        protected override string getToolTip()
        {
            return "";
        }

        protected override ImageSource getSmallIcon()
        {
            return null;
        }



        #endregion

        #region Data

        private Uri _uri;

        #endregion

        #region Public Properties

        public Uri WebAddress
        {
            get { return _uri; }
            set
            {
                ChangeAddress(value);
            }
        }

        public Uri UIWebAddress { get { return _uri; } 
            set {                
                DirectoryChanged(this, new DirectoryChangedEventArgs(value.AbsoluteUri)); 
            } }

        #endregion
    }
}
