#region Copyright (c) Lokad.com 2008
/* Copyright (c) Lokad.com 2008 
 * Company: http://www.lokad.com
 * 
 * This code is released under the terms of the new BSD licence.
 */
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

using Microsoft.Win32;

namespace Lokad.Web.Services
{
    /// <seealso cref="VersionChecker"/>
    [Serializable]
    public class VersionDetectedEventArgs : EventArgs
    {
        bool isNewVersionDetected;
        Version version;

        /// <remarks/>
        public VersionDetectedEventArgs(bool isNewVersionDetected, Version version)
        {
            this.isNewVersionDetected = isNewVersionDetected;
            this.version = version;
        }

        /// <summary>Indicates whether a new version has been detected.</summary>
        public bool IsNewVersionDetected
        {
            get { return isNewVersionDetected; }
        }

        /// <summary>Gets the new version that has been detected.</summary>
        public Version Version
        {
            get { return version; }
        }
    }

    /// <seealso cref="VersionChecker"/>
    [Serializable]
    public class VersionRetrievedEventArgs : EventArgs
    {
        bool isNewVersionDetected;
        Version version;
        string localMsiFileName;

        /// <remarks/>
        public VersionRetrievedEventArgs(bool isNewVersionDetected, Version version, string localMsiFileName)
        {
            this.isNewVersionDetected = isNewVersionDetected;
            this.version = version;
            this.localMsiFileName = localMsiFileName;
        }

        /// <summary>Indicates whether a new version has been detected.</summary>
        public bool IsNewVersionDetected
        {
            get { return isNewVersionDetected; }
        }

        /// <summary>Gets the new version that has been detected.</summary>
        public Version Version
        {
            get { return version; }
        }

        /// <summary>Get the path of the MSI package (locally retrieved
        /// during the async process).</summary>
        public string LocalMsiFileName
        {
            get { return localMsiFileName; }
        }

        /// <summary>Install the local MSI package (launching an independent process).</summary>
        public void InstallMsiAsync()
        {
            Process.Start("msiexec", string.Format(@"/i ""{0}"" REINSTALL=ALL REINSTALLMODE=vomus", LocalMsiFileName));
        }
    }

    /// <summary>Utility classes to retrieve meta data stored in an online PAD file,
    /// and optionally perform the software update if needed.</summary>
    /// <remarks>See <a href="http://www.asp-shareware.org/pad/">http://www.asp-shareware.org/pad/</a>
    /// for more details about the Portable Application Description (PAD) format.</remarks>
    [Serializable]
    public class VersionChecker
    {
        class CheckerState
        {
            public Uri PadUri;
            public Version CurrentVersion;

            public CheckerState(Uri padUri, Version currentVersion)
            {
                this.PadUri = padUri;
                this.CurrentVersion = currentVersion;
            }
        }

        internal class PadInfo
        {
            public Version Version;
            public Uri DownloadUri;
            public string VersionedFileName;

            public PadInfo() { }

            public PadInfo(Version version, Uri downloadUri, string versionedFileName)
            {
                this.Version = version;
                this.DownloadUri = downloadUri;
                this.VersionedFileName = versionedFileName;
            }
        }

        Guid productId;

        /// <summary>Constructor</summary>
        /// <param name="productId">GUID associated to the <c>ProductCode</c> in the MSI package.</param>
        public VersionChecker(Guid productId)
        {
            this.productId = productId;
        }

        /// <summary>Fired when the PAD info is retrieved.</summary>
        public event EventHandler<VersionDetectedEventArgs> VersionDetected;

        /// <summary>Fired when the new version has been retrieved.</summary>
        public event EventHandler<VersionRetrievedEventArgs> VersionRetrieved;

        /// <summary>Asynchronous retrieval of the specified PAD file.</summary>
        /// <remarks>
        /// <para>The event <see cref="VersionRetrieved"/> is fired once the version
        /// has been retrieved. The retrieval is queued in the threadpool.</para>
        /// <para>If a new version is detected, then the corresponding MSI package
        /// is downloaded to the temporary internet files folder.</para></remarks>
        public void AsyncGetVersionFromPad(Uri padUri, Version currentVersion)
        {
            ThreadPool.QueueUserWorkItem(
                new WaitCallback(this.InternalAsyncGetVersionFromPad), 
                new CheckerState(padUri, currentVersion));
        }

