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
    public class BreacrumbCorePanel : Panel
    {
        private static DependencyProperty IsArrangedProperty = DependencyProperty.RegisterAttached("IsArranged", typeof(bool),
            typeof(BreacrumbCorePanel), new PropertyMetadata(false));
        private static DependencyProperty ArrangedRectProperty = DependencyProperty.RegisterAttached("ArrangedRect", typeof(Rect),
            typeof(BreacrumbCorePanel), new PropertyMetadata(new Rect(0, 0, 0, 0)));

        public static readonly RoutedEvent RemoveShadowItemEvent = EventManager.RegisterRoutedEvent("RemoveShadowItem",
           RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BreacrumbCorePanel));

        public event RoutedEventHandler RemoveShadowItem
        {
            add { AddHandler(RemoveShadowItemEvent, value); }
            remove { RemoveHandler(RemoveShadowItemEvent, value); }
        }

        private Size lastfinalSize;

        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size(0, 0);

            foreach (UIElement child in Children)
            {
                //Fix:This is a temp fix for ArithException when loading icons for Mds files.
                try { child.Measure(availableSize); }
                catch { }
                resultSize.Width += child.DesiredSize.Width;
                resultSize.Height = Math.Max(resultSize.Height, child.DesiredSize.Height);
            }

            resultSize.Width =
                double.IsPositiveInfinity(availableSize.Width) ?
                resultSize.Width : availableSize.Width;
            resultSize.Width = Math.Min(resultSize.Width, availableSize.Width);

            if (Children.Count > 0)
            {
                Rect firstItemRect = new Rect(new Point(0, 0), Children[0].DesiredSize);
                double finalWidth = availableSize.Width - firstItemRect.Width;
                for (int i = Children.Count - 1; i >= 1; i--)
                {
                    Children[i].Measure(new Size(finalWidth, availableSize.Height));
                    finalWidth -= Children[i].DesiredSize.Width;
                }
            }


            return resultSize;
        }

        #region AttachedProperties

        private bool GetIsArranged(UIElement element)
        {
            return (bool)element.GetValue(IsArrangedProperty);
        }

        public void SetIsArranged(UIElement element, bool value)
        {
            element.SetValue(IsArrangedProperty, value);
        }

        private Rect GetArrangedRect(UIElement element)
        {
            return (Rect)element.GetValue(ArrangedRectProperty);
        }

        public void SetArrangedRect(UIElement element, Rect value)
        {
            element.SetValue(ArrangedRectProperty, value);
            if (element is BreadcrumbItem)
                (element as BreadcrumbItem).IsItemVisible = !value.Equals(new Rect(0, 0, 0, 0));
        }
        #endregion

        //private List<UIElement> GhostFolders = new List<UIElement>();

        //private void ArrangeChildren2(double availableSize, double firstItemWidth, bool arrangeVirtual)
        //{
        //    double curX = firstItemWidth;

        //    for (int i = Children.Count - 1; i >= 1; i--)
        //    {
        //        BreadcrumbItem child = (BreadcrumbItem)Children[i];
        //        if (!(child.IsShadowItem && !arrangeVirtual))
        //        {
        //            if (curX + child.DesiredSize.Width <= availableSize)
        //                curX += child.DesiredSize.Width;
        //            else break;
        //        }

        //    }
        //    ArrangeChildren(curX + firstItemWidth, firstItemWidth, arrangeVirtual);

        //}


        private void ArrangeChildren(double availableSize, double firstItemWidth, bool arrangeVirtual)
        {
            double curX = availableSize;

            for (int i = Children.Count - 1; i >= 1; i--)
            {
                BreadcrumbItem child = (BreadcrumbItem)Children[i];

                if (child.IsShadowItem && !arrangeVirtual)
                {
                    SetArrangedRect(child, new Rect(0, 0, 0, 0));
                }
                else
                {
                    if (curX >= firstItemWidth || i == Children.Count - 1)
                    {
                        if (curX - firstItemWidth < child.DesiredSize.Width)
                        {
                            Rect newRect = new Rect(new Point(firstItemWidth, 0), new Point(curX + firstItemWidth, child.DesiredSize.Height));
                            SetArrangedRect(child, newRect);
                        }
                        else
                            SetArrangedRect(child, new Rect(curX - child.DesiredSize.Width + firstItemWidth, 0, child.DesiredSize.Width,
                                child.DesiredSize.Height));
                        SetIsArranged(child, true);
                    }
                    else
                    {
                        for (int j = 1; j <= Math.Min(i, Children.Count - 1); j++)
                        {
                            child = (BreadcrumbItem)Children[j];
                            SetArrangedRect(child, new Rect(0, 0, 0, 0));
                            child.Arrange(GetArrangedRect(child));
                            SetIsArranged(child, false);
                            //GhostFolders.Add(child);                        
                        }
                        break;
                    }

                    child.Arrange(GetArrangedRect(child));
                    curX -= child.DesiredSize.Width;
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            lastfinalSize = finalSize;

            if (this.Children == null || this.Children.Count == 0)
                return finalSize;

            Rect firstItemRect = new Rect(new Point(0, 0), Children[0].DesiredSize);
            double totalX = firstItemRect.Width;
            for (int i = Children.Count - 1; i >= 1; i--)
                if (totalX + Children[i].DesiredSize.Width < finalSize.Width || i == Children.Count - 1)
                {
                    totalX += Children[i].DesiredSize.Width;
                }
                else break;

            double curX = Math.Min(totalX, finalSize.Width) + 1;


            
            SetArrangedRect(Children[0], firstItemRect);
            SetIsArranged(Children[0], true);
            curX -= firstItemRect.Width;

            ArrangeChildren(curX, firstItemRect.Width, true);

            Children[0].Arrange(GetArrangedRect(Children[0]));
            return finalSize;
        }
    }
}
