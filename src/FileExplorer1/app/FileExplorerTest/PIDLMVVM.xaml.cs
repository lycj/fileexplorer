using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using QuickZip.IO.PIDL.UserControls.ViewModel;

namespace PIDLTest
{
    /// <summary>
    /// Interaction logic for PIDLMVVM.xaml
    /// </summary>
    public partial class PIDLMVVM : Window
    {
        public PIDLMVVM()
        {
            InitializeComponent();
            
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);  
            //this.dirTree.RootDirectory = FileSystemInfoEx.FromString(@"::{00021400-0000-0000-C000-000000000046}}") as DirectoryInfoEx;
            //this.dirTree.SelectedDirectory = FileSystemInfoEx.FromString(@"C:\") as DirectoryInfoEx;                           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FileSystemInfoEx fsi = (flist.Items[flist.Items.Count - 1] as FileListViewItemViewModel).EmbeddedModel.EmbeddedEntry;
            //fsi = (flist.Items[0] as FileListViewItemViewModel).EmbeddedModel.EmbeddedEntry;
            flist.Focus(fsi);
        }
    }
}
