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

    public class ClickCountInputProcessor : InputProcessorBase
    {
        #region Constructors

        public ClickCountInputProcessor()
        {

            ProcessAllEvents = false;
            _processEvents.AddRange(new[] { 
                UIElement.StylusSystemGestureEvent,
                UIElement.PreviewTouchDownEvent,
                UIElement.PreviewMouseLeftButtonDownEvent,
                UIElement.PreviewTouchDownEvent
            });
        }

        #endregion

        #region Methods

        public override void Update(ref IUIInput input)
        {



            if (input.EventArgs is MouseButtonEventArgs)
                input.ClickCount = (input.EventArgs as MouseButtonEventArgs).ClickCount;
            else
                if (input.InputType == UIInputType.Touch && input.InputState == UIInputState.Pressed)
                {

                    //touchPts.First().Action == TouchAction.
                    if (DateTime.UtcNow.Subtract(_lastClickTime).TotalMilliseconds <
                        Defaults.MaximumClickInterval &&
                        input.IsWithin(_startInput, Defaults.MaximumTouchClickDragDistance.X,
                        Defaults.MaximumTouchClickDragDistance.Y))
                    {
                        _clickCount += 1;
                        input.ClickCount = _clickCount;

                    }
                    else
                    {
                        _startInput = input;
                        _clickCount = 1;
                    }
                    _lastClickTime = DateTime.UtcNow;
                }
            //else _clickCount = 0;
        }

        #endregion

        #region Data

        private int _clickCount = 1;
        private IUIInput _startInput = InvalidInput.Instance;
        private DateTime _lastClickTime = DateTime.MinValue;

        #endregion

        #region Public Properties



        #endregion
    }
}
