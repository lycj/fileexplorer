using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace QuickZip.UserControls
{
    public class Statusbar2 : ItemsControl
    {
         #region Constructor

        public Statusbar2()
        {
            
        }

        #endregion

        #region Methods

        

        #endregion

        #region Dependency Properties



        public object NotificationBar
        {
            get { return (object)GetValue(NotificationBarProperty); }
            set { SetValue(NotificationBarProperty, value); }
        }
        
        public static readonly DependencyProperty NotificationBarProperty =
            DependencyProperty.Register("NotificationBar", typeof(object), typeof(Statusbar2), new UIPropertyMetadata());

        

        

        public static readonly RoutedEvent EnterSimpleStatusbarEvent = 
            EventManager.RegisterRoutedEvent("EnterSimpleStatusbar", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Statusbar2));

        public event RoutedEventHandler EnterSimpleStatusbar
        {
            add { AddHandler(EnterSimpleStatusbarEvent, value); }
            remove { RemoveHandler(EnterSimpleStatusbarEvent, value); }
        }


        public static readonly RoutedEvent ExitSimpleStatusbarEvent =
            EventManager.RegisterRoutedEvent("ExitSimpleStatusbar", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Statusbar2));

        public event RoutedEventHandler ExitSimpleStatusbar
        {
            add { AddHandler(ExitSimpleStatusbarEvent, value); }
            remove { RemoveHandler(ExitSimpleStatusbarEvent, value); }
        }

        public static DependencyProperty IconSourceProperty = DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(Statusbar2));

        public ImageSource IconSource 
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }



        public bool IsSimpleStatusbar
        {
            get { return (bool)GetValue(IsSimpleStatusbarProperty); }
            set { SetValue(IsSimpleStatusbarProperty, value); }
        }
        
        public static readonly DependencyProperty IsSimpleStatusbarProperty =
            DependencyProperty.Register("IsSimpleStatusbar", typeof(bool), typeof(Statusbar2), 
            new UIPropertyMetadata(
                (o,e) => 
                {
                    if (e.NewValue != e.OldValue)
                    {
                        Statusbar2 sbar = o as Statusbar2;
                        if ((bool)e.NewValue == true)
                            sbar.RaiseEvent(new RoutedEventArgs(EnterSimpleStatusbarEvent));
                        else sbar.RaiseEvent(new RoutedEventArgs(ExitSimpleStatusbarEvent));
                    }
                }));

        

        public DataTemplate ItemIconTemplate
        {
            get { return (DataTemplate)GetValue(ItemIconTemplateProperty); }
            set { SetValue(ItemIconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemIconTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemIconTemplateProperty =
            DependencyProperty.Register("ItemIconTemplate", typeof(DataTemplate), typeof(Statusbar2));

        

        #endregion
    }
}
