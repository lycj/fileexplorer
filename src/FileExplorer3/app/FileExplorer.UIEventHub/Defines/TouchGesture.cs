using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub.Defines
{
    public class TouchGesture
    {
        public UIInputState TouchDevice { get; set; }
        public UITouchGesture TouchAction { get; set; }

        public TouchGesture(UIInputState state = UIInputState.NotApplied, UITouchGesture action = UITouchGesture.NotApplied)
        {
            TouchDevice = state;
            TouchAction = action;
        }

        public override bool Equals(object obj)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return TouchDevice.GetHashCode() + TouchAction.GetHashCode();
        }
    }

    public static partial class ExtensionMethods
    {
        public static TouchGesture GetTouchGesture(this IUIInput input)
        {
            if (input.InputType == UIInputType.Touch)
                return new TouchGesture(input.InputState, input.TouchGesture);
            else return new TouchGesture(UIInputState.NotApplied, UITouchGesture.NotApplied);
        }
    }
}
