using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileExplorer.Utils
{
    public static class DataContextFinder
    {
        public static Func<object, ISupportDrag> SupportDrag =
            dc => (dc is ISupportDrag && (dc as ISupportDrag).HasDraggables) ? dc as ISupportDrag :
                (dc is ISupportDragHelper && (dc as ISupportDragHelper).DragHelper.HasDraggables) ? (dc as ISupportDragHelper).DragHelper :
                null;

        public static Func<object, ISupportShellDrag> SupportShellDrag =
            dc => (dc is ISupportShellDrag && (dc as ISupportShellDrag).HasDraggables) ? dc as ISupportShellDrag :
                (dc is ISupportDragHelper && (dc as ISupportDragHelper).DragHelper is ISupportShellDrag && 
                (dc as ISupportDragHelper).DragHelper.HasDraggables) ? (dc as ISupportDragHelper).DragHelper as ISupportShellDrag:
                null;

        public static Func<object, ISupportShellDrop> SupportShellDrop =
           dc => (dc is ISupportShellDrop && (dc as ISupportShellDrop).IsDroppable) ? dc as ISupportShellDrop :
               (dc is ISupportDropHelper && (dc as ISupportDropHelper).DropHelper is ISupportShellDrop &&
               (dc as ISupportDropHelper).DropHelper.IsDroppable) ? (dc as ISupportDropHelper).DropHelper as ISupportShellDrop :
               null;

        public static Func<object, ISupportDrop> SupportDrop =
            dc => (dc is ISupportDrop && (dc as ISupportDrop).IsDroppable) ? dc as ISupportDrop :
                (dc is ISupportDropHelper && (dc as ISupportDropHelper).DropHelper.IsDroppable) ? (dc as ISupportDropHelper).DropHelper :
                null;


        public static T GetDataContext<T>(FrameworkElement origSource, Func<object, T> filter = null)
        {
            FrameworkElement ele;
            return GetDataContext(origSource, out ele, filter);
        }


        public static FrameworkElement GetDataContextOwner(FrameworkElement ele)
        {
            var tvItem = UITools.FindAncestor<TreeViewItem>(ele);
            if (tvItem != null)
                return tvItem;

            var lvItem = UITools.FindAncestor<ListViewItem>(ele);
            if (lvItem != null)
                return lvItem;

            var ic = UITools.FindAncestor<ItemsControl>(ele);
            if (ic != null)
                return ic;

            return ele;
        }

        public static T GetDataContext<T>(ref FrameworkElement ele, Func<object, T> filter = null)
        {
            object dataContext = ele.DataContext;
            var filterResult = filter(dataContext);
            if (filterResult != null)
            {
                ele = GetDataContextOwner(ele);
                return filterResult;
            }
            else
            {
                var ic = UITools.FindAncestor<ItemsControl>(ele);
                while (ic != null)
                {
                    filterResult = filter(ic.DataContext);
                    if (filterResult != null)
                    {
                        ele = GetDataContextOwner(ic);
                        return filterResult;
                    }
                    ic = UITools.FindAncestor<ItemsControl>(VisualTreeHelper.GetParent(ic));
                }

                return default(T);
            }

        }

        public static T GetDataContext<T>(FrameworkElement origSource, out FrameworkElement ele, 
            Func<object, T> filter = null)
        {            
            ele = null;
            object dataContext = origSource.DataContext;
            if (dataContext != null)
            {
                var filterResult = filter(dataContext);
                if (filterResult != null)
                {
                    ele = GetDataContextOwner(origSource);
                    return filterResult;
                }
                else
                {
                    var ic = UITools.FindAncestor<ItemsControl>(origSource);
                    while (ic != null)
                    {
                        filterResult = filter(ic.DataContext);
                        if (filterResult != null)
                        {
                            ele = GetDataContextOwner(ic);
                            return filterResult;
                        }
                        ic = UITools.FindAncestor<ItemsControl>(VisualTreeHelper.GetParent(ic));
                    }

                }
            }
            return default(T);
        }
    }
}
