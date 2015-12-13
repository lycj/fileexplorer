using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{

    public static partial class HubScriptCommands
    {

        public static IScriptCommand IfKeyGesture(string keyGestureVariable = "{KeyGesture}",
            IScriptCommand nextCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return new IfKeyGesture()
            {
                KeyGestureKey = keyGestureVariable,
                NextCommand = (ScriptCommandBase)nextCommand,
                OtherwiseCommand = (ScriptCommandBase)otherwiseCommand
            };
        }

        public static IScriptCommand IfKeyGesture(KeyGesture gesture,
            IScriptCommand nextCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string keyGestureVariable = "{IfKeyGesture-Gesture}";
            return
                ScriptCommands.Assign(keyGestureVariable, gesture, false,
                HubScriptCommands.IfKeyGesture(keyGestureVariable, nextCommand, otherwiseCommand));
        }
    }
    public class IfKeyGesture : UIScriptCommandBase
    {
        /// <summary>
        /// Point to a key gesture (KeyGesture) or string based gesture (e.g. Ctrl+C), 
        /// Default=Ctrl+C
        /// </summary>
        public string KeyGestureKey { get; set; }

        public ScriptCommandBase OtherwiseCommand { get; set; }

        public IfKeyGesture()
            : base("IfKeyPressed")
        {
            KeyGestureKey = "{EventArgs.Key}";
        }

        private static KeyGestureConverter converter = new KeyGestureConverter();

        public override IScriptCommand Execute(ParameterDic pm)
        {
            KeyEventArgs keyEvent = pm.GetValue<KeyEventArgs>(RoutedEventArgsKey);
            if (keyEvent == null)
                return OtherwiseCommand;

            object keygestureObject = !KeyGestureKey.StartsWith("{") ? KeyGestureKey : pm.GetValue(KeyGestureKey);
            KeyGesture gesture = null;

            if (keygestureObject is KeyGesture)
                gesture = (KeyGesture)keygestureObject;

            if (keygestureObject is string)
                gesture = (KeyGesture)converter.ConvertFrom(keygestureObject);


            return gesture != null &&
                        keyEvent.Key == gesture.Key && 
                        keyEvent.KeyboardDevice.Modifiers == gesture.Modifiers ?
                NextCommand : OtherwiseCommand;
        }

    }


}
