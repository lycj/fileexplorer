using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.Defines
{
    public static partial class UIEventHubProperties
    {
        #region IsMouseDragging
                
        //public static DependencyProperty IsMouseDraggingProperty =
        //    DependencyProperty.RegisterAttached("IsMouseDragging", typeof(bool), typeof(AttachedProperties));


        //public static bool GetIsMouseDragging(DependencyObject target)
        //{
        //    return (bool)target.GetValue(IsMouseDraggingProperty);
        //}

        //public static void SetIsMouseDragging(DependencyObject target, bool value)
        //{
        //    target.SetValue(IsMouseDraggingProperty, value);
        //}

        #endregion

        public static readonly Point InvalidPoint = new Point(double.NaN, double.NaN);

        //#region StartPosition
        //private static DependencyProperty StartPositionProperty =
        //  DependencyProperty.RegisterAttached("StartPosition", typeof(Point), typeof(AttachedProperties),
        //  new PropertyMetadata(InvalidPoint));

        //public static Point GetStartPosition(DependencyObject obj)
        //{
        //    return (Point)obj.GetValue(StartPositionProperty);
        //}

        //public static void SetStartPosition(DependencyObject obj, Point value)
        //{
        //    obj.SetValue(StartPositionProperty, value);
        //}
        //#endregion

        //#region StartInput
        //private static DependencyProperty StartInputProperty =
        //  DependencyProperty.RegisterAttached("StartInput", typeof(object), typeof(AttachedProperties),
        //  new PropertyMetadata(null));

        //public static object GetStartInput(DependencyObject obj)
        //{
        //    return (object)obj.GetValue(StartInputProperty);
        //}

        //public static void SetStartInput(DependencyObject obj, object value)
        //{
        //    obj.SetValue(StartInputProperty, value);
        //}
        //#endregion

        //#region StartScrollbarPosition
        //private static DependencyProperty StartScrollbarPositionProperty =
        // DependencyProperty.RegisterAttached("StartScrollbarPosition", typeof(Point), typeof(AttachedProperties),
        // new PropertyMetadata(InvalidPoint));

        //public static Point GetStartScrollbarPosition(DependencyObject obj)
        //{
        //    return (Point)obj.GetValue(StartScrollbarPositionProperty);
        //}

        //public static void SetStartScrollbarPosition(DependencyObject obj, Point value)
        //{
        //    obj.SetValue(StartScrollbarPositionProperty, value);
        //}
        //#endregion
    }
}
