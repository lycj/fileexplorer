using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.WPF.BaseControls;
using FileExplorer.UIEventHub;
using FileExplorer.Models;

namespace FileExplorer.Defines
{
    public static partial class UIEventHubProperties
    {
        #region DragAdorner

        public static readonly DependencyProperty DragAdornerProperty =
         DependencyProperty.RegisterAttached("DragAdorner", typeof(DragAdorner), typeof(UIEventHubProperties), new UIPropertyMetadata(null));

        public static DragAdorner GetDragAdorner(DependencyObject obj)
        {
            return (DragAdorner)obj.GetValue(DragAdornerProperty);
        }

        public static void SetDragAdorner(DependencyObject obj, DragAdorner value)
        {
            obj.SetValue(DragAdornerProperty, value);
        }


        #endregion

        #region StartDraggingItem

        /// <summary>
        /// DataContext item when start dragging.
        /// </summary>
        public static DependencyProperty StartDraggingItemProperty =
          DependencyProperty.RegisterAttached("StartDraggingItem", typeof(object), typeof(UIEventHubProperties));

        public static object GetStartDraggingItem(DependencyObject target)
        {
            return (object)target.GetValue(StartDraggingItemProperty);
        }

        public static void SetStartDraggingItem(DependencyObject target, object value)
        {
            target.SetValue(StartDraggingItemProperty, value);
        }
        #endregion

        #region DragItemTemplate
        public static readonly DependencyProperty DragItemTemplateProperty =
                 DependencyProperty.RegisterAttached("DragItemTemplate", typeof(DataTemplate), typeof(UIEventHubProperties),
                 new UIPropertyMetadata(null));


