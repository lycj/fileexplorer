using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace QuickZip.UserControls
{
    public class NavigatorBase : ComboBox
    {
        #region Constructor

        static NavigatorBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigatorBase), new FrameworkPropertyMetadata(typeof(NavigatorBase)));
        }

        public NavigatorBase() : base()
        {            
            this.AddHandler(Button.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                if (e.OriginalSource is Button)
                    switch ((e.OriginalSource as Button).Name)
                    {
                        case "btnPrev":
                            RaiseEvent(new RoutedEventArgs(GoPrevEvent));
                            break;
                        case "btnNext":
                            RaiseEvent(new RoutedEventArgs(GoNextEvent));
                            break;
                    }
            });

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
            return new NavigatorItem();
        }

        #endregion

        #region Public Properties

        public static readonly RoutedEvent GoPrevEvent = EventManager.RegisterRoutedEvent("GoPrev",
           RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigatorBase));

        public event RoutedEventHandler GoPrev
        {
            add { AddHandler(GoPrevEvent, value); }
            remove { RemoveHandler(GoPrevEvent, value); }
        }

        public static readonly RoutedEvent GoNextEvent = EventManager.RegisterRoutedEvent("GoNext",
          RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigatorBase));

        public event RoutedEventHandler GoNext
        {
            add { AddHandler(GoNextEvent, value); }
            remove { RemoveHandler(GoNextEvent, value); }
        }

        public static readonly DependencyProperty CanGoPrevProperty = DependencyProperty.Register("CanGoPrev", typeof(bool),
            typeof(NavigatorBase), new UIPropertyMetadata(false));

        public bool CanGoPrev
        {
            get { return (bool)GetValue(CanGoPrevProperty); }
            set { SetValue(CanGoPrevProperty, value); }
        }

        public static readonly DependencyProperty CanGoNextProperty = DependencyProperty.Register("CanGoNext", typeof(bool),
            typeof(NavigatorBase), new UIPropertyMetadata(false));

        public bool CanGoNext
        {
            get { return (bool)GetValue(CanGoNextProperty); }
            set { SetValue(CanGoNextProperty, value); }
        }

        #endregion
    }
}
