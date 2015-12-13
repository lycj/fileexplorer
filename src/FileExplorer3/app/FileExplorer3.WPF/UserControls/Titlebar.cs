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
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.UserControls
{
    public static class TitlebarCommands
    {
        public static readonly RoutedUICommand CloseWindowCommand = new RoutedUICommand("Close Window", "CloseWindow", typeof(TitlebarCommands));
        public static readonly RoutedUICommand MinimizeWindowCommand = new RoutedUICommand("Minimize Window", "MinimizeWindow", typeof(TitlebarCommands));
        public static readonly RoutedUICommand MaximizeWindowCommand = new RoutedUICommand("Maximize Window", "MaximizeWindow", typeof(TitlebarCommands));
        public static readonly RoutedUICommand RestoreWindowCommand = new RoutedUICommand("Restore Window", "RestoreWindow", typeof(TitlebarCommands));
    }


    public class Titlebar : ContentControl
    {
        static Titlebar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Titlebar), new FrameworkPropertyMetadata(typeof(Titlebar)));
        }

        public Titlebar()
        {
            //Func<Window> getAncestorWindow = () => { return UITools.FindAncestor<Window>(this); };
            //Func<TitlebarContainer> getTitlebarContainer = () => { return UITools.FindAncestor<TitlebarContainer>(this); };

            //this.AddHandler(UIElement.MouseDownEvent,
            //    (RoutedEventHandler)delegate
            //{
            //    getAncestorWindow().DragMove();
            //});




        }


        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Window AncestorWindow = UITools.FindAncestor<Window>(this); 
            TitlebarContainer titleBarContainer = UITools.FindAncestor<TitlebarContainer>(this);            

            if (AncestorWindow != null)
            {
                Action updateVisibilityState = () =>
                    {
                        bool isMaximized = AncestorWindow.WindowState == WindowState.Maximized;
                        this.Visibility = isMaximized ? VisibilityWhenMaximized : VisibilityWhenNormal;
                        AncestorWindow.WindowStyle = isMaximized ? WindowStyleWhenMaximized : WindowStyleWhenNormal;
                        UIElement restoreButton = (UIElement)this.Template.FindName("restoreButton", this);
                        UIElement maximizeButton = (UIElement)this.Template.FindName("maximizeButton", this);

                        if (AncestorWindow.WindowState == WindowState.Maximized)
                        {
                            restoreButton.Visibility = System.Windows.Visibility.Visible;
                            maximizeButton.Visibility = System.Windows.Visibility.Collapsed;
                        }
                        else
                        {
                            restoreButton.Visibility = System.Windows.Visibility.Collapsed;
                            maximizeButton.Visibility = System.Windows.Visibility.Visible;
                        }
                    };
                
                DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(Window.WindowStateProperty, typeof(Window));
                descriptor.AddValueChanged
                    (AncestorWindow, new EventHandler(delegate
                {
                  
                    updateVisibilityState();


                    
                }));

                updateVisibilityState();

                this.AddHandler(UIElement.MouseLeftButtonDownEvent,
               (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs e)
               {                   
                   AncestorWindow.DragMove();
               });

                this.AddHandler(Control.MouseDoubleClickEvent,
               (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs e)
               {
                   if (e.LeftButton == MouseButtonState.Pressed)
                   {
                       if (AncestorWindow.WindowState == WindowState.Maximized)
                           AncestorWindow.WindowState = WindowState.Normal;
                       else AncestorWindow.WindowState = WindowState.Maximized;
                   }
               });

                this.CommandBindings.Add(new CommandBinding(TitlebarCommands.CloseWindowCommand,
                new ExecutedRoutedEventHandler(delegate { AncestorWindow.Close(); })));

                this.CommandBindings.Add(new CommandBinding(TitlebarCommands.MaximizeWindowCommand,
                   new ExecutedRoutedEventHandler(delegate { AncestorWindow.WindowState = WindowState.Maximized; })));

                this.CommandBindings.Add(new CommandBinding(TitlebarCommands.MinimizeWindowCommand,
                   new ExecutedRoutedEventHandler(delegate { AncestorWindow.WindowState = WindowState.Minimized; })));

                this.CommandBindings.Add(new CommandBinding(TitlebarCommands.RestoreWindowCommand,
                   new ExecutedRoutedEventHandler(delegate { AncestorWindow.WindowState = WindowState.Normal; })));
            }

            
        }

        #endregion

        #region Dependency Properties

       


        public static readonly DependencyProperty TitlebarHeightProperty =
           DependencyProperty.Register("TitlebarHeight", typeof(double), typeof(Titlebar),
           new PropertyMetadata(20.0d));

        public double TitlebarHeight
        {
            get { return (double)GetValue(TitlebarHeightProperty); }
            set { SetValue(TitlebarHeightProperty, value); }
        }

        public static readonly DependencyProperty VisibilityWhenNormalProperty =
            DependencyProperty.Register("VisibilityWhenNormal", typeof(Visibility), typeof(Titlebar),
            new PropertyMetadata(Visibility.Visible));

        public Visibility VisibilityWhenNormal
        {
            get { return (Visibility)GetValue(VisibilityWhenNormalProperty); }
            set { SetValue(VisibilityWhenNormalProperty, value); }
        }

        public static readonly DependencyProperty VisibilityWhenMaximizedProperty =
            DependencyProperty.Register("VisibilityWhenMaximized", typeof(Visibility), typeof(Titlebar),
            new PropertyMetadata(Visibility.Visible));

        public Visibility VisibilityWhenMaximized
        {
            get { return (Visibility)GetValue(VisibilityWhenMaximizedProperty); }
            set { SetValue(VisibilityWhenMaximizedProperty, value); }
        }

        public static readonly DependencyProperty WindowStyleWhenNormalProperty =
               DependencyProperty.Register("WindowStyleWhenNormal", typeof(WindowStyle), typeof(Titlebar),
               new PropertyMetadata(WindowStyle.SingleBorderWindow));

        public WindowStyle WindowStyleWhenNormal
        {
            get { return (WindowStyle)GetValue(WindowStyleWhenNormalProperty); }
            set { SetValue(WindowStyleWhenNormalProperty, value); }
        }

        public static readonly DependencyProperty WindowStyleWhenMaximizedProperty =
              DependencyProperty.Register("WindowStyleWhenMaximized", typeof(WindowStyle), typeof(Titlebar),
              new PropertyMetadata(WindowStyle.SingleBorderWindow));

        public WindowStyle WindowStyleWhenMaximized
        {
            get { return (WindowStyle)GetValue(WindowStyleWhenMaximizedProperty); }
            set { SetValue(WindowStyleWhenMaximizedProperty, value); }
        }

        #endregion

    }
}
