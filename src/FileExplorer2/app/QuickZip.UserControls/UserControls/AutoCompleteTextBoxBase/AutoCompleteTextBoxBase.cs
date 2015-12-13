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
using System.IO;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace QuickZip.UserControls
{
    public class Suggestion
    {
        public string Header { get; private set; }
        public string Value { get; private set; }
        public string SuggestionType { get; private set; } //dir, zip, srt (search result)
        public string Lookup { get; set; }

        public Suggestion(string type, string header, string value)
        {
            SuggestionType = type;
            Header = header;
            Value = Value;
        }

        public Suggestion(string type, string header)
        {
            SuggestionType = type;
            Header = header;
            Value = header;
        }

        public Suggestion(string header)
        {
            Header = header;
        }

        public override string ToString()
        {
            return Header;
        }

    }

    /// <summary>
    /// Interaction logic for AutoCompleteTextBoxBase.xaml
    /// </summary>
    public partial class AutoCompleteTextBoxBase : TextBox
    {
        #region Data
        Popup Popup { get { return this.Template.FindName("PART_Popup", this) as Popup; } }
        ListBox ItemList { get { return this.Template.FindName("PART_ItemList", this) as ListBox; } }
        Grid Root { get { return this.Template.FindName("root", this) as Grid; } }
        //12-25-08 : Add Ghost image when picking from ItemList
        //TextBlock TempVisual { get { return this.Template.FindName("PART_TempVisual", this) as TextBlock; } }
        ScrollViewer Host { get { return this.Template.FindName("PART_ContentHost", this) as ScrollViewer; } }
        UIElement TextBoxView { get { foreach (object o in LogicalTreeHelper.GetChildren(Host)) return o as UIElement; return null; } }

        protected bool _loaded = false;
        private bool prevState = false;
        string lastPath;
        #endregion

        #region Constructor
        static AutoCompleteTextBoxBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBoxBase), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBoxBase)));
        }

        #endregion



        #region Methods
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _loaded = true;
            this.KeyDown += new KeyEventHandler(AutoCompleteTextBox_KeyDown);
            this.PreviewKeyDown += new KeyEventHandler(AutoCompleteTextBox_PreviewKeyDown);
            ItemList.PreviewMouseDown += new MouseButtonEventHandler(ItemList_PreviewMouseDown);
            ItemList.KeyDown += new KeyEventHandler(ItemList_KeyDown);
            ItemList.MouseDoubleClick += new MouseButtonEventHandler(ItemList_MouseDoubleClick);
            //TempVisual.MouseDown += new MouseButtonEventHandler(TempVisual_MouseDown);
            //09-04-09 Based on SilverLaw's approach 
            Popup.CustomPopupPlacementCallback += new CustomPopupPlacementCallback(Repositioning);


            Window parentWindow = getParentWindow();
            if (parentWindow != null)
            {
                parentWindow.Deactivated += delegate { prevState = IsPopupOpened; IsPopupOpened = false; };
                parentWindow.Activated += delegate { IsPopupOpened = prevState; };
            }

            //this.AddHandler(TextBox.PreviewLostKeyboardFocusEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            //{
            //    if (ItemList.IsFocused || TextBoxView.IsFocused)
            //        e.Handled = true;
            //});

        }

        void ItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (sender.Equals(ItemList))
                {
                    if (ItemList.SelectedValue is Suggestion)
                    {
                        Text = (ItemList.SelectedValue as Suggestion).Value; updateSource();
                        RaiseEvent(new RoutedEventArgs(AutoCompleteTextBoxBase.SourceUpdatedEvent));
                    }
                }
            }
        }


        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (this.IsKeyboardFocusWithin)
                return;
            base.OnLostFocus(e);
            //if (ItemList.IsFocused || TextBoxView.IsFocused)
            //    e.Handled = true;
            //else base.OnLostFocus(e);
        }

        private Window getParentWindow()
        {
            DependencyObject d = this;
            while (d != null && !(d is Window))
                d = LogicalTreeHelper.GetParent(d);
            return d as Window;
        }

        //09-04-09 Based on SilverLaw's approach 
        private CustomPopupPlacement[] Repositioning(Size popupSize, Size targetSize, Point offset)
        {
            return new CustomPopupPlacement[] {
                new CustomPopupPlacement(new Point((0.01 - offset.X), (Root.ActualHeight - offset.Y)), PopupPrimaryAxis.None) };
        }

        void TempVisual_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string text = Text;
            ItemList.SelectedIndex = -1;
            Text = text;
            IsPopupOpened = false;
        }

        void AutoCompleteTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //12-25-08 - added PageDown Support
            if (_suggestions.Count > 0 && !(e.OriginalSource is ListBoxItem))
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.Down:
                    case Key.Prior:
                    case Key.Next:
                        ItemList.Focus();
                        ItemList.SelectedIndex = 0;
                        ListBoxItem lbi = ItemList.ItemContainerGenerator.ContainerFromIndex(ItemList.SelectedIndex) as ListBoxItem;
                        lbi.Focus();
                        e.Handled = true;
                        break;


                }

            if (e.Key == Key.Back && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Text.EndsWith("\\"))
                    Text = Text.Substring(0, Text.Length - 1);
                else
                    Text = GetDirectoryName(Text) + "\\";

                this.Select(Text.Length, 0);
                e.Handled = true;
            }
        }


        void ItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is ListBoxItem)
            {

                ListBoxItem lbItem = e.OriginalSource as ListBoxItem;

                e.Handled = true;
                switch (e.Key)
                {
                    case Key.Enter:
                        Text = (lbItem.Content as Suggestion).Value; updateSource();
                        RaiseEvent(new RoutedEventArgs(AutoCompleteTextBoxBase.SourceUpdatedEvent));
                        break;
                    //12-25-08 - added "\" support when picking in list view
                    case Key.Oem5:
                        Text = (lbItem.Content as Suggestion).Value + "\\";
                        break;
                    //12-25-08 - roll back if escape is pressed
                    case Key.Escape:
                        Text = lastPath.TrimEnd('\\') + "\\";
                        break;
                    default: e.Handled = false; break;
                }
                //12-25-08 - Force focus back the control after selected.
                if (e.Handled)
                {
                    Keyboard.Focus(this);
                    IsPopupOpened = false;
                    this.Select(Text.Length, 0); //Select last char
                }
            }
        }


        void AutoCompleteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IsPopupOpened = false;
                updateSource();
                RaiseEvent(new RoutedEventArgs(AutoCompleteTextBoxBase.SourceUpdatedEvent));
                e.Handled = true;
            }


        }

        void updateSource()
        {
            if (this.GetBindingExpression(TextBox.TextProperty) != null)
                this.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        void ItemList_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TextBlock tb = e.OriginalSource as TextBlock;
                if (tb != null)
                {
                    Text = tb.Text;
                    updateSource();
                    IsPopupOpened = false;
                    e.Handled = true;
                }
            }
        }

        public static string GetDirectoryName(string path)
        {
            if (path.EndsWith("\\"))
                return path;
            //path = path.Substring(0, path.Length - 1); //Remove ending slash.

            int idx = path.LastIndexOf('\\');
            if (idx == -1)
                return "";
            return path.Substring(0, idx);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (_loaded && Visibility == System.Windows.Visibility.Visible)
            {
                lastPath = GetDirectoryName(this.Text);
                string curPath = this.Text;

                //_bgWorkerUpdateSuggestion.RunWorkerAsync();
                if (_searchThread != null && _searchThread.ThreadState == System.Threading.ThreadState.Running)
                    _searchThread.Abort();

                _searchThread = new Thread(new ThreadStart(delegate
                {
                    try
                    {
                        IEnumerator<Suggestion> suggestions = Lookup(curPath).GetEnumerator();

                        if (Thread.CurrentThread.ThreadState == System.Threading.ThreadState.Running)
                            Application.Current.Dispatcher.Invoke(DispatcherPriority.ContextIdle,
                            new ThreadStart(delegate
                            {
                                _suggestions.Clear();
                                while (suggestions.MoveNext() && this.Text == curPath)
                                {
                                    if (!(String.Equals(suggestions.Current.Header, this.Text, StringComparison.CurrentCultureIgnoreCase)))
                                        _suggestions.Add(suggestions.Current);
                                }
                                IsPopupOpened = this.IsKeyboardFocusWithin && _suggestions.Count > 0;
                            }));

                    }
                    catch { }



                }));

                _searchThread.Start();

                //try
                //{
                //    //if (lastPath != Path.GetDirectoryName(this.Text))
                //    //if (textBox.Text.EndsWith("\\"))                        
                //    {



                //    }


                //    Popup.IsOpen = ItemList.Items.Count > 0;

                //    //ItemList.Items.Filter = p =>
                //    //{
                //    //    string path = p as string;
                //    //    return path.StartsWith(this.Text, StringComparison.CurrentCultureIgnoreCase) &&
                //    //        !(String.Equals(path, this.Text, StringComparison.CurrentCultureIgnoreCase));
                //    //};
                //}
                //catch
                //{

                //}
            }
        }



        protected virtual IEnumerable<Suggestion> Lookup(string path)
        {
            
            if (Directory.Exists(Path.GetDirectoryName(path)))
            {
                DirectoryInfo lookupFolder = new DirectoryInfo(Path.GetDirectoryName(path));
                if (lookupFolder != null)
                {
                    DirectoryInfo[] AllItems = lookupFolder.GetDirectories();
                    foreach (DirectoryInfo di in AllItems)
                    {
                        if (di.FullName.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                            yield return new Suggestion("dir", di.FullName);
                    }
                    //(from di in AllItems where di.FullName.StartsWith(path, 
                    //     StringComparison.CurrentCultureIgnoreCase) select new Suggestion("Dir", di.FullName)).ToArray();
                }
            }

        }

        #endregion

        #region Data
        private ObservableCollection<Suggestion> _suggestions = new ObservableCollection<Suggestion>();
        private Thread _searchThread = null;
        #endregion

        #region Public Properties

        public ObservableCollection<Suggestion> Suggestions
        {
            get { return _suggestions; }
        }

        public static readonly RoutedEvent SourceUpdatedEvent = EventManager.RegisterRoutedEvent("SourceUpdated",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AutoCompleteTextBoxBase));

        public event RoutedEventHandler SourceUpdated
        {
            add { AddHandler(SourceUpdatedEvent, value); }
            remove { RemoveHandler(SourceUpdatedEvent, value); }
        }               

        public object DropDownPlacementTarget
        {
            get { return (object)GetValue(DropDownPlacementTargetProperty); }
            set { SetValue(DropDownPlacementTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDownPlacementTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropDownPlacementTargetProperty =
            DependencyProperty.Register("DropDownPlacementTarget", typeof(object), typeof(AutoCompleteTextBoxBase));



        public bool IsPopupOpened
        {
            get { return (bool)GetValue(IsPopupOpenedProperty); }
            set { SetValue(IsPopupOpenedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPopupOpened.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPopupOpenedProperty =
            DependencyProperty.Register("IsPopupOpened", typeof(bool), typeof(AutoCompleteTextBoxBase), new UIPropertyMetadata(false));

        



        #endregion
    }
}
