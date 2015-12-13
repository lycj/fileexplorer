using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        /// <summary>
        /// Serializable, Set cursor during UIElement.GiveFeedback/QueryCursor event.
        /// </summary>
        /// <example>HubScriptCommands.SetCustomCursor(Cursors.No)</example>
        /// <param name="cursor"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand SetCustomCursor(Cursor cursor, IScriptCommand nextCommand = null)
        {
            return new SetCustomCursor()
            {
                CursorType = cursor,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Serializable, Set cursor during UIElement.GiveFeedback event.
    /// </summary>
    public class SetCustomCursor : UIScriptCommandBase<UIElement, RoutedEventArgs>
    {
        public Cursor CursorType { get; set; }

        public SetCustomCursor()
            : base("SetCustomCursor")
        {

        }

        protected override IScriptCommand executeInner(ParameterDic pm, UIElement sender, RoutedEventArgs evnt, IUIInput input, IList<IUIInputProcessor> inpProcs)
        {
            if (evnt is GiveFeedbackEventArgs)
            {
                GiveFeedbackEventArgs gevnt = evnt as GiveFeedbackEventArgs;
                if (CursorType != null)
                {
                    gevnt.UseDefaultCursors = false;
                    Mouse.SetCursor(CursorType);
                    gevnt.Handled = true;
                }
                else gevnt.UseDefaultCursors = true;
            }
            else if (evnt is QueryCursorEventArgs)
            {
                QueryCursorEventArgs qevnt = evnt as QueryCursorEventArgs;
                if (CursorType != null)
                {
                    qevnt.Cursor = CursorType;
                    qevnt.Handled = true;
                }
            }

            return NextCommand;
        }
    }
}
