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
    /// Take multiple ContentTemplates (ContentTemplate, ContentTemplate2) and render it using the same Content.
    /// </summary>
    public class MultiContentControl : ContentControl
    {
        #region Constructor

        static MultiContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiContentControl), 
                new FrameworkPropertyMetadata(typeof(MultiContentControl)));
        }
        
        #endregion

        #region Methods
        
        #endregion

        #region Data
        
        #endregion

        #region Public Properties

        public static readonly DependencyProperty ContentVisible1Property =
           DependencyProperty.Register("ContentVisible1", typeof(bool), typeof(MultiContentControl),
           new PropertyMetadata(true));

        public bool ContentVisible1
        {
            get { return (bool)GetValue(ContentVisible1Property); }
            set { SetValue(ContentVisible1Property, value); }
        }

        public static readonly DependencyProperty ContentTemplate2Property =
            DependencyProperty.Register("ContentTemplate2", typeof(DataTemplate), typeof(MultiContentControl), 
            new PropertyMetadata(null));

        public DataTemplate ContentTemplate2
        {
            get { return (DataTemplate)GetValue(ContentTemplate2Property); }
            set { SetValue(ContentTemplate2Property, value); }
        }

        public static readonly DependencyProperty ContentVisible2Property =
            DependencyProperty.Register("ContentVisible2", typeof(bool), typeof(MultiContentControl), 
            new PropertyMetadata(false));

        public bool ContentVisible2
        {
            get { return (bool)GetValue(ContentVisible2Property); }
            set { SetValue(ContentVisible2Property, value); }
        }


        public static readonly DependencyProperty ContentTemplate3Property =
            DependencyProperty.Register("ContentTemplate3", typeof(DataTemplate), typeof(MultiContentControl), new PropertyMetadata(null));

        public DataTemplate ContentTemplate3
        {
            get { return (DataTemplate)GetValue(ContentTemplate3Property); }
            set { SetValue(ContentTemplate3Property, value); }
        }

        public static readonly DependencyProperty ContentVisible3Property =
            DependencyProperty.Register("ContentVisible3", typeof(bool), typeof(MultiContentControl),
            new PropertyMetadata(true));

        public bool ContentVisible3
        {
            get { return (bool)GetValue(ContentVisible3Property); }
            set { SetValue(ContentVisible3Property, value); }
        }

        #endregion
    }
}
