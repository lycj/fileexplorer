using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using FileExplorer.Utils;

namespace FileExplorer.WPF.BaseControls
{
    internal static class utils
    {
        private static string ParseParamStringPattern = @"([&]?(?<key>[^&^=]*)=(?<value>[^&^=]*))";
        public static Dictionary<string, string> ParseParamString(string url)
        {
            url = url.Substring(url.IndexOf('?') + 1);

            int startPos = 0;
            Regex regex = new Regex(ParseParamStringPattern);
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            while (startPos < url.Length)
            {
                var match = regex.Match(url, startPos);
                if (!match.Success)
                    throw new ArgumentException();
                startPos = match.Index + match.Length;
                string key = match.Groups["key"].Value;
                string value = match.Groups["value"].Value.Replace("AmPAmP", "&").Replace("eQuAleQual", "=");
                retVal.Add(key, value);
            }
            return retVal;
        }

    }

    public interface ILoginInfo
    {
        bool CheckLogin(Uri url);
        string StartUrl { get; }
    }

    public class SkyDriveLogin : ILoginInfo
    {
        public SkyDriveLogin(string clientId)
        {
            StartUrl = "https://login.live.com/oauth20_authorize.srf?client_id=" + clientId + 
                "&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf" +
                "&scope=wl.signin%20wl.basic%20wl.skydrive&response_type=code&display=windesktop&locale=en-GB&state=&theme=win7";
        }

        public bool CheckLogin(Uri url)
        {
            var dic = utils.ParseParamString(url.AbsoluteUri);
            if (dic.ContainsKey("code"))
            {
                AuthCode = dic["code"];
                return true;
            }
            return false;
        }

        public string StartUrl { get; private set; }
        public string AuthCode { get; private set; }
    }

    public class LoginControl : Control
    {
        #region Constructor

        static LoginControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LoginControl),
                new System.Windows.FrameworkPropertyMetadata(typeof(LoginControl)));
        }

        public LoginControl()
        {
            //Parameters = new Dictionary<string, string>();
        }

        #endregion

        #region Methods


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _wb = this.Template.FindName("wb", this) as WebBrowser;
            _wb.LoadCompleted += _wb_LoadCompleted;

            //this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (ExecutedRoutedEventHandler)((o, e) =>
            //    {
            //        if (LoginInfo.CheckLogin(_url))
            //            this.RaiseEvent(new RoutedEventArgs(CompletedEvent));
            //        else this.RaiseEvent(new RoutedEventArgs(FailedEvent));
            //    })));

            initStartUrl();
        }

        void _wb_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _url = e.Uri;
            if (LoginInfo != null)
                if (LoginInfo.CheckLogin(_url))
                    this.RaiseEvent(new RoutedEventArgs(CompletedEvent));
        }

        private void initStartUrl()
        {
            if (_wb != null && LoginInfo != null)
            {
                _wb.Navigate(LoginInfo.StartUrl);
            }
        }

        public static void OnLoginInfoChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            LoginControl lc = obj as LoginControl;
            lc.initStartUrl();            
        }


        #endregion

        #region Data

        private WebBrowser _wb;
        private Uri _url;

        #endregion

        #region Public Properties

        //public Dictionary<string, string> Parameters { get; private set; }

        public ILoginInfo LoginInfo
        {
            get { return (ILoginInfo)GetValue(LoginInfoProperty); }
            set { SetValue(LoginInfoProperty, value); }
        }

        public static readonly DependencyProperty LoginInfoProperty =
            DependencyProperty.Register("LoginInfo", typeof(ILoginInfo), typeof(LoginControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnLoginInfoChanged)));

        public static RoutedEvent CompletedEvent = EventManager.RegisterRoutedEvent(
            "Completed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(LoginControl));


        #endregion

    }
}