        public static DataTemplate GetDragItemTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DragItemTemplateProperty);
        }

        public static void SetDragItemTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DragItemTemplateProperty, value);
        }
        #endregion

        #region IsDragging

        public static DependencyProperty IsDraggingProperty =
       DependencyProperty.RegisterAttached("IsDragging", typeof(bool), typeof(UIEventHubProperties), new PropertyMetadata(false));


        public static bool GetIsDragging(DependencyObject target)
        {
            return (bool)target.GetValue(IsDraggingProperty);
        }

        public static void SetIsDragging(DependencyObject target, bool value)
        {
            target.SetValue(IsDraggingProperty, value);
        }

        #endregion


        
        #region DragMethod

        /// <summary>
        /// This is assigned in DoDragDrop, this attached property is set to sender control.
        /// EndDrag access this, if it's not None (drag to elsewhere), 
        /// </summary>
        public static DependencyProperty DragMethodProperty =
       DependencyProperty.RegisterAttached("DragMethod", typeof(DragMethod), typeof(UIEventHubProperties), new PropertyMetadata(DragMethod.Normal));


        public static DragMethod GetDragMethod(DependencyObject target)
        {
            return (DragMethod)target.GetValue(DragMethodProperty);
        }

        public static void SetDragMethod(DependencyObject target, DragMethod value)
        {
            target.SetValue(DragMethodProperty, value);
        }

        #endregion

       // [Flags]
       // public enum DropMethod : int { Normal, Menu }
       // #region DropMethod

       // /// <summary>
       // /// This is assigned in DoDragDrop, this attached property is set to sender control.
       // /// EndDrag access this, if it's not None (drag to elsewhere), 
       // /// </summary>
       // public static DependencyProperty DropMethodProperty =
       //DependencyProperty.RegisterAttached("DropMethod", typeof(DropMethod), typeof(AttachedProperties), new PropertyMetadata(DropMethod.Normal));


       // public static DropMethod GetDropMethod(DependencyObject target)
       // {
       //     return (DropMethod)target.GetValue(DropMethodProperty);
       // }

       // public static void SetDropMethod(DependencyObject target, DropMethod value)
       // {
       //     target.SetValue(DropMethodProperty, value);
       // }

       // #endregion


        public enum DragState { None, Drag, Menu, Drop }
        #region DragState

        /// <summary>
        /// This is assigned in DoDragDrop, this attached property is set to sender control.
        /// EndDrag access this, if it's not None (drag to elsewhere), 
        /// </summary>
        public static DependencyProperty DragStateProperty =
       DependencyProperty.RegisterAttached("DragState", typeof(DragState), typeof(UIEventHubProperties), new PropertyMetadata(DragState.None));


        public static DragState GetDragState(DependencyObject target)
        {
            return (DragState)target.GetValue(DragStateProperty);
        }

        public static void SetDragState(DependencyObject target, DragState value)
        {
            target.SetValue(DragStateProperty, value);
        }

        #endregion

        public enum DropState
        {
            /// <summary>
            /// Not in drop state.
            /// </summary>
            None,
            /// <summary>
            /// Item is dropping (IsupportDrop.Drop is calling)
            /// </summary>
            Drop,
            /// <summary>
            /// Menu is displaying, continue drag drop loop (in DoDragDrop.OnQueryContinueDrag).
            /// </summary>
            Menu
        }
        #region DropState

        /// <summary>
        /// This is assigned in ContinueDrop, which shows a menu
        /// EndDrag access this, if it's not None (drag to elsewhere), 
        /// </summary>
        public static DependencyProperty DropStateProperty =
            DependencyProperty.RegisterAttached("DropState", typeof(DropState), typeof(UIEventHubProperties),
                new PropertyMetadata(DropState.None));


        public static DropState GetDropState(DependencyObject target)
        {
            return (DropState)target.GetValue(DropStateProperty);
        }

        public static void SetDropState(DependencyObject target, DropState value)
        {
            target.SetValue(DropStateProperty, value);
        }

        #endregion


        #region IsDraggingOver

        public static DependencyProperty IsDraggingOverProperty =
       DependencyProperty.RegisterAttached("IsDraggingOver", typeof(bool), typeof(UIEventHubProperties), new PropertyMetadata(false));


        public static bool GetIsDraggingOver(DependencyObject target)
        {
            return (bool)target.GetValue(IsDraggingOverProperty);
        }

        public static void SetIsDraggingOver(DependencyObject target, bool value)
        {
            target.SetValue(IsDraggingOverProperty, value);
        }

        #endregion

        #region DraggingOverItem

        public static DependencyProperty DraggingOverItemProperty =
       DependencyProperty.RegisterAttached("DraggingOverItem", typeof(FrameworkElement), typeof(UIEventHubProperties),
       new PropertyMetadata(null, OnLastDraggingOverPropertyChange));

        public static void OnLastDraggingOverPropertyChange
            (DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                SetIsDraggingOver(e.OldValue as DependencyObject, false);
            if (e.NewValue != null)
                SetIsDraggingOver(e.NewValue as DependencyObject, true);
        }

        public static object GetDraggingOverItem(DependencyObject target)
        {
            return (FrameworkElement)target.GetValue(DraggingOverItemProperty);
        }

        public static void SetDraggingOverItem(DependencyObject target, FrameworkElement value)
        {
            target.SetValue(DraggingOverItemProperty, value);
        }

        #endregion

        #region PreviousDraggables

        public static DependencyProperty SelectedDraggablesProperty =
       DependencyProperty.RegisterAttached("SelectedDraggables", typeof(IEnumerable<IDraggable>),
       typeof(UIEventHubProperties), new PropertyMetadata(null));


        public static IEnumerable<IDraggable> GetSelectedDraggables(DependencyObject target)
        {
            return (IEnumerable<IDraggable>)target.GetValue(SelectedDraggablesProperty);
        }

        public static void SetSelectedDraggables(DependencyObject target, IEnumerable<IDraggable> value)
        {
            target.SetValue(SelectedDraggablesProperty, value);
        }
        #endregion

    }
}
