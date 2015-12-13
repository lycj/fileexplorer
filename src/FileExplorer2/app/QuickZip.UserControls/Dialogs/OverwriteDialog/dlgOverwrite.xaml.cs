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
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using QuickZip.Tools;
using System.Drawing;
using System.IO.Tools;

namespace QuickZip.Dialogs
{
    //public class OverwriteItemInfo : AutoNotifyPropertyChanged
    //{
    //    public Observable<Bitmap> Icon { get { return getVar<Bitmap>("Icon"); } set { setVar<Bitmap>("Icon", value); } }
    //    public Observable<string> Path
    //    {
    //        get { return getVar<string>("Path"); }
    //        set { setVar<string>("Path", value); }
    //    }
    //    public Observable<string> Label { get { return getVar<string>("Label"); } set { setVar<string>("Label", value); } }
    //    public Observable<string> Name { get { return getVar<string>("Name"); } set { setVar<string>("Name", value); } }
    //    public Observable<DateTime> Time { get { return getVar<DateTime>("Time"); } set { setVar<DateTime>("Time", value); } }
    //    public Observable<long> Length { get { return getVar<long>("Length"); } set { setVar<long>("Length", value); } }
    //    public Observable<string> CRC { get { return getVar<string>("CRC"); } set { setVar<string>("CRC", value); } }
    //}

    //public enum OverwriteActionType { actOverwrite, actKeepOriginal, actCancel };
    //public enum OverwriteMode { Ask = 0, omSkip, omOverwrite, omRenameOld, omCompareDate };

    /// <summary>
    /// Interaction logic for dlgProgress.xaml
    /// </summary>
    public partial class OverwriteDialog : Window//, ICustomOverwriteDialog
    {
       

        public OverwriteDialog()
        {
            InitializeComponent();
            this.AddHandler(Button.ClickEvent, new RoutedEventHandler(Button_Click));
            DataContext = this;
            ContentTemplate = (DataTemplate)this.Resources["dirTemplate"];
        }


        public static void OnIsFolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            OverwriteDialog odSender = (OverwriteDialog)sender;
            if (e.NewValue != e.OldValue)
            {
                if ((bool)e.NewValue)
                    odSender.ContentTemplate = (DataTemplate)odSender.Resources["dirTemplate"];
                else odSender.ContentTemplate = (DataTemplate)odSender.Resources["fileTemplate"];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button)
            {                
                Button button = e.OriginalSource as Button;
                if (button.Name == "overwrite")
                {
                    Overwrite = OverwriteMode.Replace;
                    this.DialogResult = true;
                }
                else if (button.Name == "dontoverwrite")
                {
                    Overwrite = OverwriteMode.KeepOriginal;
                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;                    
                }
                Close();
            }
        }

        //public new System.Windows.Forms.DialogResult ShowDialog()
        //{
        //    base.ShowDialog();            
        //    return _dialogResult;
        //} 


        #region Public Properties

        public static readonly DependencyProperty IsFolderProperty = DependencyProperty.Register("IsFolder",
           typeof(bool), typeof(OverwriteDialog), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsFolderChanged)));
        public bool IsFolder
        {
            get { return (bool)GetValue(IsFolderProperty); }
            set { SetValue(IsFolderProperty, value); }
        }

        //private System.Windows.Forms.DialogResult _dialogResult = System.Windows.Forms.DialogResult.OK;              
        public OverwriteMode Overwrite { get; set; }        

        public static readonly DependencyProperty ApplyAllProperty = DependencyProperty.Register("ApplyAll",
            typeof(bool), typeof(OverwriteDialog), new UIPropertyMetadata(false));
        public bool ApplyAll
        {
            get { return (bool)GetValue(ApplyAllProperty); }
            set { SetValue(ApplyAllProperty, value); }
        }
        public static readonly DependencyProperty SourceFileProperty = DependencyProperty.Register("SourceFile",
            typeof(OverwriteInfo), typeof(OverwriteDialog), new UIPropertyMetadata(new OverwriteInfo()));
        public OverwriteInfo SourceFile
        {
            get { return (OverwriteInfo)GetValue(SourceFileProperty); }
            set { SetValue(SourceFileProperty, value); }
        }

        public static readonly DependencyProperty DestinationFileProperty = DependencyProperty.Register("DestinationFile",
            typeof(OverwriteInfo), typeof(OverwriteDialog), new UIPropertyMetadata(new OverwriteInfo()));
        public OverwriteInfo DestinationFile
        {
            get { return (OverwriteInfo)GetValue(DestinationFileProperty); }
            set { SetValue(DestinationFileProperty, value); }
        }
        #endregion


    }

}
