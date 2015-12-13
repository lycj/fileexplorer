using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Defines;
using FileExplorer.Defines;

namespace FileExplorer.WPF.Utils
{
    public static class ControlUtils
    {
        public static Point GetScrollbarPosition(ScrollContentPresenter scp)
        {
            ScrollViewer scrollViewer = UITools.FindAncestor<ScrollViewer>(scp);
            return scrollViewer == null ? new Point() : new Point(scp.ActualWidth / scrollViewer.ViewportWidth * scrollViewer.HorizontalOffset,
                scp.ActualHeight / scrollViewer.ViewportHeight * scrollViewer.VerticalOffset);
        }

        public static Point GetScrollbarPosition(Control c)
        {
            var scp = c == null ? null : GetScrollContentPresenter(c);
            return scp == null ? UIEventHubProperties.InvalidPoint : GetScrollbarPosition(scp);
        }

        public static ScrollContentPresenter GetScrollContentPresenter(ItemsPresenter ip)
        {
            return UITools.FindAncestor<ScrollContentPresenter>(ip);
        }

        public static ScrollContentPresenter GetScrollContentPresenter(Control c)
        {
            var retVal = UIEventHubProperties.GetScrollContentPresenter(c);
            if (retVal == null)
            {
                retVal = UITools.FindAncestor<ScrollContentPresenter>(
                GetItemsPresenter(c));
                UIEventHubProperties.SetScrollContentPresenter(c, retVal);
            }
            return retVal;
        }

        public static ItemsPresenter GetItemsPresenter(Control c)
        {
            return UITools.FindVisualChild<ItemsPresenter>(c, null, 8);
        }

        public static AdornerLayer GetAdornerLayer(Control c, bool localOnly = false)
        {
            ItemsPresenter ip = GetItemsPresenter(c);
            ScrollContentPresenter p = GetScrollContentPresenter(ip);

            AdornerLayer adornerLayer;

            if (!localOnly)
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(p);
                if (adornerLayer != null)
                    return adornerLayer;
            }

            AdornerDecorator ad = UITools.FindAncestor<AdornerDecorator>(ip);
            try
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(ad);
                if (adornerLayer != null)
                    return adornerLayer;
            }
            catch { }
            return p.AdornerLayer;
        }


    }
}
