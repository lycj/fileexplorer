///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace QuickZip.UserControls
{
    public class NavigatorItem : ComboBoxItem
    {
        static NavigatorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigatorItem), new FrameworkPropertyMetadata(typeof(NavigatorItem)));
        }

        public NavigatorItem()
        {
            this.AddHandler(ComboBox.SelectedEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                this.IsSelected = true;
                RaiseEvent(new RoutedEventArgs(NavigateEvent));
            });
        }


        #region Methods



        #endregion


        #region Public Properties

        public static readonly RoutedEvent NavigateEvent = EventManager.RegisterRoutedEvent("Navigate",
           RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigatorItem));

        public event RoutedEventHandler Navigate
        {
            add { AddHandler(NavigateEvent, value); }
            remove { RemoveHandler(NavigateEvent, value); }
        }


        #endregion


        #region Data


        #endregion
    }
}
