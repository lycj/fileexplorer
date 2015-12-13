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

namespace FileExplorer.UIEventHub
{

    public static partial class HubScriptCommands
    {
        /// <summary>
        /// Find the item (DataContext) directly under mouse and assign to a variable in ParameterDic.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AssignItemUnderMouse(string variable = "{Variable}", bool skipIfExists = false, 
            IScriptCommand nextCommand = null)
        {
            return new AssignItemUnderMouse()
            {
                VariableKey = variable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }


    /// <summary>
    /// Find the item (DataContext) directly under mouse and assign to a variable in ParameterDic.
    /// </summary>
    public class AssignItemUnderMouse : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {
        /// <summary>
        /// Variable name to set to, default = "Variable".
        /// </summary>
        public string VariableKey { get; set; }

        /// <summary>
        /// Current Position relative to Scp.
        /// </summary>
        public string CurrentRelativePositionKey { get; set; }

        /// <summary>
        /// Whether skip (or override) if key already in dictionary, default = false.
        /// </summary>
        public bool SkipIfExists { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<AssignItemUnderMouse>();

        public AssignItemUnderMouse()
            : base("AssignItemUnderMouse")
        {
            VariableKey = "{Variable}";
            CurrentRelativePositionKey = "{CurrentRelativePosition}";
            SkipIfExists = false;
        }

        protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic, 
            RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            Point currentRelPos = pm.GetValue<Point>(CurrentRelativePositionKey);
            if (currentRelPos.X == 0 && currentRelPos.Y == 0)
                logger.Warn("CurrentRelativePosition not set.");

            FrameworkElement eleUnderMouse = UITools.GetItemUnderMouse(ic, currentRelPos);
            if (eleUnderMouse != null)
            {
                return ScriptCommands.Assign(VariableKey, eleUnderMouse.DataContext, SkipIfExists, NextCommand);
            }
            else return ScriptCommands.Assign(VariableKey, null, SkipIfExists, NextCommand); ;
        }

    }



    ///// <summary>
    ///// Find the item (DataContext) directly under mouse and assign to a variable in ParameterDic.
    ///// </summary>
    //public class AssignItemUnderMouse<T> : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    //{
    //    /// <summary>
    //    /// Variable name to set to, default = "Variable".
    //    /// </summary>
    //    public string VariableKey { get; set; }

    //    /// <summary>
    //    /// Current Position relative to Scp.
    //    /// </summary>
    //    public string CurrentRelativePositionKey { get; set; }

    //    /// <summary>
    //    /// Whether skip (or override) if key already in dictionary, default = false.
    //    /// </summary>
    //    public bool SkipIfExists { get; set; }


    //    public AssignItemUnderMouse()
    //        : base("AssignItemUnderMouse")
    //    {
    //        VariableKey = "{Variable}";
    //        CurrentRelativePositionKey = "{CurrentRelativePosition}";
    //        SkipIfExists = false;
    //    }

    //    protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic,
    //        RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
    //    {
    //        Point currentRelPos = pm.GetValue<Point>(CurrentRelativePositionKey);
    //        FrameworkElement eleUnderMouse = UITools.GetItemUnderMouse(ic, currentRelPos);
    //        if (eleUnderMouse != null)
    //        {
    //            return ScriptCommands.Assign(VariableKey, eleUnderMouse.DataContext, SkipIfExists, NextCommand);
    //        }
    //        else return ScriptCommands.Assign(VariableKey, null, SkipIfExists, NextCommand); ;
    //    }

    //}
}
