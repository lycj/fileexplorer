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

namespace TestApp.WPF
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

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Hyperlink)
            {
                MessageBox.Show((e.OriginalSource as Hyperlink).NavigateUri.ToString());
                e.Handled = true;
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AddHandler(Hyperlink.ClickEvent, (RoutedEventHandler)Hyperlink_Click);

        }
    }
}
