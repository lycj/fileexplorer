using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.WPF.BaseControls
{
    /// <summary>
    /// Display ContentOn or ContentOff depends on whether IsSwitchOn is true.
    /// </summary>
    public class Switch : HeaderedContentControl
    {
        #region Constructor

        static Switch()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Switch),
                new FrameworkPropertyMetadata(typeof(Switch)));
        }

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //this.AddHandler(HeaderedContentControl.MouseDownEvent, (RoutedEventHandler)((o, e) =>
            //    {
            //        this.SetValue(IsSwitchOnProperty, !IsSwitchOn);
            //    }));
        }

        public static void OnIsSwitchOnChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public bool IsSwitchOn
        {
            get { return (bool)GetValue(IsSwitchOnProperty); }
            set { SetValue(IsSwitchOnProperty, value); }
        }

        public static readonly DependencyProperty IsSwitchOnProperty =
            DependencyProperty.Register("IsSwitchOn", typeof(bool),
            typeof(Switch), new UIPropertyMetadata(true, new PropertyChangedCallback(OnIsSwitchOnChanged)));

        public object ContentOn
        {
            get { return (object)GetValue(ContentOnProperty); }
            set { SetValue(ContentOnProperty, value); }
        }

        public static readonly DependencyProperty ContentOnProperty =
            DependencyProperty.Register("ContentOn", typeof(object),
            typeof(Switch), new UIPropertyMetadata(null));

        public object ContentOff
        {
            get { return (object)GetValue(ContentOffProperty); }
            set { SetValue(ContentOffProperty, value); }
        }

        public static readonly DependencyProperty ContentOffProperty =
            DependencyProperty.Register("ContentOff", typeof(object),
            typeof(Switch), new UIPropertyMetadata(null));

        #endregion
    }
}
