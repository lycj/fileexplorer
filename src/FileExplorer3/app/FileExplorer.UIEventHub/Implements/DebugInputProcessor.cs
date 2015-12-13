using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{

    public class DebugInputProcessor : InputProcessorBase
    {
        public DebugInputProcessor()
        {
            ProcessAllEvents = true;
        }

        public override void Update(ref IUIInput input)
        {
            var touchEventArgs = input.EventArgs as TouchEventArgs;
            if (touchEventArgs != null)
            {
                var touchPts = touchEventArgs.GetIntermediateTouchPoints(input.Sender as IInputElement);
                var touchInput = String.Join("", touchPts.Select(tp => tp.Action.ToString()[0]));
                //Console.WriteLine(touchInput);
            }

        }
    }
}
