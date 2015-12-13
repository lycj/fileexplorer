using ShellDll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace ListSpecialDirectoryGuid
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            string[] txtFiles = new DirectoryInfoEx(KnownFolderIds.DocumentsLibrary).EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly)
                .Select(fi => fi.FullName)
                .ToArray();

            Console.WriteLine(txtFiles);

            var tbCSIDL = this.FindName("tbCSIDL") as TextBox;
            var tbKnownFolder = this.FindName("tbKnownFolder") as TextBox;
            foreach (var enumItem in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                Environment.SpecialFolder sf = (Environment.SpecialFolder)enumItem;
                try
                {
                    var csidl = IOTools.ShellFolderToCSIDL(sf);
                    if (!csidl.HasValue)
                        System.Diagnostics.Debugger.Break();
                    if (csidl.HasValue)
                    {                        
                        DirectoryInfoEx di = new DirectoryInfoEx(csidl.Value);
                        di = new DirectoryInfoEx(sf);
                        tbCSIDL.Text += String.Format("SpecialFolder={0}, Label={1}, Csidl={2}, Guid={3}", di.ShellFolderType, di.Label, csidl, di.FullName);
                    }

                }
                catch (ArgumentException ex)
                {
                    tbCSIDL.Text += String.Format("SpecialFolder={0} Not supported", sf);
                }
                tbCSIDL.Text += Environment.NewLine;
            }

            foreach (var f in KnownFolder.GetKnownFolders())
            {
                string path = f.Path;
                if (path != null)
                {
                    DirectoryInfoEx di = new DirectoryInfoEx(path);
                    if (di.KnownFolderType != null)
                        tbKnownFolder.Text += String.Format("C={0}, N={1}, KFID={2}, Path={3}", di.KnownFolderType.Category,
                            di.KnownFolderType.Definition.Name,
                            di.KnownFolderType.KnownFolderId,
                            di.FullName) + Environment.NewLine;
                }
                //tbKnownFolder.Text += String.Format("{0}, {1}, {2}", f.Category, f.Definition.LocalizedName, f.Path) + Environment.NewLine;                
            }
            //var knownFolder = KnownFolder.FromCsidl(ShellAPI.CSIDL.CSIDL_COMMON_DOCUMENTS);
            //var di1 = new DirectoryInfoEx(KnownFolder.FromCsidl(ShellAPI.CSIDL.CSIDL_COMMON_DOCUMENTS));
            //knownFolder = KnownFolder.FromDirectoryPath(@"C:\", KnownFolderFindMode.ExactMatch);
            //knownFolder.pi
            //var knownFolder = KnownFolders.GetKnownFolder(KnownFolderIdentifiers.LocalAppData);
            ////KnownFolder.IKnownFolder foundFolder;
            ////KnownFolders._knownFolderManager.FindFolderFromPath(knownFolder.Path, KnownFolderFindMode.ExactMatch, 
            ////    out foundFolder);
            //tb.Text +=
            //    String.Format("{0}", di1);
        }

    }
}
