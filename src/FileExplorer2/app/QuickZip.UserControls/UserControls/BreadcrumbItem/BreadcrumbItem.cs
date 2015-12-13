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
    public class BreadcrumbItem : HeaderedItemsControl
    {
        static BreadcrumbItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BreadcrumbItem), new FrameworkPropertyMetadata(typeof(BreadcrumbItem)));
        }        

        public BreadcrumbItem()
        {
            this.Loaded += delegate { _loaded = true; };       
            
        }

        public void raiseShowCaptionEvent(bool value)
        {
            //Fix:69: http://social.msdn.microsoft.com/forums/en-US/wpf/thread/6ec60f31-5a6f-486e-a4ac-309505987735/
            //sometimes that element genuinely isn't there yet. (after loaded event / BreadcrumbItem.cs)
            try
            {
                if (value)
                    RaiseEvent(new RoutedEventArgs(ShowingCaptionEvent));
                else RaiseEvent(new RoutedEventArgs(HidingCaptionEvent));
            }
            catch
            {

            }
        }

        public static object OnShowCaptionCoerce(DependencyObject obj, object value)
        {
            BreadcrumbItem item = (BreadcrumbItem)obj;
            if (item._loaded)
            {
                item.raiseShowCaptionEvent((bool)value);
            }
            else
            {
                RoutedEventHandler action = null;
                action = (RoutedEventHandler)delegate
                 {
                     item.Loaded -= action;
                     if (!item._showCaptionHandled && !(bool)value)
                         item.raiseShowCaptionEvent((bool)value);
                     item._showCaptionHandled = true;
                 };
                item.Loaded += action;
            }
            return value;
        }


        protected override DependencyObject GetContainerForItemOverride()
        {
            BreadcrumbItem retVal = new BreadcrumbItem();
            retVal.ShowToggle = false;
            retVal.IsTopLevel = false;
            return retVal;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!ShowCaption)
                raiseShowCaptionEvent(ShowCaption);
            this.AddHandler(Button.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
            {
                if ((args.OriginalSource is Button))
                    RaiseEvent(new RoutedEventArgs(SelectedEvent));
                args.Handled = true;
            });
        }

        #region Public Properties

        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected",
           RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbItem));

        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        public static readonly RoutedEvent ShowingCaptionEvent = EventManager.RegisterRoutedEvent("ShowingCaption",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbItem));

        public event RoutedEventHandler ShowingCaption
        {
            add { AddHandler(ShowingCaptionEvent, value); }
            remove { RemoveHandler(ShowingCaptionEvent, value); }
        }

        public static readonly RoutedEvent HidingCaptionEvent = EventManager.RegisterRoutedEvent("HidingCaption",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreadcrumbItem));

        public event RoutedEventHandler HidingCaption
        {
            add { AddHandler(HidingCaptionEvent, value); }
            remove { RemoveHandler(HidingCaptionEvent, value); }
        }

        public static readonly DependencyProperty ShowCaptionProperty =
                    DependencyProperty.Register("ShowCaption", typeof(bool), typeof(BreadcrumbItem),
                    new UIPropertyMetadata(true, null, new CoerceValueCallback(OnShowCaptionCoerce)));

        /// <summary>
        /// Display Caption
        /// </summary>
        public bool ShowCaption
        {
            get { return (bool)GetValue(ShowCaptionProperty); }
            set { SetValue(ShowCaptionProperty, value); }
        }

        public static readonly DependencyProperty ShowToggleProperty =
                    DependencyProperty.Register("ShowToggle", typeof(bool), typeof(BreadcrumbItem),
                    new UIPropertyMetadata(true));

        /// <summary>
        /// Display Toggle
        /// </summary>
        public bool ShowToggle
        {
            get { return (bool)GetValue(ShowToggleProperty); }
            set { SetValue(ShowToggleProperty, value); }
        }

        public static readonly DependencyProperty IsTopLevelProperty =
                    DependencyProperty.Register("IsTopLevel", typeof(bool), typeof(BreadcrumbItem),
                    new UIPropertyMetadata(true));

        /// <summary>
        /// IsTopLevel?
        /// </summary>
        public bool IsTopLevel
        {
            get { return (bool)GetValue(IsTopLevelProperty); }
            set { SetValue(IsTopLevelProperty, value); }
        }

        public static readonly DependencyProperty IsShadowItemProperty =
                   DependencyProperty.Register("IsShadowItem", typeof(bool), typeof(BreadcrumbItem),
                   new UIPropertyMetadata(true));

        /// <summary>
        /// For 1st level BreadcrumbItem, grey color if true.
        /// </summary>
        public bool IsShadowItem
        {
            get { return (bool)GetValue(IsShadowItemProperty); }
            set { SetValue(IsShadowItemProperty, value); }
        }

        public static readonly DependencyProperty IsSeparatorProperty =
                   DependencyProperty.Register("IsSeparator", typeof(bool), typeof(BreadcrumbItem),
                   new UIPropertyMetadata(false));

        /// <summary>
        /// Display separator, use for 2nd level BreadcrumbItem only.
        /// </summary>
        public bool IsSeparator
        {
            get { return (bool)GetValue(IsSeparatorProperty); }
            set { SetValue(IsSeparatorProperty, value); }
        }

        public static readonly DependencyProperty IsDropDownOpenProperty =
            ComboBox.IsDropDownOpenProperty.AddOwner(typeof(BreadcrumbItem), 
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownChanged)));

        public static void OnIsDropDownChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            Debug.WriteLine("DropDown Changed");
        }

        /// <summary>
        /// Is current dropdown (combobox) opened
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        public static readonly DependencyProperty IsItemVisibleProperty =
           DependencyProperty.Register("IsItemVisible", typeof(bool), typeof(BreadcrumbItem), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Whether the BreadcrumbItem is visible in breadcrumb bar, otherwise they should display in first BreadcrumbItem's Items
        /// </summary>
        public bool IsItemVisible
        {
            get { return (bool)GetValue(IsItemVisibleProperty); }
            set { SetValue(IsItemVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
           DependencyProperty.Register("IsLoading", typeof(bool), typeof(BreadcrumbItem), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Display loading animation if isloading
        /// </summary>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        #endregion


        #region Data

        bool _loaded = false;
        bool _showCaptionHandled = false;

        #endregion
    }
}
