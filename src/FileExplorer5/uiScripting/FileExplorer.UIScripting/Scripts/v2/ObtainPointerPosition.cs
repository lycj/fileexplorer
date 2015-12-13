using FileExplorer.Script;
using FileExplorer.UIEventHub;
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
        public static IScriptCommand ObtainPointerPosition(IScriptCommand nextCommand = null)
        {
            return new ObtainPointerPosition()
            {
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }


    public class ObtainPointerPosition : UIScriptCommandBase<FrameworkElement, RoutedEventArgs>
    {
        ///// <summary>
        ///// If set, Point to output of GridViewHeader width and length (Size), if it's exists.
        ///// Default = "{GridViewHeaderSize}"
        ///// </summary>
        //public string GridViewHeaderSizeKey { get; set; }

        /// <summary>
        /// Start position relative to sender, regardless the scrollbar.
        /// </summary>
        public string StartPositionKey { get; set; }
        /// <summary>
        /// Start scrollbar position.
        /// </summary>
        public string StartScrollbarPositionKey { get; set; }

        /// <summary>
        /// Start position relative to sender, adjusted with scrollbar position.
        /// </summary>
        public string StartPositionAdjustedKey { get; set; }


        /// <summary>
        /// Current position relative to sender, regardless the scrollbar.
        /// </summary>
        public string CurrentPositionKey { get; set; }
        /// <summary>
        /// Current scrollbar position.
        /// </summary>
        public string CurrentScrollbarPositionKey { get; set; }

        /// <summary>
        /// Current position relative to sender, adjusted with scrollbar position.
        /// </summary>
        public string CurrentPositionAdjustedKey { get; set; }

        /// <summary>
        /// Current Position relative to Scp.
        /// </summary>
        public string CurrentRelativePositionKey { get; set; }
        /// <summary>
        /// SelectionBounds (Rect) on visual, regardless the scrollbar.
        /// </summary>
        public string SelectionBoundKey { get; set; }
        /// <summary>
        ///SelectionBounds (Rect) that used to calcuate selected items, took scroll bar position into account.                       
        /// </summary>
        public string SelectionBoundAdjustedKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ObtainPointerPosition>();

        public ObtainPointerPosition()
            : base("ObtainPointerPosition")
        {
            StartPositionKey = "{StartPosition}";
            StartScrollbarPositionKey = "{StartScrollbarPosition}";
            StartPositionAdjustedKey = "{StartPositionAdjusted}";
            //StartVisualPositionKey = "{StartVisualPosition}";
            CurrentPositionKey = "{CurrentPosition}";
            CurrentScrollbarPositionKey = "{CurrentScrollbarPosition}";
            CurrentRelativePositionKey = "{CurrentRelativePosition}";
            CurrentPositionAdjustedKey = "{CurrentPositionAdjusted}";
            SelectionBoundKey = "{SelectionBound}";
            SelectionBoundAdjustedKey = "{SelectionBoundAdjusted}";
        }

        private Point add(params Point[] pts)
        {
            Point retVal = new Point(0, 0);
            foreach (var pt in pts)
                retVal = new Point(retVal.X + pt.X, retVal.Y + pt.Y);
            return retVal;
        }

        public static Point AdjustScrollBarPosition(Point pt, Point startScrollbarPosition, Point currentScrollbarPosition)
        {
            return new Point(pt.X - currentScrollbarPosition.X + startScrollbarPosition.X,
                pt.Y - currentScrollbarPosition.Y + startScrollbarPosition.Y);
        }

        //protected virtual Point AdjustHeaderPosition(Point point, ParameterDic pd,
        //    Size gridViewHeaderSize, 
        //    int offset = -1)
        //{
        //    Point pt = new Point(point.X, point.Y);
        //    //pt.Offset(0, offset * ((Size)pd["ContentBelowHeaderSize"]).Height);
        //    ////Deduct Grid View Header from the position.
        //    pt.Offset(0, offset * gridViewHeaderSize.Height);
        //    //return pt;
        //    return point;
        //}

        protected virtual Point AdjustHeaderPosition(Point point, ParameterDic pd, Point scpRelativePos,
           int offset = -1)
        {
            Point pt = new Point(point.X, point.Y);
            ////Deduct Grid View Header and other contents from the position.            
            pt.Offset(offset * scpRelativePos.X, offset * scpRelativePos.Y);
            return pt;
            //return point;
        }

        protected override IScriptCommand executeInner(ParameterDic pm, FrameworkElement ele,
            RoutedEventArgs eventArgs, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            var scp = ControlUtils.GetScrollContentPresenter(ele is Control ? (Control)ele : UITools.FindAncestor<Control>(ele));            
            var scpRelativePos = scp == null ? new Point(0,0) : scp.TranslatePoint(new Point(0, 0), ele);
            var gvhrp = UITools.FindVisualChild<GridViewHeaderRowPresenter>(ele);

            DragInputProcessor dragInpProc = inpProcs.Where(p => p is DragInputProcessor).Cast<DragInputProcessor>().First();

            var startPos = AdjustHeaderPosition(dragInpProc.StartInput.Position, pm, scpRelativePos, -1);
            var startScrollbarPos = dragInpProc.StartInput.ScrollBarPosition;
            var currentPos = AdjustHeaderPosition(input.Position, pm, scpRelativePos, -1);
            var currentRelativePos = input.PositionRelativeTo(scp);            
            var currentScrollbarPos = input.ScrollBarPosition;
            var startAdjustedPos = add(startPos, startScrollbarPos);
            var currentAdjustedPos = add(currentPos, currentScrollbarPos);

            //var startVisualPos = AdjustScrollBarPosition(startPos, //for used in visual only.
            //    startScrollbarPos, currentScrollbarPos);                    

            pm.SetValue(StartPositionKey, startPos);
            pm.SetValue(StartScrollbarPositionKey, startScrollbarPos);
            pm.SetValue(StartPositionAdjustedKey, startAdjustedPos);

            pm.SetValue(CurrentPositionKey, currentPos);
            pm.SetValue(CurrentScrollbarPositionKey, currentPos);
            pm.SetValue(CurrentRelativePositionKey, currentRelativePos);
            pm.SetValue(CurrentPositionAdjustedKey, currentAdjustedPos);

            pm.SetValue(SelectionBoundKey, new Rect(startPos, currentPos));
            pm.SetValue(SelectionBoundAdjustedKey, new Rect(startAdjustedPos,
                currentAdjustedPos));

            logger.Debug(String.Format("{0}", new Rect(startPos, currentPos)));

            return _nextCommand;
        }

    }
}
