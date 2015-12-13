using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Navigation;

namespace QuickZip.UserControls
{
    public class BindableWebBrowser : ContentControl
    {

        public BindableWebBrowser()
        {
            Content = _browser = new WebBrowser();
            _browser.Navigating += (NavigatingCancelEventHandler) ((o,e) =>
            {
                Source = e.Uri;  
            });
        }



        #region Methods




        public static void OnSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            BindableWebBrowser bindWebBrowser = o as BindableWebBrowser;
            if (bindWebBrowser != null && !e.NewValue.Equals(e.OldValue as Uri))
            {
                if (bindWebBrowser._browser.Source != e.NewValue as Uri) 
                    bindWebBrowser._browser.Source = e.NewValue as Uri;
            }
        }

        #endregion

        #region Data

        private WebBrowser _browser;

        #endregion


        #region  Public Properties




        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri),
            typeof(BindableWebBrowser), new UIPropertyMetadata(new PropertyChangedCallback(OnSourceChanged)));





        #endregion


    }
}
