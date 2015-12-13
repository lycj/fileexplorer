using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Http;
using FileExplorer.WPF.Models;

namespace FileExplorer.Models
{
   

    public class SkyDriveLogin : ILoginInfo
    {
        public SkyDriveLogin(string clientId)
        {
            StartUrl = "https://login.live.com/oauth20_authorize.srf?client_id=" + clientId +
                "&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf" +
                "&scope=wl.signin%20wl.basic%20wl.offline_access%20wl.skydrive%20wl.skydrive_update" +
                "&response_type=code&display=windesktop&locale=en-GB&state=&theme=win7";
        }

        public bool CheckLogin(BrowserStatus status)
        {
            var dic = ParamStringUtils.ParseParamString(status.Url.AbsoluteUri);
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


}
