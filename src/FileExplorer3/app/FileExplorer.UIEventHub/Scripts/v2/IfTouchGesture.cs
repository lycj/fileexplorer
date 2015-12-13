using FileExplorer.Script;
using FileExplorer.UIEventHub.Defines;
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

        public static IScriptCommand IfTouchGesture(string TouchGestureVariable = "{TouchGesture}",
            IScriptCommand nextCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return new IfTouchGesture()
            {
                TouchGestureKey = TouchGestureVariable,
                NextCommand = (ScriptCommandBase)nextCommand,
                OtherwiseCommand = (ScriptCommandBase)otherwiseCommand
            };
        }

        public static IScriptCommand IfTouchGesture(TouchGesture gesture,
            IScriptCommand nextCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string TouchGestureVariable = "{IfTouchGesture-Gesture}";
            return
                ScriptCommands.Assign(TouchGestureVariable, gesture, false,
                HubScriptCommands.IfTouchGesture(TouchGestureVariable, nextCommand, otherwiseCommand));
        }
    }    

    public class IfTouchGesture : UIScriptCommandBase
    {
        /// <summary>
        /// Point to a Touch gesture (TouchGesture) 
        /// </summary>
        public string TouchGestureKey { get; set; }

        public ScriptCommandBase OtherwiseCommand { get; set; }

        public IfTouchGesture()
            : base("IfTouchGesture")
        {
            TouchGestureKey = "{EventArgs.Key}";
        }
        public override IScriptCommand Execute(ParameterDic pm)
        {
            TouchEventArgs TouchEvent = pm.GetValue<TouchEventArgs>(RoutedEventArgsKey);
            IUIInput input = pm.GetValue<IUIInput>(InputKey);            
            TouchGesture inputGesture = input.GetTouchGesture();
            TouchGesture gesture = pm.GetValue<TouchGesture>(TouchGestureKey);
            
            if (inputGesture == null || gesture == null || inputGesture.TouchDevice == UIInputState.NotApplied)
                return OtherwiseCommand;

            bool match = (gesture.TouchAction == UITouchGesture.NotApplied ||  inputGesture.TouchAction == gesture.TouchAction) &&
                (gesture.TouchDevice == UIInputState.NotApplied || inputGesture.TouchDevice == gesture.TouchDevice);
            return match ? NextCommand : OtherwiseCommand;
        }

    }


}
