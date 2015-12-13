using System;
using System.Diagnostics;
//using DotNetOpenAuth.OAuth2;
using Google.Apis.Auth.OAuth2;
//using Google.Apis.Auth.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
//using Google.Apis.Util;
using Google.Apis.Services;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Models;

namespace FileExplorer.Models
{
    public class GoogleDriveLogin : ILoginInfo
    {
        //https://developers.google.com/accounts/docs/OAuth2Login#accessingtheservice
        public GoogleDriveLogin(string clientId)
        {
            StartUrl =
                String.Format("https://accounts.google.com/o/oauth2/auth?client_id={0}&response_type=code&" +
                "scope=https://www.googleapis.com/auth/drive&redirect_uri=urn:ietf:wg:oauth:2.0:oob", clientId);
        }

        public bool CheckLogin(BrowserStatus status)
        {
            if (status.Title.StartsWith("Success code="))
            {
                AuthCode = status.Title.Replace("Success code=", "");
                return true;
            }

            return false;
        }

        public string StartUrl { get; private set; }
        public string AuthCode { get; private set; }
    }

}
