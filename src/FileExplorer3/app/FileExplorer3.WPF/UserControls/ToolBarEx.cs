using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.UserControls
{
    public class ToolbarEx : Menu
    {
        #region Constructor

        static ToolbarEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarEx), new FrameworkPropertyMetadata(typeof(ToolbarEx)));
        }

        public ToolbarEx()
        {
            
        }

        #endregion

        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolbarItemEx();
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion

    }

    public enum ToolbarItemType { Button, Menu, MenuButton, Combo, Check, ComboItem, CheckItem, Separator }    

    public class ToolbarItemEx : MenuItem
    {
        #region Constructor

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
         
            this.AddHandler(MenuItem.CheckedEvent, (RoutedEventHandler)((o, e) =>
                {
                    switch (this.HeaderType)
                    {
                        case ToolbarItemType.Combo :
                            foreach (var item in Items)
                            {
                                var subToolbarItem = item as ToolbarSubItemEx;
                                if (subToolbarItem != null)
                                    subToolbarItem.IsChecked = subToolbarItem.Equals(e.Source);
                            }
                            break;

                    }
                    //Debug.WriteLine("Checked");
                }));

            this.AddHandler(Thumb.DragDeltaEvent, (DragDeltaEventHandler)((o, e) =>
                {
                    double newValue = this.SliderValue - (e.VerticalChange / 100);
                    if (newValue > SliderMaximum)
                        newValue = SliderMaximum;
                    if (newValue < SliderMinimum)
                        newValue = SliderMinimum;

                    if (e.VerticalChange < 0)
                        this.SetValue(SliderValueProperty, Math.Ceiling(newValue));
                    else this.SetValue(SliderValueProperty, Math.Truncate(newValue));                    
                }));

            this.AddHandler(MenuItem.ClickEvent, (RoutedEventHandler)((o, e) =>
                {                    
                    if (this.HeaderType == ToolbarItemType.MenuButton && this.IsSliderEnabled)
                    {
                        ToolbarSubItemEx sourceItem = UITools.FindAncestor<ToolbarSubItemEx>(e.OriginalSource as UIElement);
                        if (sourceItem != null && sourceItem.Value != null)
                        {
                            double value;
                            if (double.TryParse(sourceItem.Value.ToString(), out value))
                                this.SetValue(SliderValueProperty, value);
                        }
                    }

                }));
            
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolbarSubItemEx();
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties


        public Brush InnerBorderBrush
        {
            get { return (Brush)GetValue(InnerBorderBrushProperty); }
            set { SetValue(InnerBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty InnerBorderBrushProperty =
            DependencyProperty.Register("InnerBorderBrush", typeof(Brush),
            typeof(ToolbarItemEx), new PropertyMetadata(SystemColors.ActiveBorderBrush));
        


        /// <summary>
        /// Lookup from http://www.adamdawes.com/windows8/win8_segoeuisymbol.html
        /// </summary>
        public string Symbol
        {
            get { return (string)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol", typeof(string),
            typeof(ToolbarItemEx), new PropertyMetadata(""));



        public static readonly DependencyProperty IsHeaderVisibleProperty =
            DependencyProperty.Register("IsHeaderVisible", typeof(bool), typeof(ToolbarItemEx),
            new UIPropertyMetadata(true));

        public bool IsHeaderVisible
        {
            get { return (bool)GetValue(IsHeaderVisibleProperty); }
            set { SetValue(IsHeaderVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsSeparatorProperty =
            DependencyProperty.Register("IsSeparator", typeof(bool), typeof(ToolbarItemEx),
            new UIPropertyMetadata(false));

        public bool IsSeparator
        {
            get { return (bool)GetValue(IsSeparatorProperty); }
            set { SetValue(IsSeparatorProperty, value); }
        }

        #region Slider related

        public static readonly DependencyProperty IsSliderEnabledProperty =
            DependencyProperty.Register("IsSliderEnabled", typeof(bool), typeof(ToolbarItemEx),
            new UIPropertyMetadata(false));

        public bool IsSliderEnabled
        {
            get { return (bool)GetValue(IsSliderEnabledProperty); }
            set { SetValue(IsSliderEnabledProperty, value); }
        }

        public static readonly DependencyProperty SliderMaximumProperty =
            DependencyProperty.Register("SliderMaximum", typeof(double), typeof(ToolbarItemEx),
            new UIPropertyMetadata(1000.0d));

        public double SliderMaximum
        {
            get { return (double)GetValue(SliderMaximumProperty); }
            set { SetValue(SliderMaximumProperty, value); }
        }

        public static readonly DependencyProperty SliderMinimumProperty =
            DependencyProperty.Register("SliderMinimum", typeof(double), typeof(ToolbarItemEx),
            new UIPropertyMetadata(0.0d));

        public double SliderMinimum
        {
            get { return (double)GetValue(SliderMinimumProperty); }
            set { SetValue(SliderMinimumProperty, value); }
        }

        public static readonly DependencyProperty SliderValueProperty =
                    DependencyProperty.Register("SliderValue", typeof(double), typeof(ToolbarItemEx),
                    new UIPropertyMetadata(new PropertyChangedCallback(delegate { /*Debug.WriteLine("Changed-ToolbarMenuItem");*/ })));


        public static readonly DependencyProperty StepsProperty =
            DependencyProperty.RegisterAttached("Steps", typeof(ObservableCollection<Step>), typeof(ToolbarItemEx));

        public static ObservableCollection<Step> GetSelectionAdorner(DependencyObject target)
        {
            return (ObservableCollection<Step>)target.GetValue(StepsProperty);
        }

        public static void SetSelectionAdorner(DependencyObject target, ObservableCollection<Step> value)
        {
            target.SetValue(StepsProperty, value);
        }

        public double SliderValue
        {
            get { return (double)GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }


        //public static readonly DependencyProperty ItemHeightProperty =
        //             DependencyProperty.Register("ItemHeight", typeof(double), typeof(ToolbarItemEx),
        //             new UIPropertyMetadata());


        #endregion




        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object),
            typeof(ToolbarItemEx), new PropertyMetadata(null));


        public bool IsHeaderAlignRight
        {
            get { return (bool)GetValue(IsHeaderAlignRightProperty); }
            set { SetValue(IsHeaderAlignRightProperty, value); }
        }

        public static readonly DependencyProperty IsHeaderAlignRightProperty =
            DependencyProperty.Register("IsHeaderAlignRight", typeof(bool),
            typeof(ToolbarItemEx), new PropertyMetadata(false));


        public ToolbarItemType HeaderType
        {
            get { return (ToolbarItemType)GetValue(HeaderTypeProperty); }
            set { SetValue(HeaderTypeProperty, value); }
        }

        public static readonly DependencyProperty HeaderTypeProperty =
            DependencyProperty.Register("HeaderType", typeof(ToolbarItemType),
            typeof(ToolbarItemEx), new PropertyMetadata(ToolbarItemType.Button));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
            typeof(ToolbarItemEx), new PropertyMetadata(new CornerRadius(0)));


        #endregion
    }


    public class ToolbarSubItemEx : ToolbarItemEx 
    {

        #region Constructor

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate(); 
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties


        public static readonly DependencyProperty IsSeparatorProperty =
           ToolbarItemEx.IsSeparatorProperty.AddOwner(typeof(ToolbarSubItemEx));

        public bool IsSeparator
        {
            get { return (bool)GetValue(IsSeparatorProperty); }
            set { SetValue(IsSeparatorProperty, value); }
        }

        /// <summary>
        /// Lookup from http://www.adamdawes.com/windows8/win8_segoeuisymbol.html
        /// </summary>
        public string Symbol
        {
            get { return (string)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }

        public static readonly DependencyProperty SymbolProperty =
           ToolbarItemEx.SymbolProperty.AddOwner(typeof(ToolbarSubItemEx));

        public ToolbarItemType HeaderType
        {
            get { return (ToolbarItemType)GetValue(HeaderTypeProperty); }
            set { SetValue(HeaderTypeProperty, value); }
        }

        public static readonly DependencyProperty HeaderTypeProperty =
            DependencyProperty.Register("HeaderType", typeof(ToolbarItemType),
            typeof(ToolbarSubItemEx), new PropertyMetadata(ToolbarItemType.Button));


        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object),
            typeof(ToolbarSubItemEx), new PropertyMetadata(null));

        //public double ExtraHeight
        //{
        //    get { return (double)GetValue(ExtraHeightProperty); }
        //    set { SetValue(ExtraHeightProperty, ExtraHeight); }
        //}

        //public static readonly DependencyProperty ExtraHeightProperty =
        //    DependencyProperty.Register("ExtraHeight", typeof(double),
        //    typeof(ToolbarSubItemEx), new PropertyMetadata(0d));



        public static readonly DependencyProperty IsStepStopProperty =
                    DependencyProperty.Register("IsStepStop", typeof(bool), typeof(ToolbarSubItemEx),
                    new UIPropertyMetadata(false));

        public bool IsStepStop
        {
            get { return (bool)GetValue(IsStepStopProperty); }
            set { SetValue(IsStepStopProperty, value); }
        }


        #endregion

    }
}
