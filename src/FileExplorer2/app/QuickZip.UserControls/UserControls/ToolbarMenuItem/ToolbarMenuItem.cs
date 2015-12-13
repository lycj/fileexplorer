using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;

namespace QuickZip.UserControls
{
    public class ToolbarMenuItem : MenuItem
    {

        static ToolbarMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolbarMenuItem), new FrameworkPropertyMetadata(typeof(ToolbarMenuItem)));
        }

        public ToolbarMenuItem()
            : base()
        {            
            this.AddHandler(Button.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                ////buttonContent clicked, see ToolbarTemplates.xaml
                //if (Command != null)
                //{
                //    Command.Execute(null);
                //    e.Handled = true;
                //}                
            });

            this.AddHandler(ToolbarMenuItem.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                if (IsSliderEnabled)
                    if (e.OriginalSource is ToolbarMenuItem)
                    {
                        ToolbarMenuItem item = (ToolbarMenuItem)e.OriginalSource;
                        SliderValue = item.SliderStep;
                    }
            });

            //Template = (ControlTemplate)this.TryFindResource("TopLevelItemTemplateKey");
            this.AddHandler(ToolbarMenuItem.LoadedEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                if (this.Role == MenuItemRole.TopLevelItem)
                    Template = (ControlTemplate)this.TryFindResource("TopLevelItemTemplateKey");
            });

            //Monitor Command Changed
            DependencyPropertyDescriptor isHeaderToggledescriptor = DependencyPropertyDescriptor.FromProperty
                (MenuItem.CommandProperty, typeof(ToolbarMenuItem));
            isHeaderToggledescriptor.AddValueChanged(this, new EventHandler(delegate
            {
                IsHeaderTogglePopup = Command == null;
            }));

            DependencyPropertyDescriptor roleDescriptor = DependencyPropertyDescriptor.FromProperty(MenuItem.RoleProperty, typeof(ToolbarMenuItem));
            roleDescriptor.AddValueChanged(this, new EventHandler(delegate
            {
                    switch (this.Role)
                    {
                        case MenuItemRole.TopLevelItem:
                            Template = (ControlTemplate)this.TryFindResource("TopLevelItemTemplateKey");
                            break;
                        case MenuItemRole.TopLevelHeader:
                            Template = (ControlTemplate)this.TryFindResource("TopLevelHeaderTemplateKey");
                            break;
                        case MenuItemRole.SubmenuItem:
                            Template = (ControlTemplate)this.TryFindResource("SubmenuItemTemplateKey");
                            break;
                        case MenuItemRole.SubmenuHeader:
                            Template = (ControlTemplate)this.TryFindResource("SubmenuHeaderTemplateKey");
                            break;
                    }
            }));
        }

        #region Methods
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolbarMenuItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Border border = this.Template.FindName("Border", this) as Border;
            if (border != null)
                HeaderHeight = border.ActualHeight;
        }


        private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ToolbarMenuItem item = (ToolbarMenuItem)sender;
            item.IsHeaderTogglePopup = (e.NewValue != null);
        }

        #endregion

        #region Public Properties



        public double HeaderHeight
        {
            get;
            private set;
        }


        public static readonly DependencyProperty IsSeparatorProperty =
            DependencyProperty.Register("IsSeparator", typeof(bool), typeof(ToolbarMenuItem),
            new UIPropertyMetadata(false));

        public bool IsSeparator
        {
            get { return (bool)GetValue(IsSeparatorProperty); }
            set { SetValue(IsSeparatorProperty, value); }
        }

        internal static readonly DependencyProperty IsHeaderTogglePopupProperty =
            DependencyProperty.Register("IsHeaderTogglePopup", typeof(bool), typeof(ToolbarMenuItem),
            new UIPropertyMetadata(true));

        internal bool IsHeaderTogglePopup
        {
            get { return (bool)GetValue(IsHeaderTogglePopupProperty); }
            set { SetValue(IsHeaderTogglePopupProperty, value); }
        }



        //Slider related

        public static readonly DependencyProperty IsSliderEnabledProperty =
            DependencyProperty.Register("IsSliderEnabled", typeof(bool), typeof(ToolbarMenuItem),
            new UIPropertyMetadata(false));

        public bool IsSliderEnabled
        {
            get { return (bool)GetValue(IsSliderEnabledProperty); }
            set { SetValue(IsSliderEnabledProperty, value); }
        }

        public static readonly DependencyProperty SliderMaximumProperty =
            DependencyProperty.Register("SliderMaximum", typeof(double), typeof(ToolbarMenuItem),
            new UIPropertyMetadata(1000.0d));

        public double SliderMaximum
        {
            get { return (double)GetValue(SliderMaximumProperty); }
            set { SetValue(SliderMaximumProperty, value); }
        }

        public static readonly DependencyProperty SliderMinimumProperty =
            DependencyProperty.Register("SliderMinimum", typeof(double), typeof(ToolbarMenuItem),
            new UIPropertyMetadata(0.0d));

        public double SliderMinimum
        {
            get { return (double)GetValue(SliderMinimumProperty); }
            set { SetValue(SliderMinimumProperty, value); }
        }

        public static readonly DependencyProperty SliderValueProperty =
                    DependencyProperty.Register("SliderValue", typeof(double), typeof(ToolbarMenuItem),
                    new UIPropertyMetadata(new PropertyChangedCallback(delegate { /*Debug.WriteLine("Changed-ToolbarMenuItem");*/ })));

        public double SliderValue
        {
            get { return (double)GetValue(SliderValueProperty); }
            set { SetValue(SliderValueProperty, value); }
        }

        public static readonly DependencyProperty SliderStepProperty =
                     DependencyProperty.Register("SliderStep", typeof(double), typeof(ToolbarMenuItem),
                     new UIPropertyMetadata(0.0d));

        public double SliderStep
        {
            get { return (double)GetValue(SliderStepProperty); }
            set { SetValue(SliderStepProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
                     DependencyProperty.Register("ItemHeight", typeof(double), typeof(ToolbarMenuItem),
                     new UIPropertyMetadata());

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }


        public static readonly DependencyProperty IsStepStopProperty =
                     DependencyProperty.Register("IsStepStop", typeof(bool), typeof(ToolbarMenuItem),
                     new UIPropertyMetadata(false));

        public bool IsStepStop
        {
            get { return (bool)GetValue(IsStepStopProperty); }
            set { SetValue(IsStepStopProperty, value); }
        }


        
        

        #endregion

    }
}
