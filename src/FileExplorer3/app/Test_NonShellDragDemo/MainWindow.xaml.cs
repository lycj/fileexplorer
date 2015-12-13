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

namespace Test_NonShellDragDemo
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

            tvDnd1.DataContext = new NumberTreeViewModel("TreeView");
            lvDnd1.DataContext = NumberListViewModel.GenerateRange("VirtualStackPanel", 1, 20);
            lvDnd2.DataContext = NumberListViewModel.GenerateRange("VirtualWrapPanel", 21, 40);
            lvDnd3.DataContext = NumberListViewModel.GenerateRange("GridView", 41, 60);
            lvDnd4.DataContext = NumberListViewModel.GenerateRange("StackPanel", 61, 80);
        }
    }
}
