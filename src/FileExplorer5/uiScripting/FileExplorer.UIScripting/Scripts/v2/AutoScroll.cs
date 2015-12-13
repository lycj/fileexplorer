using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        /// <summary>
        /// Perform some scrolling (LineLeft/Right/Up/Down) when scroll outside bounds, 
        /// Only work with Panel that support IScrollInfo.
        /// </summary>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AutoScroll(IScriptCommand nextCommand = null)
        {
            return new AutoScroll()
            {
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }


    public class AutoScroll : UIScriptCommandBase<Control, RoutedEventArgs>
    {

        /// <summary>
        /// Current Position relative to Scp.
        /// </summary>
        public string CurrentRelativePositionKey { get; set; }

        public AutoScroll()
            : base("AutoScroll")
        {
            CurrentRelativePositionKey = "{CurrentRelativePosition}";
        }

        protected override Script.IScriptCommand executeInner(ParameterDic pm, Control sender, 
            RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            var scp = ControlUtils.GetScrollContentPresenter(sender);

            if (input != null)
            {
                Point posRelToScp = pm.GetValue<Point>(CurrentRelativePositionKey);                
                IScrollInfo isInfo = UITools.FindVisualChild<Panel>(scp) as IScrollInfo;
                if (isInfo != null)
                {
                    if (isInfo.CanHorizontallyScroll)
                        if (posRelToScp.X < 0)
                            isInfo.LineLeft();
                        else if (posRelToScp.X > (isInfo as Panel).ActualWidth)
                            isInfo.LineRight();
                    if (isInfo.CanVerticallyScroll)
                        if (posRelToScp.Y < 0)
                            isInfo.LineUp();
                        else if (posRelToScp.Y > (isInfo as Panel).ActualHeight) //isInfo.ViewportHeight is bugged.
                            isInfo.LineDown();
                }
            }
            return NextCommand;
        }

    }
}
