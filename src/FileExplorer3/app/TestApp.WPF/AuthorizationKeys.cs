using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public static class AuthorizationKeys
    {
        public static string SkyDrive_Client_Id = "0000000040112888";
        public static string SkyDrive_Client_Secret = "qIueVYvFCKEQ0-43jC9qkVzbXAkHwnMr";
        //public static string GoogleDrive_Client_Id = "256177968390-g61mf5ep9du7q75fvpi2utmtdvparvea.apps.googleusercontent.com";
        //public static string GoogleDrive_Client_Secret = "wzSk3ABZCVcsLJDHoA1VVAkt";
        public static string DropBox_Client_Id = "2u1e0be86skg555";
        public static string DropBox_Client_Secret = "fhybo4ff8bh2pns";
        ////OneDrive API -  https://account.live.com/developers/applications
        //public static string SkyDrive_Client_Id = "Replace_your_SkyDrive_Client_Id";
        //public static string SkyDrive_Client_Secret = "Replace_your_SkyDrive_Client_Secret";

        //GoogleDrive API - https://cloud.google.com/console/project 
        //The code below is not used at this time, please download your secret to gapi_client_secret.json
        public static string GoogleDrive_Client_Id = null; //"Replace_your_GoogleDrive_Client_Id";
        public static string GoogleDrive_Client_Secret = null; //"Replace_your_GoogleDrive_Client_Secret";

        //DropBox API - https://www.dropbox.com/developers/apps/create
        //public static string DropBox_Client_Id = null; //"Replace_your_DropBox_Client_Id";
        //public static string DropBox_Client_Secret = null; //"Replace_your_DropBox_Client_Secret";
    }
}
