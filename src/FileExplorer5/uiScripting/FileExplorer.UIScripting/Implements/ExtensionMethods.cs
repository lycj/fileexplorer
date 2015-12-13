using FileExplorer.Defines;
using FileExplorer.UIEventHub.Defines;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.BaseControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{
    public static partial class ExtensionMethods
    {

        /// <summary>
        /// Cursor position relative to inputElement.
        /// </summary>
        /// <param name="inputElement"></param>
        /// <returns></returns>
        public static Point PositionRelativeTo(this IUIInput input, IInputElement inputElement,
            Func<Point, Point> adjustFunc)
        {
            return adjustFunc(input.PositionRelativeTo(inputElement));
        }

        public static bool IsSameInputType(this IUIInput input, IUIInput input2)
        {
            if (input != null && input2 != null)
            {
                var inp1 = input.InputType;
                var inp2 = input2.InputType;
                if (inp1 == UIInputType.MouseLeft || inp1 == UIInputType.MouseRight) inp1 = UIInputType.Mouse;
                if (inp2 == UIInputType.MouseLeft || inp2 == UIInputType.MouseRight) inp2 = UIInputType.Mouse;
                return inp1 == inp2;
            }
            return false;
        }



        public static bool IsSameSource(this IUIInput input, IUIInput input2)
        {
            return input != null && input2 != null &&
                 input.Sender.Equals(input2.Sender);
        }

        private static Size getMiniumDragDistance(UIInputType inputType)
        {
            if (inputType == UIInputType.Touch)
                return new Size(5, 5);
            else return new Size(SystemParameters.MinimumHorizontalDragDistance,
                SystemParameters.MinimumVerticalDragDistance);
        }

        public static bool IsDragThresholdReached(this TouchInput input, TouchInput input2)
        {
            return
                input.IsSameSource(input2) && input.IsValidPositionForLisView(true) &&

                (
                     Math.Abs(input.Position.X - input2.Position.X) < Defaults.MaximumTouchDragThreshold.X &&
                     Math.Abs(input.Position.Y - input2.Position.Y) < Defaults.MaximumTouchDragThreshold.Y
                )
                ;
        }

        public static bool IsDragThresholdReached(this IUIInput input, IUIInput input2)
        {
            var minDragDist = getMiniumDragDistance(input.InputType);

            return
                input.IsSameSource(input2) && input.IsValidPositionForLisView(true) &&
                (
                     Math.Abs(input.Position.X - input2.Position.X) > minDragDist.Width ||
                     Math.Abs(input.Position.Y - input2.Position.Y) > minDragDist.Height
                );
        }

        public static bool IsWithin(this IUIInput input, IUIInput input2, double x, double y)
        {
            return
                input.IsSameSource(input2) && input.IsValidPositionForLisView(true) &&

                (
                     Math.Abs(input.Position.X - input2.Position.X) < x &&
                     Math.Abs(input.Position.Y - input2.Position.Y) < y
                )
                ;
        }

      

        public static void Update(this IEnumerable<IUIInputProcessor> processors, ref IUIInput input)
        {
            foreach (var p in processors)
                if (p.ProcessAllEvents || p.ProcessEvents.Contains(input.EventArgs.RoutedEvent))
                    p.Update(ref input);
        }

        public static bool IsValid(this IUIInput input)
        {
            return IsValidPositionForLisView(input, true);
        }

        public static bool IsValidPositionForLisView(this IUIInput input, bool validIfNotListView = false)
        {
            var sender = input.Sender as ListView;
            if (sender == null)
                return validIfNotListView;
            var originalSource = input.EventArgs.OriginalSource as DependencyObject;
            if (sender == null)
                return false;
            var scp = ControlUtils.GetScrollContentPresenter(sender);
            
            var lvo = UITools.FindAncestor<ListView>(originalSource);
            var scpo = originalSource == sender ? scp : 
                UITools.FindAncestor<ScrollContentPresenter>(originalSource);
            //Make sure return false for ContextMenu items 
            if (lvo == null)
                return false;

            //This is for handling user click in empty area of a panel.
            bool isOverScrollViewer = (originalSource is ScrollViewer) && scpo == null && lvo == sender;
            if (scp == null ||
                //ListViewEx contains Top/RightContent, allow placing other controls in it, this is to avoid that)
                //scp of event listener (the main ListView) is not equals to scp of event source
                (!scp.Equals(scpo) && !isOverScrollViewer))
                return false;


            bool isOverGridViewHeader = UITools.FindAncestor<GridViewColumnHeader>(originalSource) != null;
            bool isOverScrollBar = UITools.FindAncestor<ScrollBar>(originalSource) != null;
            if (isOverGridViewHeader || isOverScrollBar)
                return false;

            return true;
        }

        public static ISelectable ToISelectable(object viewModel, ListBoxItem lbm)
        {
            if (viewModel is ISelectable)
                return viewModel as ISelectable;
            else
                if (lbm.DataContext is ISelectable)
                    return lbm.DataContext as ISelectable;
                else return new LBSelectable(lbm);
        }
    }
}
