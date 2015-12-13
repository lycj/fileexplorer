using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using Caliburn.Micro;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Models;

namespace FileExplorer.WPF.ViewModels
{
    public class LoginViewModel : Screen
    {
        #region Constructor

        public LoginViewModel(ILoginInfo loginInfo)
        {
            LoginInfo = loginInfo;
            DisplayName = "";
        }

        #endregion

        #region Methods

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            var uc = view as UserControl;
            _webBrowser = uc.FindName("webBrowser") as WebBrowser;

            _webBrowser.LoadCompleted += loadCompleted;
            _webBrowser.Navigating += navigating;
            _webBrowser.Navigate(LoginInfo.StartUrl);
            

        }

        void navigating(object sender, NavigatingCancelEventArgs e)
        {            
            CurrentUri = e.Uri;
            if (LoginInfo != null)
                if (LoginInfo.CheckLogin(new BrowserStatus()
                {
                    Url = CurrentUri,
                    IsCompleted = false
                }))
                    TryClose(true);
        }

        void loadCompleted(object sender, NavigationEventArgs e)
        {
            DisplayName = ((dynamic)_webBrowser.Document).Title;
            CurrentUri = e.Uri;
            if (LoginInfo != null)
                if (LoginInfo.CheckLogin(new BrowserStatus()
                    {
                        Url = CurrentUri, 
                        Title = ((dynamic)_webBrowser.Document).Title,
                        IsCompleted = true
                    }))
                    TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }

        #endregion

        #region Data

        WebBrowser _webBrowser;
        private ILoginInfo _loginInfo;
        private Uri _currentUri;

        #endregion

        #region Public Properties

        public ILoginInfo LoginInfo { get { return _loginInfo; } set { _loginInfo = value; NotifyOfPropertyChange(() => LoginInfo); } }
        public Uri CurrentUri { get { return _currentUri; } set { _currentUri = value; NotifyOfPropertyChange(() => CurrentUri); } }        

        #endregion
    }
}
