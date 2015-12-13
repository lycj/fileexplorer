using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public class Navigator2 : ComboBox
    {
        #region Constructor
  
        public Navigator2() : base()
        {            
            //this.AddHandler(Button.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            //{
            //    if (e.OriginalSource is Button)
            //        switch ((e.OriginalSource as Button).Name)
            //        {
            //            case "btnPrev":
            //                RaiseEvent(new RoutedEventArgs(GoBackEvent));
            //                break;
            //            case "btnNext":
            //                RaiseEvent(new RoutedEventArgs(GoNextEvent));
            //                break;
            //        }
            //});

            //this.AddHandler(NavigatorItem.NavigateEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            //{
            //    if (e.OriginalSource is NavigatorItem)
            //    {
            //        OnNavigate(Items.IndexOf((e.OriginalSource as NavigatorItem).DataContext));
            //    }

            //});

        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {            
            return new NavigatorItem2();
        }

        #endregion

        #region Public Properties

        public ICommand GoBackCommand
        {
            get { return (ICommand)GetValue(GoBackCommandProperty); }
            set { SetValue(GoBackCommandProperty, value); }
        }
        
        public static readonly DependencyProperty GoBackCommandProperty =
            DependencyProperty.Register("GoBackCommand", typeof(ICommand), typeof(Navigator2));


        public ICommand GoNextCommand
        {
            get { return (ICommand)GetValue(GoNextCommandProperty); }
            set { SetValue(GoNextCommandProperty, value); }
        }        
        public static readonly DependencyProperty GoNextCommandProperty =
            DependencyProperty.Register("GoNextCommand", typeof(ICommand), typeof(Navigator2));

        //public static readonly RoutedEvent GoBackEvent = EventManager.RegisterRoutedEvent("GoBack",
        //   RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Navigator2));

        //public event RoutedEventHandler GoBack
        //{
        //    add { AddHandler(GoBackEvent, value); }
        //    remove { RemoveHandler(GoBackEvent, value); }
        //}

        //public static readonly RoutedEvent GoNextEvent = EventManager.RegisterRoutedEvent("GoNext",
        //  RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Navigator2));

        //public event RoutedEventHandler GoNext
        //{
        //    add { AddHandler(GoNextEvent, value); }
        //    remove { RemoveHandler(GoNextEvent, value); }
        //}

        //public static readonly DependencyProperty CanGoBackProperty = DependencyProperty.Register("CanGoBack", typeof(bool),
        //    typeof(Navigator2), new UIPropertyMetadata(false));

        //public bool CanGoBack
        //{
        //    get { return (bool)GetValue(CanGoBackProperty); }
        //    set { SetValue(CanGoBackProperty, value); }
        //}

        //public static readonly DependencyProperty CanGoNextProperty = DependencyProperty.Register("CanGoNext", typeof(bool),
        //    typeof(Navigator2), new UIPropertyMetadata(false));

        //public bool CanGoNext
        //{
        //    get { return (bool)GetValue(CanGoNextProperty); }
        //    set { SetValue(CanGoNextProperty, value); }
        //}

        #endregion
    }
}
