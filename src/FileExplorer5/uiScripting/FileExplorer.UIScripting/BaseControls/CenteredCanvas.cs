using FileExplorer.UIEventHub;
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
    /// A Canvas panel that position item by it's centre point (X,Y) and it's size (DesiredWidth, DesiredHeight)
    /// </summary>
    public class CenteredCanvas : Panel, IChildInfo
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
                double X = GetX(element);
                double Y = GetY(element);
                double left = X - (element.DesiredSize.Width / 2);
                double top = Y - (element.DesiredSize.Height / 2);
                element.Arrange(new Rect(new Point(left, top), new Size(element.DesiredSize.Width, element.DesiredSize.Height)));
            }
            return finalSize;
        }

        public Rect GetChildRect(int itemIndex)
        {
            if (itemIndex >= base.InternalChildren.Count)
                return new Rect(0, 0, 0, 0);
            var element = base.InternalChildren[itemIndex];
            double X = GetX(element);
            double Y = GetY(element);
            double left = X - (element.DesiredSize.Width / 2);
            double top = Y - (element.DesiredSize.Height / 2);
            return new Rect(left, top, element.DesiredSize.Width, element.DesiredSize.Height);
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public static DependencyProperty XProperty = DependencyProperty.RegisterAttached("X",
         typeof(double), typeof(CenteredCanvas), new FrameworkPropertyMetadata(0.0,
             FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static void SetX(UIElement element, double value)
        {
            element.SetValue(XProperty, value);
        }

        public static double GetX(UIElement element)
        {
            return (double)element.GetValue(XProperty);
        }

        public static DependencyProperty YProperty = DependencyProperty.RegisterAttached("Y",
         typeof(double), typeof(CenteredCanvas), new FrameworkPropertyMetadata(0.0,
             FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static void SetY(UIElement element, double value)
        {
            element.SetValue(YProperty, value);
        }

        public static double GetY(UIElement element)
        {
            return (double)element.GetValue(YProperty);
        }

        #endregion




    }
}
