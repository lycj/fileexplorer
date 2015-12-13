using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using TestApp.WPF;

namespace TestSkyDrive2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
            //Install-Package Google.Apis.Drive.v2 -Pre

            //var provider = new NativeApplicationClient(GoogleClient.GoogleDescription,
            //    AuthorizationKeys.GoogleDrive_Client_Id, AuthorizationKeys.GoogleDrive_Client_Secret);

            //Google
            //var client = new NativeApplicationClient(GoogleClient.GoogleDescription,
            //    AuthorizationKeys.GoogleDrive_Client_Id, AuthorizationKeys.GoogleDrive_Client_Secret);
            //client.ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(AuthorizationKeys.GoogleDrive_Client_Secret);

            //var state = client.GetClientAccessToken(new[] { GoogleClient.Scopes.Drive.Default });
            //var token = client.ExchangeUserCredentialForToken("lycjdev@gmail.com", "p@ssw0rd123p@ssw0rd123",
            //        new[] { GoogleClient.Scopes.Drive.AppsReadonly });
            
            //WindowsLive
            var client = new NativeApplicationClient(WindowsLiveClient.WindowsLiveDescription,
                null,null);
            //client.ClientCredentialApplicator = ClientCredentialApplicator.NetworkCredential(null);
            //var state = client.GetClientAccessToken(new[] { WindowsLiveClient.Scopes.Basic });
            var token = client.ExchangeUserCredentialForToken("lycj-dev@outlook.com", "p@ssw0rd123",
                    new[] { WindowsLiveClient.Scopes.Basic });


            //var token = client.ExchangeUserCredentialForToken("lycj-dev@outlook.com", "p@ssw0rd123",
            //        new[] { WindowsLiveClient.Scopes.Basic });
            //provider.ClientIdentifier = "0000000040112888";
            //provider.ClientSecret = "qIueVYvFCKEQ0-43jC9qkVzbXAkHwnMr";
//            var auth = new OAuth2Authenticator<NativeApplicationClient>(provider, GetAuthorization);
            //auth.LoadAccessToken();
            //var service = new DriveService(new BaseClientService.Initializer()
            //{
            //    Authenticator = auth
            //});

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

            //AsyncUtils.RunSync(() => testUpload());
            //SkyDriveLogin login;
            //lc.LoginInfo = login = new SkyDriveLogin( "0000000040112888");
            //lc.AddHandler(LoginControl.CompletedEvent, (RoutedEventHandler)((o, e) =>
            //    {
            //        //login.AuthCode
            //        initAuth(login.AuthCode);
            //    }));
        }

    }
}
