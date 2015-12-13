using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        /// <summary>
        /// FindSelectedItems If the ItemsPanel that support IChildInfo, required {SelectionBoundAdjusted}.
        /// </summary>
        public static IScriptCommand FindSelectedItemsUsingIChildInfo(IScriptCommand nextCommand = null)
        {
            return new FindSelectedItemsUsingIChildInfo() { NextCommand = (ScriptCommandBase)nextCommand };
        }

        /// <summary>
        /// FindSelectedItems If the View is GridView, required {CurrentRelativePosition}.
        /// </summary>
        public static IScriptCommand FindSelectedItemsUsingGridView(IScriptCommand nextCommand = null)
        {
            return new FindSelectedItemsUsingGridView() { NextCommand = (ScriptCommandBase)nextCommand };
        }

        /// <summary>
        /// FindSelectedItems using HitTest, required {SelectionBound}.
        /// </summary>
        public static IScriptCommand FindSelectedItemsUsingHitTest(IScriptCommand nextCommand = null)
        {
            return new FindSelectedItemsUsingHitTest() { NextCommand = (ScriptCommandBase)nextCommand};
        }
    }

    public enum FindSelectionMode { IChildInfo, GridView, HitTest }     

    public abstract class FindSelectedItemsImplBase : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {
        public string SelectedListKey { get; set; }
        public string SelectedIdListKey { get; set; }
        //public string UnselectedListKey { get; set; }
        //public string UnselectedIdListKey { get; set; }        

        public FindSelectedItemsImplBase(FindSelectionMode findSelectionMode)
            : base("FindSelectedItemsUsing" + findSelectionMode.ToString())
        {
            SelectedListKey = "{SelectedList}";
            SelectedIdListKey = "{SelectedIdList}";

            //UnselectedListKey = "{UnselectedList}";
            //UnselectedIdListKey = "{UnselectedIdList}";
        }
    }

    /// <summary>
    /// FindSelectedItems If the ItemsPanel that support IChildInfo, required {SelectionBoundAdjusted}.
    /// </summary>
    internal class FindSelectedItemsUsingIChildInfo : FindSelectedItemsImplBase
    {
        /// <summary>
        ///SelectionBounds (Rect) that used to calcuate selected items, took scroll bar position into account.                       
        /// </summary>
        public string SelectionBoundAdjustedKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FindSelectedItemsUsingIChildInfo>();

        public FindSelectedItemsUsingIChildInfo() :
            base(FindSelectionMode.IChildInfo)
        {
            SelectionBoundAdjustedKey = "{SelectionBoundAdjusted}";
        }


        protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            var scp = ControlUtils.GetScrollContentPresenter(ic);
            IChildInfo icInfo = UITools.FindVisualChild<Panel>(scp) as IChildInfo;
            if (icInfo == null)
                return ResultCommand.Error(new NotSupportedException());

            Rect selectionBound = pm.GetValue<Rect>(SelectionBoundAdjustedKey);
            List<object> selectedList = new List<object>();
            List<int> selectedIdList = new List<int>();

            for (int i = 0; i < ic.Items.Count; i++)
                if (icInfo.GetChildRect(i).IntersectsWith(selectionBound))
                {
                    selectedList.Add(ic.Items[i]);
                    selectedIdList.Add(i);
                }

            pm.SetValue(SelectedListKey, selectedList);
            pm.SetValue(SelectedIdListKey, selectedIdList);
            logger.Debug(String.Format("Selected = {0}", selectedIdList.Count()));
           
            return NextCommand;
        }

    }


    /// <summary>
    /// FindSelectedItems If the View is GridView, required {CurrentRelativePosition}.
    /// </summary>
    internal class FindSelectedItemsUsingGridView : FindSelectedItemsImplBase
    {
        /// <summary>
        /// Current Position relative to Scp.
        /// </summary>
        public string CurrentRelativePositionKey { get; set; }


        public FindSelectedItemsUsingGridView()
            : base(FindSelectionMode.GridView)
        {
            CurrentRelativePositionKey = "{CurrentRelativePosition}";
        }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FindSelectedItemsUsingGridView>();


        protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic,
            RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            Point posRelToScp = pm.GetValue<Point>(CurrentRelativePositionKey);
            var startSelected = UIEventHubProperties.GetStartSelectedItem(ic);
            List<object> selectedList = new List<object>();
            List<int> selectedIdList = new List<int>();
            var scp = ControlUtils.GetScrollContentPresenter(ic);

            var currentSelected = UITools.GetSelectedListBoxItem(scp, posRelToScp);
            if (startSelected != null && currentSelected != null)
            {
                int startIdx = ic.ItemContainerGenerator.IndexFromContainer(startSelected);
                int endIdx = ic.ItemContainerGenerator.IndexFromContainer(currentSelected);

                for (int i = Math.Min(startIdx, endIdx); i <= Math.Max(startIdx, endIdx); i++)
                {
                    selectedList.Add(ic.Items[i]);
                    selectedIdList.Add(i);
                }
            }

            //UpdateStartSelectedItems, or clear it if no longer selecting.
            if (UIEventHubProperties.GetIsSelecting(ic))
            {
                if (UIEventHubProperties.GetStartSelectedItem(ic) == null)
                {
                    var itemUnderMouse = UITools.GetSelectedListBoxItem(scp, posRelToScp);
                    UIEventHubProperties.SetStartSelectedItem(ic, itemUnderMouse);
                }
            }
            else
                UIEventHubProperties.SetStartSelectedItem(ic, null);

            if (UIEventHubProperties.GetIsSelecting(ic))
            {
                if (UIEventHubProperties.GetStartSelectedItem(ic) == null)
                    UITools.SetItemUnderMouseToAttachedProperty(ic, posRelToScp,
                        UIEventHubProperties.StartSelectedItemProperty);
            }

            pm.SetValue(SelectedListKey, selectedList);
            pm.SetValue(SelectedIdListKey, selectedIdList);
            logger.Debug(String.Format("Selected = {0}", selectedIdList.Count()));
            return NextCommand;
        }
    }

    /// <summary>
    /// FindSelectedItems using HitTest, required {SelectionBound}.
    /// </summary>
    internal class FindSelectedItemsUsingHitTest : FindSelectedItemsImplBase
    {
        /// <summary>
        /// SelectionBounds (Rect) on visual, regardless the scrollbar.
        /// </summary>
        public string SelectionBoundKey { get; set; }
        //public bool IsHighlighting { get; set; }

        public FindSelectedItemsUsingHitTest() :
            base(FindSelectionMode.HitTest)
        { SelectionBoundKey = "{SelectionBound}"; }

        private static Rect _prevBound = new Rect(0, 0, 0, 0);

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FindSelectedItemsUsingHitTest>();

        private static HitTestResultBehavior selectResultCallback(HitTestResult result)
        {
            return HitTestResultBehavior.Continue;
        }

        protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            Rect selectionBound = pm.GetValue<Rect>(SelectionBoundKey);
            var scp = ControlUtils.GetScrollContentPresenter(ic);                        

            List<object> selectedList = new List<object>();
            List<int> selectedIdList = new List<int>();
            List<object> unselectedList = new List<object>();
            List<int> unselectedIdList = new List<int>();

            Func<HitTestResult, HitTestResultBehavior> cont = (result) => HitTestResultBehavior.Continue;
            #region selectFilter
            HitTestFilterCallback selectFilter = (HitTestFilterCallback)((potentialHitTestTarget) =>
            {
                var frameworkElement = potentialHitTestTarget as FrameworkElement;
                if (frameworkElement != null && frameworkElement.DataContext is ISelectable && 
                    !frameworkElement.DataContext.Equals(ic.DataContext))
                {
                    selectedList.Add(potentialHitTestTarget);
                    int id = ic.ItemContainerGenerator.IndexFromContainer(potentialHitTestTarget);
                    if (id != -1)
                        selectedIdList.Add(id);

                    if (unselectedList.Contains(potentialHitTestTarget)) unselectedList.Remove(potentialHitTestTarget);
                    if (unselectedIdList.Contains(id)) unselectedIdList.Remove(id);
                    return HitTestFilterBehavior.ContinueSkipChildren;
                }
                return HitTestFilterBehavior.Continue;
            });
            #endregion

            #region unselectFilter
            HitTestFilterCallback unselectFilter = (HitTestFilterCallback)((potentialHitTestTarget) =>
            {
                var frameworkElement = potentialHitTestTarget as FrameworkElement;
                if (frameworkElement != null && frameworkElement.DataContext is ISelectable &&
                    !frameworkElement.DataContext.Equals(ic.DataContext))
                {
                    unselectedList.Add(potentialHitTestTarget);
                    unselectedIdList.Add(ic.ItemContainerGenerator.IndexFromContainer(potentialHitTestTarget));
                    return HitTestFilterBehavior.ContinueSkipChildren;
                }
                return HitTestFilterBehavior.Continue;
            });
            #endregion


            //Unselect all visible selected items (by using _lastPos) no matter it's current selected or not.
            VisualTreeHelper.HitTest(ic, unselectFilter,
                new HitTestResultCallback(cont),
                new GeometryHitTestParameters(new RectangleGeometry(_prevBound)));

            //Select all visible items in select region.
            VisualTreeHelper.HitTest(ic, selectFilter,
                new HitTestResultCallback(cont),
                new GeometryHitTestParameters(new RectangleGeometry(selectionBound)));

            _prevBound = selectionBound;

            if (ic.DataContext is IContainer<ISelectable>)
            {
                var childItems = (ic.DataContext as IContainer<ISelectable>).GetChildItems().ToList();

                //var prevSelctionIds = IsHighlighting ? 
                //    childItems.Where(s => s.IsSelecting).Select(s => childItems.IndexOf(s)).ToArray() : 
                //    childItems.Where(s => s.IsSelected).Select(s => childItems.IndexOf(s)).ToArray();
                    
                //for (int i = 0; i < ic.Items.Count; i++)
                //    if (!(selectedIdList.Contains(i)) && //already selected, skip.
                //        !(unselectedIdList.Contains(i)) &&  //not selected, skip.
                //        allSelectedItems.Contains(ic.Items[i]))
                //    {                        
                //        selectedIdList.Add(i);
                //    }
                //Console.WriteLine(unselectedIdList.Count);
                selectedIdList = selectedIdList.SkipWhile(i => unselectedIdList.Contains(i)).ToList();
            }
            else
            for (int i = 0; i < ic.Items.Count; i++)
                if (!(selectedIdList.Contains(i)) && //already selected, skip.
                    !(unselectedIdList.Contains(i)))  //not selected, skip.
            {
                
                FrameworkElement item = ic.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                if (item != null && (item.DataContext as ISelectable).IsSelected)
                {
                    selectedList.Add(item);
                    selectedIdList.Add(i);
                }                
            }

            //pm.SetValue(SelectedListKey, selectedList);
            pm.SetValue(SelectedIdListKey, selectedIdList);
            logger.Debug(String.Format("Selected = {0}", selectedIdList.Count()));      
            return NextCommand;
        }


    }

}
