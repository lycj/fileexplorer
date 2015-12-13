using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Threading;

namespace RGSamples
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    };

    public class GlassEffect
    {
        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled",
                typeof(Boolean),
                typeof(GlassEffect),
                new FrameworkPropertyMetadata(OnIsEnabledChanged));





        public static Brush GetBrushProperty(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BrushPropertyProperty);
        }

        public static void SetBrushProperty(DependencyObject obj, Brush value)
        {
            obj.SetValue(BrushPropertyProperty, value);
        }

        // Using a DependencyProperty as the backing store for BrushProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrushPropertyProperty =
            DependencyProperty.RegisterAttached("BrushProperty", typeof(Brush), typeof(GlassEffect), new UIPropertyMetadata(null));

        

        //public static readonly DependencyProperty TimerProperty =
        //    DependencyProperty.RegisterAttached("Timer",
        //        typeof(object),
        //        typeof(GlassEffect), new FrameworkPropertyMetadata(null));

        public static void SetIsEnabled(DependencyObject element, Boolean value)
        {
            element.SetValue(IsEnabledProperty, value);
        }
        public static Boolean GetIsEnabled(DependencyObject element)
        {
            return (Boolean)element.GetValue(IsEnabledProperty);
        }

        //public static void SetTimer(DependencyObject element, object value)
        //{
        //    element.SetValue(TimerProperty, value);
        //}
        //public static object GetTimer(DependencyObject element)
        //{
        //    return element.GetValue(TimerProperty);
        //}

        public static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
             Window wnd = (Window)obj;
               

            if ((bool)args.NewValue == true)
            {
                wnd.Loaded += new RoutedEventHandler(wnd_Loaded);

                //if (GetTimer(wnd) == null)
                //{
                //    DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                //    timer.Tick += new EventHandler((o, e) => wnd_Loaded(wnd, null));
                //    timer.Interval = TimeSpan.FromMinutes(0.5);
                //    timer.Start();

                //    SetTimer(wnd, timer);
                //}

                wnd.Activated += new EventHandler((o,e) => wnd_Loaded(wnd, null));
                wnd.SourceInitialized += new EventHandler((o, e) => wnd_Loaded(wnd, null));
            }
            else
            {
                //wnd.Background = GetBrushProperty(wnd);
                //IntPtr mainWindowPtr = new WindowInteropHelper(wnd).Handle;
                //HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                //MARGINS margins = new MARGINS();
                //margins.cxLeftWidth = 0;
                //margins.cxRightWidth = 0;
                //margins.cyTopHeight = 0;
                //margins.cyBottomHeight = 0;

                //DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                //if (GetTimer(wnd) is DispatcherTimer)
                //{
                //    DispatcherTimer timer = GetTimer(wnd) as DispatcherTimer;
                //    timer.Stop();
                //    SetTimer(wnd, null);
                //}
            }
        }

        static void wnd_Loaded(object sender, RoutedEventArgs e)
        {
            Window wnd = (Window)sender;
            Brush originalBackground = wnd.Background;
            SetBrushProperty(wnd, originalBackground);
            wnd.Background = Brushes.Transparent;
            try
            {
                IntPtr mainWindowPtr = new WindowInteropHelper(wnd).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                //System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
                //float DesktopDpiX = desktop.DpiX;
                //float DesktopDpiY = desktop.DpiY;

                MARGINS margins = new MARGINS();
                margins.cxLeftWidth = -1;
                margins.cxRightWidth = -1;
                margins.cyTopHeight = -1;
                margins.cyBottomHeight = -1;

                DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
            }
            catch (DllNotFoundException)
            {
                wnd.Background = originalBackground;
            }
        }

    }

}