        void InternalAsyncGetVersionFromPad(object state)
        {
            Uri padUri = ((CheckerState)state).PadUri;
            Version currentVersion = ((CheckerState)state).CurrentVersion;
            try
            {
                PadInfo padInfo = GetInfoFromPad(padUri);
                bool isNewVersionDetected = currentVersion.CompareTo(padInfo.Version) < 0;

                if (null != VersionDetected)
                {
                    VersionDetected(this, new VersionDetectedEventArgs(isNewVersionDetected, padInfo.Version));
                }

                string localMsiFileName = null;
                if (isNewVersionDetected &&
                    null != padInfo.DownloadUri && !string.IsNullOrEmpty(padInfo.VersionedFileName))
                {
                    localMsiFileName =
                        DownloadMsi(padInfo.DownloadUri,
                            GetPackageNameFromRegistry(productId, padInfo.VersionedFileName));
                }

                if (null != VersionRetrieved)
                {
                    VersionRetrieved(this,
                        new VersionRetrievedEventArgs(isNewVersionDetected, padInfo.Version, localMsiFileName));
                }
            }
            catch (WebException)
            {
                // silent failure in case of network failure.
            }
            catch (InvalidOperationException)
            {
                // silent failure in case of PAD parsing failure.
            }
            catch (Exception)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        /// <summary>Retrieve the version number contained in an online PAD file.</summary>
        internal static PadInfo GetInfoFromPad(Uri padUri)
        {
            // HACK: if network connection is down, this method must fail in a clean manner.
            // For ex, an 'null' value could be returned.

            // retrieving the latest version number from the published PAD file
            using (XmlTextReader reader = new XmlTextReader(padUri.ToString()))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(reader);

                    PadInfo padInfo = new PadInfo();

                    XmlNode node = document.SelectSingleNode(@"/XML_DIZ_INFO/Program_Info/Program_Version");

                    if (null != node)
                    {
                        Version version = null;
                        if (TryParseVersion(node.InnerText, out version))
                        {
                            padInfo.Version = version;
                        }
                        else
                        {
                            throw new InvalidOperationException("Version number cannot be parsed.");
                        }
                    }

                    node = document.SelectSingleNode(@"/XML_DIZ_INFO/Program_Info/File_Info/Filename_Versioned");
                    if (null != node)
                    {
                        padInfo.VersionedFileName = node.InnerText;
                    }
                    else
                    {
                        node = document.SelectSingleNode(@"/XML_DIZ_INFO/Program_Info/Program_Name");
                        if (node != null)
                        {
                            string name = node.InnerText;
                            node = document.SelectSingleNode(@"/XML_DIZ_INFO/Program_Info/Program_Version");
                            if (node != null)
                            {
                                string version = node.InnerText;
                                padInfo.VersionedFileName = (name + version.Replace(".", "")).Replace(" ", "") + ".msi";
                            }
                        }
                    }

                    node = document.SelectSingleNode(@"/XML_DIZ_INFO/Web_Info/Download_URLs/Primary_Download_URL");
                    if (null != node)
                    {
                        try
                        {
                            padInfo.DownloadUri = new Uri(node.InnerText);                            
                        }
                        catch (UriFormatException)
                        {
                            padInfo.DownloadUri = null;
                        }
                    }

                    return padInfo;
                }
                catch (XmlException)
                {
                    throw new InvalidOperationException("PAD file does not appear to be correct XML.");
                }
            }
        }

        /// <summary>Download the MSI package.</summary>
        /// <param name="msiLocation">URL of the MSI package.</param>
        /// <param name="versionedFileName">Name of the local copy of the MSI package.</param>
        /// <returns>The full path of the local copy of the MSI package.</returns>
        internal static string DownloadMsi(Uri msiLocation, string versionedFileName)
        {
            string fullName =
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + versionedFileName;

            // Delete previous copies of the MSI package if any.
            if (File.Exists(fullName))
            {
                File.Delete(fullName);
            }

            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(msiLocation, fullName + ".tmp");

                // Heuristic: download is (probably) not atomic - at least nothing is
                // specified in the MSDN docs - yet, the application can be closed during
                // download. This would typically lead to a corrupted .msi file. Thus,
                // we are moving the file at the end of the download.
                File.Move(fullName + ".tmp", fullName);
            }
            catch (WebException)
            {
                return null;
            }

            return fullName;
        }

        /// <summary>Parsing and normalizing incomplete version numbers.</summary>
        /// <returns><c>true</c> if the parsing operation succeed.</returns>
        public static bool TryParseVersion(string versionAsString, out Version version)
        {
            version = null;

            if (null == versionAsString)
            {
                return false;
            }

            string[] tokens = versionAsString.Split(new char[] { '.' });

            int[] numbers = new int[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                if (!int.TryParse(tokens[i], NumberStyles.None, CultureInfo.InvariantCulture, out numbers[i]))
                {
                    return false;
                }
            }
            
            switch (numbers.Length)
            {
                case 2:
                    version = new Version(numbers[0], numbers[1], 0, 0);
                    break;

                case 3:
                    version = new Version(numbers[0], numbers[1], numbers[2], 0);
                    break;

                case 4:
                    version = new Version(numbers[0], numbers[1], numbers[2], numbers[3]);
                    break;

                default:
                    return false;
            }

            return true;
        }


        /// <summary>Returns the package name as installed for the specified Product ID.</summary>
        /// <remarks>
        /// The new MSI package name must match the old one, otherwise the MSI install fails.
        /// Absurd but true, see
        /// http://groups.google.fr/group/Wixg/browse_thread/thread/d5cb261379f29479
        /// </remarks>
        static string GetPackageNameFromRegistry(Guid productId, string defaultName)
        {      
            string regPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\Installer\\Products\\" +
                GetProductCodeFromProductId(productId) + "\\SourceList";

            return (string)Registry.GetValue(regPath, "PackageName", null) ?? defaultName;
        }

        /// <summary>For weird reasons, installed packages are not listed against
        /// their <c>ProductCode</c>, but against a permutation of their <c>ProductCode</c>.
        /// This method returns the token needed for retrieving the package name
        /// from the windows registry.</summary>
        /// <remarks>
        /// Code token found at
        /// http://www.hanselman.com/blog/CommentView.aspx?guid=4e93e0a7-7af9-4397-95dd-db013901e6ee
        /// </remarks>
        static string GetProductCodeFromProductId(Guid productId)
        {
            string raw = productId.ToString("N");
            char[] aRaw = raw.ToCharArray();

            //compressed format reverses 11 byte sequences of the original guid
            int[] revs
                = new int[] { 8, 4, 4, 2, 2, 2, 2, 2, 2, 2, 2 };
            int pos = 0;
            for (int i = 0; i < revs.Length; i++)
            {
                Array.Reverse(aRaw, pos, revs[i]);
                pos += revs[i];
            }
            string n = new string(aRaw);
            Guid newGuid = new Guid(n);

            //GUID in registry are all caps.
            return newGuid.ToString("N").ToUpper();
        }
    }
}
