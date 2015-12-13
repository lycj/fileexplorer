using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace QuickZip.UserControls
{
    public enum ExtraMouseAction
    {
        Next, 
        Back,
        None
    }

    public class ExtraMouseGesture : InputGesture
    {
        private ExtraMouseAction _extraMouseAction;

        public ExtraMouseAction ExtraMouseAction
        {
            get { return _extraMouseAction; }
            private set { _extraMouseAction = value; }
        }

        public ExtraMouseGesture(ExtraMouseAction action)
            : base()
        {
            _extraMouseAction = action;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (inputEventArgs is MouseButtonEventArgs)
            {
                MouseButtonEventArgs args = (MouseButtonEventArgs)inputEventArgs;
                switch (ExtraMouseAction)
                {
                    case ExtraMouseAction.Back :
                        return args.ChangedButton == MouseButton.XButton1;
                    case ExtraMouseAction.Next :
                        return args.ChangedButton == MouseButton.XButton2;
                }
            }                        
            return false;
        }
    }
}
