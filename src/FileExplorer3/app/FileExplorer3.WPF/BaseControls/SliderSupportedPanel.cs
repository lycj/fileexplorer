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
using FileExplorer.WPF.UserControls;

namespace FileExplorer.WPF.BaseControls
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

            for (int i = 0; i < Children.Count; i++)            
            {
                Control child = (Control)Children[i];

                //child.VerticalContentAlignment = i == 0 ? VerticalAlignment.Top :
                //    i == Children.Count - 1 ? VerticalAlignment.Bottom : VerticalAlignment.Center;
                
                Rect allocatedRect = new Rect(0, curY, finalSize.Width, child.DesiredSize.Height);
                child.Arrange(allocatedRect);        
                
                double midPoint = i == 0 ? 
                    finalSize.Height : 
                    finalSize.Height - ((allocatedRect.Top + allocatedRect.Bottom) / 2.0d);
                //double midPoint = finalSize.Height - ((curY + item.HeaderHeight) / 2.0d);
                if (child is ToolbarSubItemEx)
                {
                    ToolbarSubItemEx item = (ToolbarSubItemEx)child;
                    double value;
                    if (!item.IsSeparator && item.Value != null && double.TryParse(item.Value.ToString(), out value))                    
                        steps.Add(new Step(midPoint, value, item.IsStepStop));

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
