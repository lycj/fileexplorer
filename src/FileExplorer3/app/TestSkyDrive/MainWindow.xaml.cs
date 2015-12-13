using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Caliburn.Micro;
using Cofe.Core.Utils;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth2;
using FileExplorer.BaseControls;
using FileExplorer.Models;
using FileExplorer.ViewModels;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Microsoft.Live;

namespace TestSkyDrive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

          
            
            //var _authClient = new LiveAuthClient();
            //string url = _authClient.GetLoginUrl(new[] { "wl.signin", "wl.basic", "wl.skydrive" });
            //wb.Navigate(url);

        }

        private static readonly AuthorizationServerDescription WindowsLiveDescription = new AuthorizationServerDescription
        {
            TokenEndpoint = new Uri("https://oauth.live.com/token"),
            AuthorizationEndpoint = new Uri("https://oauth.live.com/authorize"),
            ProtocolVersion = ProtocolVersion.V20
        };

        private static IAuthorizationState GetAuthorization(NativeApplicationClient arg)
        {
            // Get the auth URL:
            IAuthorizationState state = new AuthorizationState(
                new[] { WindowsLiveClient.Scopes.Basic });
            state.Callback = new Uri(NativeApplicationClient.OutOfBandCallbackUrl);
            Uri authUri = arg.RequestUserAuthorization(state);

            // Request authorization from the user (by opening a browser window):

            Process.Start(authUri.ToString());
            Console.Write("  Authorization Code: ");
            string authCode = Console.ReadLine();
            Console.WriteLine();

            //var authCode = BrowserForm.GetToken(authUri.ToString());

            // Retrieve the access token by using the authorization code:
            return arg.ProcessUserAuthorization(authCode ?? string.Empty, state);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //Install-Package Google.Apis.Authentication.OAuth2 -Version 1.2.4696.27634 
            //Install-Package DotNetOpenAuth -Version 4.3.4.13329

            var provider = new NativeApplicationClient(WindowsLiveDescription);
            provider.ClientIdentifier = "0000000040112888";
            //provider.ClientSecret = "qIueVYvFCKEQ0-43jC9qkVzbXAkHwnMr";
            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);

            //var plus = new PlusService(auth);
            //plus.Key = "BLAH";
            //var me = plus.People.Get("me").Fetch();
            //Console.WriteLine(me.DisplayName);

            //    var client = new WindowsLiveClient()
            //    {
            //        //ClientIdentifier = "0000000040112888",
            //        //ClientCredentialApplicator = ClientCredentialApplicator.PostParameter("qIueVYvFCKEQ0-43jC9qkVzbXAkHwnMr"),
            //    };
            //    //client.ClientCredentialApplicator = ClientCredentialApplicator.NetworkCredential(
            //    //client.ClientCredentialApplicator = ClientCredentialApplicator.PostParameter("0000000040112888");
            //    var state = client.ExchangeUserCredentialForToken("lycj-dev@outlook.com", "p@ssw0rd123",
            //        new[] { WindowsLiveClient.Scopes.Basic });
    
            AsyncUtils.RunSync(() => testUpload());
            //SkyDriveLogin login;
            //lc.LoginInfo = login = new SkyDriveLogin( "0000000040112888");
            //lc.AddHandler(LoginControl.CompletedEvent, (RoutedEventHandler)((o, e) =>
            //    {
            //        //login.AuthCode
            //        initAuth(login.AuthCode);
            //    }));
        }

        public IWindowManager _windowManager = new WindowManager();
        private static string clientId = "0000000040112888";

        private string loginSkyDrive()
        {
            //var login = new SkyDriveLogin(clientId);
            //if (_windowManager.ShowDialog(new LoginViewModel(login)).Value)
            //{
            //   return login.AuthCode;
            //}
            return null;
        }

        public async Task testUpload()
        {
            //var server = new AuthorizationServerDescription();
            //server.TokenEndpoint = new Uri("
           
                //.RequestUserAuthorization(new [] { WindowsLiveClient.Scopes.SkydriveUpdate }, null);

            //var _profileSkyDrive = new SkyDriveProfile(null, clientId, loginSkyDrive);
            //var rootModel = new[] { await _profileSkyDrive.ParseAsync("/me/skydrive") };
            //var newFile = new SkyDriveItemModel(_profileSkyDrive, "/SkyDrive/Photos/upload.txt", new object(), null);
            //string ioPath = _profileSkyDrive.PathMapper[newFile].IOPath;
            //using (var sw = new StreamWriter(File.OpenWrite(ioPath)))
            //    sw.WriteLine("upload");
            //await _profileSkyDrive.PathMapper.UpdateSourceAsync(newFile);                
        }

        public async void initAuth(string authCode)
        {
            //var authClient = new LiveAuthClient("0000000040112888");
            //LiveConnectSession session = await authClient.ExchangeAuthCodeAsync(authCode);
            //LiveConnectClient client = new LiveConnectClient(session);
            //LiveOperationResult liveOpResult = await client.GetAsync("me/albums");
            //dynamic dynResult = liveOpResult.Result;
            ////LiveLoginResult authResult = await authClient.LoginAsync(new List<string>() { "wl.signin", "wl.basic", "wl.skydrive" });
        }

        //public static string ParseParamStringPattern = @"([&]?(?<key>[^&^=]*)=(?<value>[^&^=]*))";
        //public static string ParseAlphaParamStringPattern = @"([&]?(?<key>[\w]*)=(?<value>[\w]*))";


        //public Dictionary<string, string> ParseParamString(string url)
        //{
        //    url = url.Substring(url.IndexOf('?') + 1);

        //    int startPos = 0;
        //    Regex regex = new Regex(ParseParamStringPattern);
        //    Dictionary<string, string> retVal = new Dictionary<string, string>();
        //    while (startPos < url.Length)
        //    {
        //        var match = regex.Match(url, startPos);
        //        if (!match.Success)
        //            throw new ArgumentException();
        //        startPos = match.Index + match.Length;
        //        string key = match.Groups["key"].Value;
        //        string value = match.Groups["value"].Value.Replace("AmPAmP", "&").Replace("eQuAleQual", "=");
        //        retVal.Add(key, value);
        //    }
        //    return retVal;
        //}

        //private void wb_LoadCompleted(object sender, NavigationEventArgs e)
        //{
        //    if (e.Uri.AbsoluteUri.Contains("code="))
        //    {
        //        if (App.Current.Properties.Contains("auth_code"))
        //        {
        //            App.Current.Properties.Clear();
        //        }

        //        Match match = Regex.Match(e.Uri.AbsoluteUri, ParseAlphaParamStringPattern);
        //        string auth_code = ParseParamString(e.Uri.AbsoluteUri)["code"];
        //        App.Current.Properties.Add("auth_code", auth_code);
        //        this.Close();
        //    }
        //}
    }
}
