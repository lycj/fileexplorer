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

namespace QuickZip.Dialogs
{
    /// <summary>
    /// Interaction logic for dlgPassword.xaml
    /// </summary>
    public partial class PasswordDialog : Window
    {
        public PasswordDialog()
        {
            InitializeComponent();
        }

        #region Methods

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //    _loaded = true;
        //    pBox.Password = _password;
        //}

        public static void OnPasswordChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            PasswordDialog dlg = sender as PasswordDialog;
            string password = args.NewValue as string;

            //dlg._password = password;
            //if (!dlg._loaded)            
                dlg.pBox.Password = password;
        }

        #endregion

        #region Data


        //private string _password = "";
        private bool _loaded = false;

        #endregion

        #region Public Properties
        


        public string PasswordPeoperty
        {
            get { return (string)GetValue(PasswordPeopertyProperty); }
            set { SetValue(PasswordPeopertyProperty, value); }
        }
        
        public static readonly DependencyProperty PasswordPeopertyProperty =
            DependencyProperty.Register("PasswordPeoperty", typeof(string), 
            typeof(PasswordDialog), new UIPropertyMetadata("", new PropertyChangedCallback(OnPasswordChanged)));

        

        public string SourceNameProperty
        {
            get { return (string)GetValue(SourceNamePropertyProperty); }
            set { SetValue(SourceNamePropertyProperty, value); }
        }
        
        public static readonly DependencyProperty SourceNamePropertyProperty =
            DependencyProperty.Register("SourceNameProperty", typeof(string), typeof(PasswordDialog), new UIPropertyMetadata(""));

        

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void bOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
