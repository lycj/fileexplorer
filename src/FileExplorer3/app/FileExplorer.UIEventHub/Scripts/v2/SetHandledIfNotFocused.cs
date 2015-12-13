using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FileExplorer.UIEventHub;
using System.Windows;
using FileExplorer.Script;
using MetroLog;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand SetHandledIfNotFocused(IScriptCommand nextCommand = null)
        {
            return new SetHandledIfNotFocused()
            {
                 NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }


    /// <summary>
    /// Assume sender is ItemsControl, 
    /// </summary>
    public class SetHandledIfNotFocused : UIScriptCommandBase<ItemsControl, RoutedEventArgs>
    {

        public SetHandledIfNotFocused() 
            : base("SetHandledIfNotFocused")
        {

        }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<SetHandledIfNotFocused>();

        protected override IScriptCommand executeInner(ParameterDic pm, ItemsControl ic, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            var scp = ControlUtils.GetScrollContentPresenter(ic);

            if (!ic.IsKeyboardFocusWithin)
            {
                var itemUnderMouse = UITools.GetItemUnderMouse(ic, input.PositionRelativeTo(scp));

                if ((itemUnderMouse is ListBoxItem && (itemUnderMouse as ListBoxItem).IsSelected) ||
                    (itemUnderMouse is TreeViewItem && (itemUnderMouse as TreeViewItem).IsSelected))
                {
                    ic.Focus();
                    evnt.Handled = true;
                    logger.Debug("Set");
                }
            }

            return NextCommand;
        }
    
    }
}
