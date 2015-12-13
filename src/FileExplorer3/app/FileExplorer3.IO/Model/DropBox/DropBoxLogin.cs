using DropNet;
using DropNet.Models;
using FileExplorer.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    //Does not work for default version (IE7) of WebBrowser, please run SetWebBrowserToIE9.reg
    //http://weblog.west-wind.com/posts/2011/May/21/Web-Browser-Control-Specifying-the-IE-Version
    public class DropBoxLogin : ILoginInfo
    {
        private DropNetClient _client;
        

        public DropBoxLogin(string clientId, string clientSecret)
        {
            _client = new DropNetClient(clientId, clientSecret);
            
            StartUrl = _client.GetTokenAndBuildUrl("http://localhost");
        }

        public bool CheckLogin(BrowserStatus status)
        {
            if (status.Url.OriginalString.StartsWith("http://localhost"))
            {
                if (status.Url.OriginalString.Contains("oauth_token"))
                    AccessToken = _client.GetAccessToken();
                return true;
            }
            return false;
        }

        public string StartUrl
        {
            get;
            set;
        }
      
        public UserLogin AccessToken
        {
            get;
            set;
        }
    }
}
