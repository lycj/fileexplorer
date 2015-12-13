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
    public enum DragState { Normal, Touched, Pressed, TouchDragging, Dragging, Released }
    public class DragInputProcessor : InputProcessorBase
    {
        #region Constructors        

        public DragInputProcessor()
        {
            ProcessAllEvents = false;
            _processEvents.AddRange(new[] { 
                UIElement.PreviewMouseDownEvent,
                UIElement.PreviewTouchDownEvent,

                UIElement.MouseMoveEvent,
                UIElement.TouchMoveEvent,

                UIElement.PreviewMouseUpEvent,
                UIElement.PreviewTouchUpEvent
            }
            );
        }

        #endregion

        #region Methods

        public override void Update(ref IUIInput input)
        {

            switch (input.InputState)
            {
                case UIInputState.Pressed:
                    UpdateInputPressed(input);
                    break;
                case UIInputState.Released:
                    UpdatInputReleased(input);
                    break;
                default:
                    UpdateInputPosition(input);
                    break;
            }
            input.IsDragging = IsDragging;
            if (IsDragging)
                input.TouchGesture = UITouchGesture.Drag;
        }

        public void UpdateInputPosition(IUIInput input)
        {
            if (_dragState == DragState.Touched && input.EventArgs is TouchEventArgs)
            {
                if (DateTime.UtcNow.Subtract(_touchTime).TotalMilliseconds >= Defaults.MaximumTouchHoldInterval)
                {
                    var rect = (input.EventArgs as TouchEventArgs).GetTouchPoint(null).Size;
                    if ((input as TouchInput).IsDragThresholdReached(_startTouchInput as TouchInput))
                    {
                        StartInput = _startTouchInput;
                        _dragState = DragState.Pressed;
                        //_touchTime = DateTime.MinValue;
                        //_isDragging = true;
                        //DragStartedFunc(input);
                    }
                    else
                    {
                        _touchTime = DateTime.MinValue;
                        _dragState = DragState.Normal;
                    }
                }

              
            }

            

            //Console.WriteLine(String.Format("UpdateInputPosition - {0}", _dragState));
            if (_dragState == DragState.Pressed && input.IsDragThresholdReached(_startInput))
            {
                _dragState = DragState.Dragging;
                _isDragging = true;                
                DragStartedFunc(input);                
            }
        }

        public void UpdatInputReleased(IUIInput input)
        {
            //Console.WriteLine("UpdatInputReleased -" + input.ToString());
            if (input.IsSameSource(_startInput))
            {
                if (_isDragging && _dragState == DragState.Dragging)
                {
                    DragStoppedFunc(input);                    
                }
                _isDragging = false;
                _dragState = DragState.Released;
            }

            //Console.WriteLine(String.Format("UpdatInputReleased - {0}", _dragState));
        }

        public void UpdateInputPressed(IUIInput input)
        {
            if (_dragState == DragState.Released)
            {
                StartInput = InvalidInput.Instance;
                _dragState = DragState.Normal;
            }

            if (!_isDragging && input.IsValidPositionForLisView(true))
                if (input.ClickCount <= 1) //Touch/Stylus input 's ClickCount = 0
                {
                    //When touch and hold it raise a mouse right click command, skip it.
                    if (_dragState == DragState.Touched && input.InputType == UIInputType.MouseRight)
                        return;
                    //Console.WriteLine(input);
                    StartInput = input;
                    _isDragging = false;
                    switch (input.InputType)
                    {
                        case UIInputType.Touch:
                            _startTouchInput = input;
                            _dragState = DragState.Touched;                            
                            _touchTime = DateTime.UtcNow;
                            //input.EventArgs.Handled = true;
                            break;
                        default:
                            switch (_dragState)
                            {
                                case DragState.Touched:
                                    break;
                                case DragState.Normal:
                                case DragState.Released:
                                    IsDragging = false;
                                    _dragState = DragState.Pressed;
                                    break;
                            }

                            break;
                    }


                }
            //Console.WriteLine(String.Format("UpdateInputPressed - {0}", _dragState));
        }

        #endregion

        #region Data

        private DateTime _touchTime = DateTime.MinValue;
        private DragState _dragState = DragState.Normal;
        private bool _isDragging = false;
        private IUIInput _startInput = InvalidInput.Instance;
        private IUIInput _startTouchInput = InvalidInput.Instance;
        //private Func<Point, Point> _positionAdjustFunc = pt => pt;

        #endregion

        #region Public Properties

        //public Func<Point, Point> PositionAdjustFunc { get { return _positionAdjustFunc; } set { _positionAdjustFunc = value; } }

        public DragState DragState { get { return _dragState; } set { _dragState = value; } }

        public IUIInput StartInput
        {
            get { return _startInput; }
            private set { _startInput = value; /*Console.WriteLine("StartInput =" + value.ToString());*/ }
        }
        public bool IsDragging
        {
            get { return _isDragging; }
            set
            {                
                _isDragging = value;
                if (!value && _dragState == DragState.Dragging)
                    StartInput = InvalidInput.Instance;
                _dragState = DragState.Normal;
            }
        }
        public Action<IUIInput> DragStartedFunc = (currentInput) => { };
        public Action<IUIInput> DragStoppedFunc = (currentInput) => { };


        #endregion
    }
}
