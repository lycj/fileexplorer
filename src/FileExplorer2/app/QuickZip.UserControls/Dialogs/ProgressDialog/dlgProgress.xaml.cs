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
using System.IO.Tools;

namespace QuickZip.Dialogs
{    
    /// <summary>
    /// Interaction logic for dlgProgress.xaml
    /// </summary>
    public partial class ProgressDialog : Window, ICustomProgressDialog
    {        

        public static DependencyProperty ProgressModeProperty = DependencyProperty.Register("ProgressMode", typeof(ProgressMode), typeof(ProgressDialog), new UIPropertyMetadata(ProgressMode.Normal, new PropertyChangedCallback(OnProgressModeChanged)));        
        public static DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(int), typeof(ProgressDialog), new UIPropertyMetadata(0, new PropertyChangedCallback(OnProgressChanged)));        
        public static DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(ProgressDialog), new UIPropertyMetadata("Header property"));
        public static DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ProgressDialog), new UIPropertyMetadata("Message property"));
        public static DependencyProperty SubMessageProperty = DependencyProperty.Register("SubMessage", typeof(string), typeof(ProgressDialog), new UIPropertyMetadata(""));
        public static DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(ProgressDialog), new UIPropertyMetadata(""));
        public static DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(string), typeof(ProgressDialog), new UIPropertyMetadata(""));
        public static DependencyProperty TotalItemsProperty = DependencyProperty.Register("TotalItems", typeof(int), typeof(ProgressDialog), new UIPropertyMetadata(0));
        public static DependencyProperty ItemsCompletedProperty = DependencyProperty.Register("ItemsCompleted", typeof(int), typeof(ProgressDialog),
            new UIPropertyMetadata(0, new PropertyChangedCallback(OnItemCompletedChanged)));
        public static DependencyProperty ItemsRemainProperty = DependencyProperty.Register("ItemsRemain", typeof(int), typeof(ProgressDialog), new UIPropertyMetadata(0));
        public static DependencyProperty TimeRemainProperty = DependencyProperty.Register("TimeRemain", typeof(TimeSpan), typeof(ProgressDialog), new UIPropertyMetadata(TimeSpan.FromSeconds(0) ));
        public static DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(ProgressDialog), new PropertyMetadata());

        public static DependencyProperty IsCompletedProperty = DependencyProperty.Register("IsCompleted", typeof(bool), typeof(ProgressDialog), new UIPropertyMetadata(false));
        public static DependencyProperty IsCancelEnabledProperty = DependencyProperty.Register("IsCancelEnabled", typeof(bool), typeof(ProgressDialog), new UIPropertyMetadata(true));
        public static DependencyProperty IsRestartEnabledProperty = DependencyProperty.Register("IsRestartEnabled", typeof(bool), typeof(ProgressDialog), new UIPropertyMetadata(false));
        public static DependencyProperty IsPauseEnabledProperty = DependencyProperty.Register("IsPauseEnabled", typeof(bool), typeof(ProgressDialog), new UIPropertyMetadata(false));
        public static DependencyProperty IsResumeEnabledProperty = DependencyProperty.Register("IsResumeEnabled", typeof(bool), typeof(ProgressDialog), new UIPropertyMetadata(false));

        public static DependencyProperty IsCanceledProperty = DependencyProperty.Register("IsCanceled", typeof(bool), typeof(ProgressDialog), new UIPropertyMetadata(false));
        public static DependencyProperty IsPausedProperty = DependencyProperty.Register("IsPaused", typeof(bool), typeof(ProgressDialog), new UIPropertyMetadata(false));

        public ProgressMode ProgressMode { get { return (ProgressMode)GetValueEx(ProgressModeProperty); } set { SetValueEx(ProgressModeProperty, value); } }
        public int Progress { get { return (int)GetValueEx(ProgressProperty); } internal set { SetValueEx(ProgressProperty, value); } }
        public new string Title { get { return (string)GetValueEx(TitleProperty); } set { SetValueEx(TitleProperty, value); } }
        public string Header { get { return (string)GetValueEx(HeaderProperty); } set { SetValueEx(HeaderProperty, value); } }
        public string Message { get { return (string)GetValueEx(MessageProperty); } set { SetValueEx(MessageProperty, value); } }
        public string SubMessage { get { return (string)GetValueEx(SubMessageProperty); } set { SetValueEx(SubMessageProperty, value); } }
        public string Source { get { return (string)GetValueEx(SourceProperty); } set { SetValueEx(SourceProperty, value); } }
        public string Target { get { return (string)GetValueEx(TargetProperty); } set { SetValueEx(TargetProperty, value); } }
        public int TotalItems { get { return (int)GetValueEx(TotalItemsProperty); } set { SetValueEx(TotalItemsProperty, value); } }
        public int ItemsCompleted { get { return (int)GetValueEx(ItemsCompletedProperty); } set { SetValueEx(ItemsCompletedProperty, value); } }
        public int ItemsRemain { get { return (int)GetValueEx(ItemsRemainProperty); } internal set { SetValueEx(ItemsRemainProperty, value); } }
        public TimeSpan TimeRemain { get { return (TimeSpan)GetValueEx(TimeRemainProperty); } set { SetValueEx(TimeRemainProperty, value); } }
        public DateTime StartTime { get { return (DateTime)GetValueEx(StartTimeProperty); } internal set { SetValueEx(StartTimeProperty, value); } }

        public bool IsCompleted { get { return (bool)GetValueEx(IsCompletedProperty); } set { SetValueEx(IsCompletedProperty, value); } }
        public bool IsCancelEnabled { get { return (bool)GetValueEx(IsCancelEnabledProperty); } set { SetValueEx(IsCancelEnabledProperty, value); } }
        public bool IsRestartEnabled { get { return (bool)GetValueEx(IsRestartEnabledProperty); } set { SetValueEx(IsRestartEnabledProperty, value); } }
        public bool IsPauseEnabled { get { return (bool)GetValueEx(IsPauseEnabledProperty); } set { SetValueEx(IsPauseEnabledProperty, value); } }
        public bool IsResumeEnabled { get { return (bool)GetValueEx(IsResumeEnabledProperty); } set { SetValueEx(IsResumeEnabledProperty, value); } }
        public bool IsCanceled { get { return (bool)GetValueEx(IsCanceledProperty); } set { SetValueEx(IsCanceledProperty, value); } }
        public bool IsPaused { get { return (bool)GetValueEx(IsPausedProperty); } set { SetValueEx(IsPausedProperty, value); } }


        public static DependencyProperty UIScaleProperty =
            DependencyProperty.Register("UIScale", typeof(double), typeof(ProgressDialog),
            new PropertyMetadata(1.0d));

        public double UIScale
        {
            get { return (double)GetValue(UIScaleProperty); }
            set { SetValue(UIScaleProperty, value); }
        }

        public TimeSpan TimeUsed { get { return DateTime.Now.Subtract(StartTime); } }

        //internal RoutedEvent OnErrorEvent = EventManager.RegisterRoutedEvent("OnError", RoutingStrategy.Bubble, typeof(EventHandler), typeof(ProgressDialog));
        //internal RoutedEvent OnErrorEvent = EventManager.RegisterRoutedEvent("On", RoutingStrategy.Bubble, typeof(EventHandler), typeof(ProgressDialog));
        //internal RoutedEvent OnErrorEvent = EventManager.RegisterRoutedEvent("OnError", RoutingStrategy.Bubble, typeof(EventHandler), typeof(ProgressDialog));

        public void SetValueEx(DependencyProperty property, object value)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Send, (SendOrPostCallback)delegate { SetValue(property, value); }, null);
        }

        public object GetValueEx(DependencyProperty property)
        {
            object retVal = null;
            this.Dispatcher.Invoke(DispatcherPriority.Send, (SendOrPostCallback)delegate { retVal = GetValue(property); }, null);
            return retVal;
        }

        public void ShowWindow()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Send, (SendOrPostCallback)delegate { Show(); }, null);
        }

        public void CloseWindow()
        {
            this.Dispatcher.Invoke(DispatcherPriority.Send, (SendOrPostCallback)delegate { Close(); }, null);
        }

        public static void OnProgressModeChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ProgressDialog pd = (ProgressDialog)s;
            switch ((ProgressMode)e.NewValue)
            {
                case ProgressMode.Error: pd.pBar2.Foreground = Brushes.IndianRed; pd.pBar2.Visibility = Visibility.Visible; break;
                case ProgressMode.Abort: pd.pBar2.Foreground = Brushes.IndianRed; pd.pBar2.Visibility = Visibility.Visible; break;
                //case ProgressMode.Pause: pd.pBar2.Foreground = Brushes.Yellow; pd.pBar2.Visibility = Visibility.Visible; break;
                default: pd.pBar2.Visibility = Visibility.Hidden; break;
            }
            if (pd.pBar2.Visibility == Visibility.Visible)
                pd.pBar.Visibility = Visibility.Hidden;
            else pd.pBar.Visibility = Visibility.Visible;
        }

        public static void OnProgressChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ProgressDialog pd = (ProgressDialog)s;

            if ((int)e.NewValue != 0)
                pd.pBar.IsIndeterminate = false;
        }


        public static void OnItemCompletedChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            ProgressDialog pd = (ProgressDialog)s;

            if (pd.TotalItems > 0)
            {
                pd.Progress = (int)(((float)pd.ItemsCompleted / (float)pd.TotalItems) * 100.0);
                pd.ItemsRemain = pd.TotalItems - pd.ItemsCompleted;                
            }
        }

        public ProgressDialog(ICustomProgressDialog dataContext)
        {            
            InitializeComponent();
            DataContext = dataContext;
        }

        public ProgressDialog()
        {
            Title = "Title Property";
            StartTime = DateTime.Now;
            InitializeComponent();
            btnClose.Content = ApplicationCommands.Close.Text;
            
            DataContext = this;


            DispatcherTimer dt = new DispatcherTimer(TimeSpan.FromSeconds(1), 
                DispatcherPriority.Background,               
                delegate {                     
                    //Debug.WriteLine("Tick"); 
                    if (!IsCompleted)
                        TimeRemain = TimeSpan.FromTicks((Progress == 0 || Progress == 100) ? 0 : (long)((float)TimeUsed.Ticks / (float)Progress * 100));                     
                }, 
                this.Dispatcher);

            this.Closing += delegate { dt.Stop(); dt = null; };
        }

        #region IProgressDialogViewModel Members


        public event CancelClickedHandler OnCanceled;
        public event CancelClickedHandler OnPaused = new CancelClickedHandler(() => { });

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = true;
            if (OnCanceled != null)
                OnCanceled();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

    }
}
