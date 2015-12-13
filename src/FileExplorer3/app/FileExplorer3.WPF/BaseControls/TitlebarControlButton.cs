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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{
    public class TitlebarControlButton : ButtonBase
    {
        static TitlebarControlButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitlebarControlButton), new FrameworkPropertyMetadata(typeof(TitlebarControlButton)));
        }

        public TitlebarControlButton()
        {

        }

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            
            Window AncestorWindow = UITools.FindAncestor<Window>(this);

            if (AncestorWindow != null)
            {
                DependencyPropertyDescriptor waDescriptor = DependencyPropertyDescriptor.FromProperty(Window.IsActiveProperty, typeof(Window));
                waDescriptor.AddValueChanged
                    (AncestorWindow, new EventHandler(delegate
                    {
                        WindowIsActive = AncestorWindow.IsActive;

                    }));
            }
        }

        public static void BackgroundColorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TitlebarControlButton tb = (TitlebarControlButton)sender;
            tb.Background = new LinearGradientBrush(
                new GradientStopCollection()
                {
                    new GradientStop(Color.FromRgb(222,222,222), 0),
                    new GradientStop((Color)e.NewValue, 1)
                },
                new Point(0.5, 0), new Point(0.5, 1));
            
        }

        #endregion

        #region Dependency Properties

        internal static readonly DependencyProperty WindowIsActiveProperty =
       DependencyProperty.Register("WindowIsActive", typeof(bool), typeof(TitlebarControlButton),
       new PropertyMetadata(true));

        internal bool WindowIsActive
        {
            get { return (bool)GetValue(WindowIsActiveProperty); }
            set { SetValue(WindowIsActiveProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TitlebarControlButton),
            new PropertyMetadata(new CornerRadius(0)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(TitlebarControlButton),
            new PropertyMetadata(new PropertyChangedCallback(BackgroundColorChanged)));

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BlurRadiusProperty =
           DependencyProperty.Register("BlurRadius", typeof(double), typeof(TitlebarControlButton),
           new PropertyMetadata(0.0d));

        public double BlurRadius
        {
            get { return (double)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        #endregion
    }
}
