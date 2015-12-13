using FileExplorer.Defines;
using FileExplorer.UIEventHub.Defines;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileExplorer.UIEventHub
{
    public interface IUIInput
    {
        /// <summary>
        /// The event this input based on.
        /// </summary>
        RoutedEventArgs EventArgs { get; }

        object Sender { get; set; }

        int ClickCount { get; set; }

        /// <summary>
        /// Cursor position relative to the sender.
        /// </summary>
        Point Position { get; }

        Point ScrollBarPosition { get; }

        /// <summary>
        /// Cursor position relative to inputElement.
        /// </summary>
        /// <param name="inputElement"></param>
        /// <returns></returns>
        Point PositionRelativeTo(IInputElement inputElement);

        /// <summary>
        /// The pressed button (e.g. touch or mouseLeft), None if just move.
        /// </summary>
        UIInputType InputType { get; }

        /// <summary>
        /// Whether Pressed or released or not apply.
        /// </summary>
        UIInputState InputState { get; }

        bool IsDragging { get; set; }
        UIInputState Touch { get; set; }
        UITouchGesture TouchGesture { get; set; }
    }

    public abstract class UIInputBase : IUIInput
    {
        #region Constructors

        protected UIInputBase(object sender, RoutedEventArgs args)
        {
            _sender = sender;
            _args = args;
            ScrollBarPosition = ControlUtils.GetScrollbarPosition(sender as Control);

        }

        public static IUIInput FromEventArgs(object sender, RoutedEventArgs args)
        {
            if (args is MouseButtonEventArgs)
                return new MouseInput(sender, args as MouseButtonEventArgs);
            if (args is MouseEventArgs)
                return new MouseInput(sender, args as MouseEventArgs);
            if (args is TouchEventArgs)
                return new TouchInput(sender, args as TouchEventArgs);
            if (args is StylusEventArgs)
                return new StylusInput(sender, args as StylusEventArgs);

            return new OtherInput(sender, args);
        }

        #endregion

        #region Methods

        public abstract Point PositionRelativeTo(IInputElement inputElement);

        public override string ToString()
        {
            if (_args == null)
                return "Invalid";
            return String.Format("{0} {1} {2} {3}", _args.RoutedEvent.Name,
                _inputType, _inputState, _position);
        }

        #endregion

        #region Data

        protected RoutedEventArgs _args;
        protected int _clickCount = 0;
        protected Point _position = UIEventHubProperties.InvalidPoint;
        protected Point _sbPosition = UIEventHubProperties.InvalidPoint;
        protected UIInputType _inputType = UIInputType.None;
        protected UIInputState _inputState = UIInputState.NotApplied;
        protected UIInputState _touchInputState = UIInputState.NotApplied;
        protected UITouchGesture _flickDirection = UITouchGesture.NotApplied;
        private object _sender;
        //private Point _positionScp = AttachedProperties.InvalidPoint;

        #endregion

        #region Public Properties

        public RoutedEventArgs EventArgs { get { return _args; } }
        public object Sender { get { return _sender; } set { _sender = value; } }
        public int ClickCount { get { return _clickCount; } set { _clickCount = value; } }
        public Point ScrollBarPosition { get { return _sbPosition; } set { _sbPosition = value; } }
        public Point Position { get { return _position; } set { _position = value; } }
        //public Point PositionRelativeToScp { get { return _positionScp; } set { _positionScp = value; } }
        public UIInputType InputType { get { return _inputType; } set { _inputType = value; } }
        public UIInputState InputState { get { return _inputState; } set { _inputState = value; } }
        public UIInputState Touch { get { return _touchInputState; } set { _touchInputState = value; } }
        public UITouchGesture TouchGesture { get { return _flickDirection; } set { _flickDirection = value; } }

        public bool IsDragging { get; set; }
        #endregion

    }

    public class OtherInput : UIInputBase
    {
        public OtherInput(object sender, RoutedEventArgs args)
            : base(sender, args)
        {

        }

        public override Point PositionRelativeTo(IInputElement inputElement)
        {
            return UIEventHubProperties.InvalidPoint;
        }
    }

    public class InvalidInput : UIInputBase
    {
        public static InvalidInput Instance = new InvalidInput();
        public InvalidInput()
            : base(null, null)
        { }
        public override Point PositionRelativeTo(IInputElement inputElement)
        {
            return UIEventHubProperties.InvalidPoint;
        }
    }

    public class MouseInput : UIInputBase
    {
        #region Constructors

        public MouseInput(object sender, MouseEventArgs args)
            : base(sender, args)
        {
            _inputType = UIInputType.Mouse;
            _args = args;
            Position = args.GetPosition(sender as IInputElement);
        }

        public MouseInput(object sender, MouseButtonEventArgs args)
            : this(sender, args as MouseEventArgs)
        {
            _inputType = UIInputType.Mouse;
            _clickCount = args.ClickCount;
            switch (args.ChangedButton)
            {
                case MouseButton.Left:
                    _inputType = UIInputType.MouseLeft; break;
                case MouseButton.Right:
                    _inputType = UIInputType.MouseRight; break;
            }

            if (InputType != UIInputType.None)
                switch (args.ButtonState)
                {
                    case MouseButtonState.Pressed: _inputState = UIInputState.Pressed; break;
                    case MouseButtonState.Released: _inputState = UIInputState.Released; break;
                }

        }

        #endregion

        #region Methods


        public override Point PositionRelativeTo(IInputElement inputElement)
        {
            return (_args as MouseEventArgs).GetPosition(inputElement);
        }

        #endregion

        #region Data


        #endregion

        #region Public Properties


        #endregion

    }

    public class TouchInput : UIInputBase
    {
        #region Constructors

        public TouchInput(object sender, TouchEventArgs args)
            : base(sender, args)
        {
            var touchPoint = args.GetTouchPoint(sender as IInputElement);
            Position = touchPoint.Position;

            _inputType =
                UIInputType.Touch;
            switch (touchPoint.Action)
            {
                case TouchAction.Move:
                    _inputState = UIInputState.Hold;

                    break;
                case TouchAction.Down:
                    _inputState = UIInputState.Pressed;

                    break;
                case TouchAction.Up:
                    _inputState = UIInputState.Released;

                    break;

            }


        }

        #endregion

        #region Methods

        public override Point PositionRelativeTo(IInputElement inputElement)
        {
            return (_args as TouchEventArgs).GetTouchPoint(inputElement).Position;
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion

    }

    public class StylusInput : UIInputBase
    {
        #region Constructors

        public StylusInput(object sender, StylusEventArgs args)
            : base(sender, args)
        {
            Position = args.GetPosition(sender as IInputElement);

            _inputType = UIInputType.Stylus;
            switch (args.RoutedEvent.Name)
            {
                case "StylusDown":
                    _inputState = UIInputState.Pressed;
                    break;
                case "StylusUp":
                    _inputState = UIInputState.Released;
                    break;
                case "StylusMove":
                    _inputState = UIInputState.NotApplied;
                    break;
                default:
                    _inputType = UIInputType.None;
                    break;
            }
        }

        #endregion

        #region Methods

        public override Point PositionRelativeTo(IInputElement inputElement)
        {
            return (_args as StylusEventArgs).GetPosition(inputElement);
        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        #endregion

    }

}
