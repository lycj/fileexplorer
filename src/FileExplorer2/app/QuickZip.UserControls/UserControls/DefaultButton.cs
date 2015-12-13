using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

namespace QuickZip.UserControls
{
    //http://connect.microsoft.com/VisualStudio/feedback/details/415801/wpf-button-with-isdefault-true-invoked-by-enter-key-unexpectedly
    public class DefaultButton : Button
    {
        DispatcherTimer dt;

        public DefaultButton()
        {
            this.Loaded += (RoutedEventHandler)delegate
            {

                dt = new DispatcherTimer();
                dt.Interval = TimeSpan.FromSeconds(1);
                dt.Tick += new EventHandler(dt_Tick);
                dt.Start();
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
        }

        void dt_Tick(object sender, EventArgs e)
        {
            dt.Stop();
            dt.Tick -= new EventHandler(dt_Tick);
            IsDefault = true;
        }
    }
}
