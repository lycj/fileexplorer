///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace QuickZip.UserControls
{
    public class SliderSupportedPanel : Panel
    {

        #region Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);

                resultSize.Height += child.DesiredSize.Height;
                resultSize.Width = Math.Max(resultSize.Width, child.DesiredSize.Width);
            }

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double curY = 0;
            List<Step> steps = new List<Step>();

            foreach (FrameworkElement child in Children)
            {
                Rect allocatedRect = new Rect(0, curY, finalSize.Width, child.DesiredSize.Height);
                child.Arrange(allocatedRect);
                double midPoint = finalSize.Height - ((allocatedRect.Top + allocatedRect.Bottom) / 2.0d);
                //double midPoint = finalSize.Height - ((curY + item.HeaderHeight) / 2.0d);
                if (child is ToolbarMenuItem)
                {
                    ToolbarMenuItem item = (ToolbarMenuItem)child;
                    if (!item.IsSeparator)
                        steps.Add(new Step(midPoint, item.SliderStep, item.IsStepStop));

                }
                curY += child.DesiredSize.Height;
            }

            Steps = new ObservableCollection<Step>(steps.OrderBy<Step, double>(x => x.Posision));
            return finalSize;
        }

        #endregion

        #region Public Properties

        public static readonly DependencyProperty StepsProperty = MultiStepSlider.StepsProperty.AddOwner(typeof(SliderSupportedPanel));

        public ObservableCollection<Step> Steps
        {
            get { return (ObservableCollection<Step>)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        #endregion
    }
}
