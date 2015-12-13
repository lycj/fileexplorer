using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Markup;

namespace QuickZip.UserControls
{

    [ContentProperty("Content")]
    public class Breadcrumb2 : ItemsControl
    {
        public Breadcrumb2()
        {

        }


        #region Methods

        private void SetupAnimations()
        {
            FadeStoryBoard = new Storyboard();
            DoubleAnimation fadeAnimation = new DoubleAnimation(0.7, new Duration(TimeSpan.FromSeconds(0.2)));
            Storyboard.SetTarget(fadeAnimation, this);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("Opacity"));
            FadeStoryBoard.Children.Add(fadeAnimation);

            UnfadeStoryBoard = new Storyboard();
            DoubleAnimation unfadeAnimation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.2)));
            Storyboard.SetTarget(unfadeAnimation, this);
            Storyboard.SetTargetProperty(unfadeAnimation, new PropertyPath("Opacity"));
            UnfadeStoryBoard.Children.Add(unfadeAnimation);

            //RootModel.PropertyChanged += (PropertyChangedEventHandler)delegate(object sender, PropertyChangedEventArgs e)
            //{
            //    //if (this.IsFocused)
            //    switch (e.PropertyName)
            //    {
            //        case "IsBreadcrumbVisible":
            //            if (!RootModel.IsBreadcrumbVisible)
            //                UnfadeStoryBoard.Begin();
            //            break;
            //    }

            //};

            tbox.GotFocus += delegate { UnfadeStoryBoard.Begin(); };
            this.MouseEnter += delegate { UnfadeStoryBoard.Begin(); };

            tbox.LostFocus += delegate { if (!tbox.IsMouseOver && EnableAutoFade) FadeStoryBoard.Begin(); };
            this.MouseLeave += delegate
            {
                if (EnableAutoFade && (IsBreadcrumbVisible
                     || !tbox.IsFocused))
                    FadeStoryBoard.Begin();
            };

            this.AddHandler(AutoCompleteTextBoxBase.SourceUpdatedEvent,
                (RoutedEventHandler)delegate
                {
                    if (Validation.GetErrors(tbox).Count == 0)
                    {
                        FadeStoryBoard.Begin();
                        ToggleTextBoxVisibility(false);
                    }

                });


        }

        public void ToggleTextBoxVisibility(bool visible)
        {
            if (btnToggle.IsChecked == visible && IsBreadcrumbEnabled)
            {
                ToggleButtonAutomationPeer bap = new ToggleButtonAutomationPeer(btnToggle);
                IToggleProvider iip = bap.GetPattern(PatternInterface.Toggle) as IToggleProvider;
                iip.Toggle();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();


            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            btnToggle = (ToggleButton)this.Template.FindName("btnToggle", this);
            bcoreCM = (ContextMenu)this.Template.FindName("bcoreCM", this);
            tbox = (TextBox)this.Template.FindName("tbox", this);
            BreadcrumbBackgroundGrid = (Grid)this.Template.FindName("BreadcrumbBackgroundGrid", this);

           

            this.AddHandler(BreadcrumbItem.SelectedEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                BreadcrumbItem item = (BreadcrumbItem)e.OriginalSource;
                SelectedValue = (item.DataContext);
                e.Handled = true;
            });

            this.AddHandler(Button.ClickEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                if (e.OriginalSource.Equals(btnToggle))
                {
                    //RootModel.IsBreadcrumbVisible = !RootModel.IsBreadcrumbVisible;
                    if (!btnToggle.IsChecked == true)
                    {
                        tbox.Focus();
                        tbox.SelectAll();
                    }
                }
                //else if (RefreshCommand != null)
                //    RefreshCommand.Execute(null);
            });

            this.AddHandler(TextBox.LostFocusEvent, (RoutedEventHandler)delegate(object sender, RoutedEventArgs e)
            {
                if (e.OriginalSource.Equals(tbox))
                {

                    if (!btnToggle.IsMouseOver)
                    {
                        ToggleTextBoxVisibility(false);
                    }
                    ////if (this.IsKeyboardFocusWithin)
                    //RootModel.IsBreadcrumbVisible = true;
                }
            });

            SetupAnimations();

            if (!EnableAutoFade)
                UnfadeStoryBoard.Begin();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
            if (!IsTextBoxEnabled)
                return;

            ToggleTextBoxVisibility(true);
            tbox.Focus();
            tbox.SelectAll();

        }

        #endregion

        #region Data

        internal ToggleButton btnToggle;
        internal ContextMenu bcoreCM;
        internal TextBox tbox;
        internal Grid BreadcrumbBackgroundGrid;

        #endregion


        #region Public Properties

        public Storyboard FadeStoryBoard { get; private set; }
        public Storyboard UnfadeStoryBoard { get; private set; }

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(Breadcrumb2), 
            new UIPropertyMetadata());


        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(Breadcrumb2),
            new UIPropertyMetadata());


        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(Breadcrumb2), new UIPropertyMetadata());




        public bool EnableAutoFade
        {
            get { return (bool)GetValue(EnableAutoFadeProperty); }
            set { SetValue(EnableAutoFadeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableFade.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAutoFadeProperty =
            DependencyProperty.Register("EnableAutoFade", typeof(bool), typeof(Breadcrumb2),
            new UIPropertyMetadata(
                new PropertyChangedCallback(
                    (o, e) => { Storyboard sb = (o as Breadcrumb2).UnfadeStoryBoard; if (sb != null) sb.Begin();  })
                    
                    ));

        

        public bool IsBreadcrumbVisible
        {
            get { return (bool)GetValue(IsBreadcrumbVisibleProperty); }
            set { SetValue(IsBreadcrumbVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBreadcrumbVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBreadcrumbVisibleProperty =
            DependencyProperty.Register("IsBreadcrumbVisible", typeof(bool), typeof(Breadcrumb2), new UIPropertyMetadata(true));




        public bool IsBreadcrumbEnabled
        {
            get { return (bool)GetValue(IsBreadcrumbEnabledProperty); }
            set { SetValue(IsBreadcrumbEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBreadcrumbEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBreadcrumbEnabledProperty =
            DependencyProperty.Register("IsBreadcrumbEnabled", typeof(bool), typeof(Breadcrumb2), new UIPropertyMetadata(true));

        

        public bool IsTextBoxEnabled
        {
            get { return (bool)GetValue(IsTextBoxEnabledProperty); }
            set { SetValue(IsTextBoxEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTextBoxEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsTextBoxEnabledProperty =
            DependencyProperty.Register("IsTextBoxEnabled", typeof(bool),
            typeof(Breadcrumb2), new UIPropertyMetadata(true));


        #endregion

    }
}
