///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
// This document used kirupa's work (http://blog.kirupa.com/?p=256)                                              //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{
    public class TitlebarContainer : ContentControl
    {
        static TitlebarContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitlebarContainer), new FrameworkPropertyMetadata(typeof(TitlebarContainer)));
        }

        public TitlebarContainer()
        {

        }

        #region Methods

        //http://blog.kirupa.com/?p=256 (Resizing Custom / Transparent Windows (Again!))
        #region Third Party Code
        private void InitializeWindowSource(object sender, EventArgs e)
        {
            hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
        }

        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Window AncestorWindow = UITools.FindAncestor<Window>(this);

            if (AncestorWindow != null)
            {
                InitializeWindowSource(AncestorWindow, null);

                this.AddHandler(Rectangle.MouseDownEvent, (MouseButtonEventHandler)delegate(object sender, MouseButtonEventArgs e)
                {
                    if (e.OriginalSource is Rectangle)
                    {
                        e.Handled = true;
                        Rectangle rect = (Rectangle)e.OriginalSource;
                        if (!String.IsNullOrEmpty(rect.Name))
                        {
                            ResizeDirection resizeDirection = (ResizeDirection)Enum.Parse(typeof(ResizeDirection), rect.Name);
                            ResizeWindow(resizeDirection);
                        }

                    }
                });


                DependencyPropertyDescriptor wsDescriptor = DependencyPropertyDescriptor.FromProperty(Window.WindowStateProperty, typeof(Window));
                wsDescriptor.AddValueChanged
                    (AncestorWindow, new EventHandler(delegate
                    {
                        ResizeGripVisibility = AncestorWindow.WindowState == WindowState.Maximized ?
                            Visibility.Collapsed : Visibility.Visible;

                    }));

                DependencyPropertyDescriptor waDescriptor = DependencyPropertyDescriptor.FromProperty(Window.IsActiveProperty, typeof(Window));
                waDescriptor.AddValueChanged
                    (AncestorWindow, new EventHandler(delegate
                    {
                        WindowIsActive = AncestorWindow.IsActive;

                    }));

            }
        }

        #endregion


        #region Data

        private const int WM_SYSCOMMAND = 0x0112;
        private HwndSource hwndSource;

        #endregion


        internal static readonly DependencyProperty ResizeGripVisibilityProperty =
         DependencyProperty.Register("ResizeGripVisibility", typeof(Visibility), typeof(TitlebarContainer),
         new PropertyMetadata(Visibility.Visible));

        internal Visibility ResizeGripVisibility
        {
            get { return (Visibility)GetValue(ResizeGripVisibilityProperty); }
            set { SetValue(ResizeGripVisibilityProperty, value); }
        }

        internal static readonly DependencyProperty WindowIsActiveProperty =
        DependencyProperty.Register("WindowIsActive", typeof(bool), typeof(TitlebarContainer),
        new PropertyMetadata(true));

        internal bool WindowIsActive
        {
            get { return (bool)GetValue(WindowIsActiveProperty); }
            set { SetValue(WindowIsActiveProperty, value); }
        }


        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(UIElement), typeof(TitlebarContainer));

        public UIElement Title
        {
            get { return (UIElement)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
