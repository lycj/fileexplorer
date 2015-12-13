using FileExplorer.WPF;
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

namespace Test_IOCPanelTest
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

            List<ValueViewModel> list = new List<ValueViewModel>();

            for (int i = 1; i <= 1997; i++)
                list.Add(new ValueViewModel(i));
            
            lv.ItemsSource = list;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lv.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("DigitSize");
            view.GroupDescriptions.Add(groupDescription);




            //foreach (var t in Enum.GetValues(typeof(LayoutType)))
            //    cbLayout.Items.Add(t);
            //cbLayout.SelectedIndex = 0;


                //lv.Items.Add(new ListViewItem() { Content = i });
            //lv.ItemsSource = new List<int>() { 1, 3, 5, 7, 9 };
        }
    }
}
