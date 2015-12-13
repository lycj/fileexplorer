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
    /// A Canvas that let you choose a center point (CentreX, CentreY), while all subitems are 
    /// position based on the center point, it's offset (OffsetX, OffsetY) and it's size (DesiredWidth/DesiredHeight)
    /// </summary>
    public class SelfCenteredCanvas : Panel
    {
        #region Constructor

        #endregion

        #region Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in base.InternalChildren)
            {
                element.Measure(availableSize);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in base.InternalChildren)
            {
                double X = CentreX + GetOffsetX(element);
                double Y = CentreY + GetOffsetY(element);
                double left = X - (element.DesiredSize.Width / 2);
                double top = Y - (element.DesiredSize.Height / 2);
                element.Arrange(new Rect(new Point(left, top), new Size(element.DesiredSize.Width, element.DesiredSize.Height)));
            }
            return finalSize;
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public static DependencyProperty CentreXProperty = DependencyProperty.Register("CentreX", typeof(double), typeof(SelfCenteredCanvas),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));
        
        public double CentreX
        {
            get { return (double)GetValue(CentreXProperty); }
            set { SetValue(CentreXProperty, value); }
        }

        public static DependencyProperty CentreYProperty = DependencyProperty.Register("CentreY", typeof(double), typeof(SelfCenteredCanvas),
           new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public double CentreY
        {
            get { return (double)GetValue(CentreYProperty); }
            set { SetValue(CentreYProperty, value); }
        }

        public static DependencyProperty OffsetXProperty = DependencyProperty.RegisterAttached("OffsetX",
         typeof(double), typeof(CenteredCanvas), new FrameworkPropertyMetadata(0.0,
             FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static void SetOffsetX(UIElement element, double value)
        {
            element.SetValue(OffsetXProperty, value);
        }

        public static double GetOffsetX(UIElement element)
        {
            return (double)element.GetValue(OffsetXProperty);
        }

        public static DependencyProperty OffsetYProperty = DependencyProperty.RegisterAttached("OffsetY",
         typeof(double), typeof(CenteredCanvas), new FrameworkPropertyMetadata(0.0,
             FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static void SetOffsetY(UIElement element, double value)
        {
            element.SetValue(OffsetYProperty, value);
        }

        public static double GetOffsetY(UIElement element)
        {
            return (double)element.GetValue(OffsetYProperty);
        }

        #endregion

     
    }
}
