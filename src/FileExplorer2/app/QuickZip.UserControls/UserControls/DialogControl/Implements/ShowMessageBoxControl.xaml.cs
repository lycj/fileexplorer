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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickZip.UserControls.Dialogs
{
    /// <summary>
    /// Interaction logic for ShowMessageBoxControl.xaml
    /// </summary>
    public partial class ShowMessageBoxControl : UserControl, IShowMessageBox
    {
        public ShowMessageBoxControl()
        {
            InitializeComponent();
            DataContext = this;            
        }

        #region IShowMessageBox Members

        public string Message
        {
            get;
            set; 
        }

        #endregion

        #region IDialogCommon Members

        public string Title
        {
            get;
            set; 
        }

        public object Icon
        {
            get;
            set;
        }

        #endregion
    }
}
