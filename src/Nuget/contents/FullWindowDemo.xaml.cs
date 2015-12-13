using FileExplorer.Utils;
using System;
using System.Collections.Generic;
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

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for FullWindowDemo.xaml
    /// </summary>
    public partial class FullWindowDemo : Window
    {
        public FullWindowDemo()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            FileExplorer.Models.FileSystemInfoExProfile profile =
                new FileExplorer.Models.FileSystemInfoExProfile(null, null);
            var desktopDir = AsyncUtils.RunSync(() => profile.ParseAsync(""));
            explorer.RootDirectories = new FileExplorer.Models.IEntryModel[] { desktopDir };
        }
    }
}
