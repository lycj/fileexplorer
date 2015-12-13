using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace QuickZip.UserControls
{
    public static partial class UITools
    {
        public static T FindAncestor<T>(DependencyObject obj) where T : DependencyObject
        {

            while (obj != null)
            {

                T o = obj as T;

                if (o != null)

                    return o;

                obj = VisualTreeHelper.GetParent(obj);
            }
            return default(T);

        }

        public static T FindLogicalAncestor<T>(DependencyObject obj) where T : DependencyObject
        {

            while (obj != null)
            {

                T o = obj as T;

                if (o != null)

                    return o;

                obj = LogicalTreeHelper.GetParent(obj);
            }
            return default(T);

        }



        public static T FindAncestor<T>(this UIElement obj) where T : UIElement
        {
            return FindAncestor<T>((DependencyObject)obj);
        }

        //http://stackoverflow.com/questions/665719/wpf-animate-listbox-scrollviewer-horizontaloffset
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            // Search immediate children first (breadth-first)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is T)
                    return (T)child;

                else
                {
                    T childOfChild = FindVisualChild<T>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
    }


}
