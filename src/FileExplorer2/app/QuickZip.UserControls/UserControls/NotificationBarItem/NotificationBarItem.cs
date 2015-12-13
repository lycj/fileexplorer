using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Markup;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public class NotificationBarSubItem : MenuItem
    {
        //static NotificationBarSubItem()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationBarSubItem), new FrameworkPropertyMetadata(typeof(NotificationBarSubItem)));
        //}
    }



    [DefaultProperty("Header")]
    [ContentProperty("Header")]
    public class NotificationBarItem : MenuItem
    {
        static NotificationBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationBarItem), new FrameworkPropertyMetadata(typeof(NotificationBarItem)));
        }


        public NotificationBarItem()
        {
            this.AddHandler(MenuItem.ClickEvent, (RoutedEventHandler)delegate
            {
                this.IsSubmenuOpen = false;
            });

            this.AddHandler(MenuItem.MouseDoubleClickEvent, (RoutedEventHandler)delegate
            {
                if (this.Command != null)
                    Command.Execute(null);
            });
        }

        #region Public Properties

        //public Storyboard ProgressBarForegroundChangeStoryBoard { get; set; }
        //public ColorAnimation ProgressBarColorAnimation{ get; set; }

        public static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine("Header changed");
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NotificationBarSubItem();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.IsActive)
                RaiseEvent(new RoutedEventArgs(UnFadeEvent));
            else RaiseEvent(new RoutedEventArgs(FadeEvent));
        }

        //public static void OnNotificationModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        //{
        //    NotificationBarItem nitem = (NotificationBarItem)obj;
        //    //Debug.WriteLine("Header changed");
        //    Color newColor = Colors.CadetBlue;
        //    switch ((NotificationMode)e.NewValue)
        //    {
        //        case NotificationMode.Error: newColor = Colors.Red; break;
        //        case NotificationMode.Warning: newColor = Colors.YellowGreen; break;
        //    }

        //    Storyboard ProgressBarForegroundChangeStoryBoard = new Storyboard();
        //    ColorAnimation ProgressBarColorAnimation = new ColorAnimation(newColor, new Duration(TimeSpan.FromSeconds(0.5)));
        //    //Storyboard.SetTargetName(ProgressBarColorAnimation, "pBar");
        //    ProgressBar pBar = nitem.FindName("pBar") as ProgressBar;
        //    if (pBar != null)
        //    {
        //        Storyboard.SetTarget(ProgressBarColorAnimation, pBar);
        //        Storyboard.SetTargetProperty(ProgressBarColorAnimation, new PropertyPath("Foreground"));
        //        ProgressBarForegroundChangeStoryBoard.Children.Add(ProgressBarColorAnimation);
        //        ProgressBarForegroundChangeStoryBoard.Begin();
        //    }
        //}

        public static RoutedEvent FadeEvent = EventManager.RegisterRoutedEvent("Fade", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationBarItem));
        public static RoutedEvent UnFadeEvent = EventManager.RegisterRoutedEvent("UnFade", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NotificationBarItem));

        public event RoutedEventHandler Fade
        {
            add { AddHandler(FadeEvent, value); }
            remove { RemoveHandler(FadeEvent, value); }
        }


        public static void OnIsActiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            NotificationBarItem nitem = (NotificationBarItem)obj;
            if (nitem.IsVisible)
               if (e.NewValue != e.OldValue)
                   if (nitem.Template.FindName("icon", nitem) != null)
                {
                    if ((bool)e.NewValue)
                        nitem.RaiseEvent(new RoutedEventArgs(UnFadeEvent));
                    else nitem.RaiseEvent(new RoutedEventArgs(FadeEvent));
                }
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(NotificationBarItem)
         , new PropertyMetadata(true, new PropertyChangedCallback(OnIsActiveChanged)));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }



        public static readonly DependencyProperty ProgressForegroundProperty = DependencyProperty.Register("ProgressForeground", typeof(Brush), typeof(NotificationBarItem));

        public Brush ProgressForeground
        {
            get { return (Brush)GetValue(ProgressForegroundProperty); }
            set { SetValue(ProgressForegroundProperty, value); }
        }



        public new static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(NotificationBarItem)
           , new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged)));

        public new string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty SubHeaderProperty = DependencyProperty.Register("SubHeader", typeof(string), typeof(NotificationBarItem));

        public string SubHeader
        {
            get { return (string)GetValue(SubHeaderProperty); }
            set { SetValue(SubHeaderProperty, value); }
        }


        public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register("Progress", typeof(double), typeof(NotificationBarItem));

        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public new static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotificationBarItem));

        public new ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(NotificationBarItem));

        

        #endregion
    }
}
