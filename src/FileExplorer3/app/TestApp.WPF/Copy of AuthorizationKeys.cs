using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    //Copy this file to AuthorizationKeys.cs so it can compile.
    public static class AuthorizationKeys
    {
        //OneDrive API -  https://account.live.com/developers/applications
        public static string SkyDrive_Client_Id = null; //"Replace_your_SkyDrive_Client_Id";
        public static string SkyDrive_Client_Secret = null; //"Replace_your_SkyDrive_Client_Secret";

        //GoogleDrive API - https://cloud.google.com/console/project 
        //The code below is not used at this time, please download your secret to gapi_client_secret.json
        public static string GoogleDrive_Client_Id = null; //"Replace_your_GoogleDrive_Client_Id";
        public static string GoogleDrive_Client_Secret = null; //"Replace_your_GoogleDrive_Client_Secret";

        //DropBox API - https://www.dropbox.com/developers/apps/create
        public static string DropBox_Client_Id = null; //"Replace_your_DropBox_Client_Id";
        public static string DropBox_Client_Secret = null; //"Replace_your_DropBox_Client_Secret";
    }
}
