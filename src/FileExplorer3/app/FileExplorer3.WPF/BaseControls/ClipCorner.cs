using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FileExplorer.WPF.UserControls;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{
  
    public class ClipCorner : Border
    {
        #region Constructor

        static ClipCorner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClipCorner),
                new FrameworkPropertyMetadata(typeof(ClipCorner)));
        }
        #endregion

        #region Methods

   
        #endregion

        #region Data

        #endregion

        #region Public Properties

       
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
            typeof(ClipCorner), new PropertyMetadata(new CornerRadius(0)));


        #endregion
    }
}
