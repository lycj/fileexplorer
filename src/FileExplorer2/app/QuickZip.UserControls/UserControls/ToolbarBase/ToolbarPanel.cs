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

namespace QuickZip.UserControls
{
    public class ToolbarPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);

                resultSize.Width += child.DesiredSize.Width;
                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
            }

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double curLeftX = 0;
            double curRightX = finalSize.Width;

            foreach (FrameworkElement child in Children)
            {
                if (child.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    curRightX -= child.DesiredSize.Width;
                    if (curLeftX < curRightX)
                        child.Arrange(new Rect(curRightX, 0, child.DesiredSize.Width, finalSize.Height));
                    else child.Arrange(new Rect(0, 0, 0, 0));
                }
                else
                {
                    curLeftX += child.DesiredSize.Width;
                    if (curLeftX < curRightX)
                        child.Arrange(new Rect(curLeftX - child.DesiredSize.Width, 0, child.DesiredSize.Width, finalSize.Height));
                    else child.Arrange(new Rect(0, 0, 0, 0));
                }
            }

            return finalSize;
        }
    }
}
