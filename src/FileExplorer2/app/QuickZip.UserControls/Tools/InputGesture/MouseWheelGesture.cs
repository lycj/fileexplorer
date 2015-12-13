using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    //http://cebla5.spaces.live.com/blog/cns!1B8262ED00250003!133.entry
    public enum MouseWheelAction
    {
        AllMovement,
        WheelUp,
        WheelDown
    }

    public class MouseWheelGesture : MouseGesture
    {
        private MouseWheelAction mouseWheelAction;

        public MouseWheelAction MouseWheelAction
        {
            get { return mouseWheelAction; }
            set { mouseWheelAction = value; }
        }

        public MouseWheelGesture()
            : base(MouseAction.WheelClick)
        {
            mouseWheelAction = MouseWheelAction.AllMovement;
        }


        public MouseWheelGesture(MouseWheelAction action)
            : base(MouseAction.WheelClick)
        {
            this.mouseWheelAction = action;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (base.Matches(targetElement, inputEventArgs))
            {
                MouseWheelEventArgs wheelArgs = inputEventArgs as MouseWheelEventArgs;
                if (wheelArgs != null)
                {
                    if (MouseWheelAction == MouseWheelAction.AllMovement
                            || (MouseWheelAction == MouseWheelAction.WheelDown && wheelArgs.Delta < 0)
                            || MouseWheelAction == MouseWheelAction.WheelUp && wheelArgs.Delta > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
