using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AsyncTest
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

        private CancellationTokenSource _cts;

        private async Task listAsync()
        {
            if (_cts != null)
                _cts.Cancel();
            _cts = new CancellationTokenSource();
            lbList.Items.Clear();
            var di = new DirectoryInfoEx(tbFolder.Text);
            foreach (var item in await di.GetFilesAsync("*", cbRecrusive.IsChecked.Value ? 
                SearchOption.AllDirectories : SearchOption.TopDirectoryOnly , _cts.Token))
                lbList.Items.Add(item);
        }

        private void btnList_Click(object sender, RoutedEventArgs e)
        {
           
            listAsync();
        }

        private void btnCabcel_Click(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
                _cts.Cancel();
        }
    }
}
