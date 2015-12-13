using FileExplorer.Defines;
using FileExplorer.UIEventHub.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{

    public class FlickInputProcessor : InputProcessorBase
    {



        #region Constructors

        public FlickInputProcessor()
        {
            _processEvents.AddRange(new[] { 
                UIElement.PreviewTouchDownEvent,
                UIElement.PreviewTouchUpEvent
            }
            );
        }

        #endregion

        #region Methods

        public override void Update(ref IUIInput input)
        {
            switch (input.EventArgs.RoutedEvent.Name)
            {
                case "PreviewTouchDown":
                    _touchTime = DateTime.UtcNow;
                    _touchDownPosition = input.Position;
                    break;

                case "PreviewTouchUp":
                    if (DateTime.UtcNow.Subtract(_touchTime).TotalMilliseconds < Defaults.MaximumFlickTime)
                    {
                        if (Math.Abs(input.Position.X - _touchDownPosition.X) > Defaults.MinimumFlickThreshold)
                        {
                            if (input.Position.X > _touchDownPosition.X)
                                input.TouchGesture = UITouchGesture.FlickRight;
                            else input.TouchGesture = UITouchGesture.FlickLeft;
                        }
                        else if (Math.Abs(input.Position.Y - _touchDownPosition.Y) > Defaults.MinimumFlickThreshold)
                        {
                            if (input.Position.Y > _touchDownPosition.Y)
                                input.TouchGesture = UITouchGesture.FlickDown;
                            else input.TouchGesture = UITouchGesture.FlickUp;
                        }
                    }
                    break;
            }

        }

        #endregion

        #region Data

        private Point _touchDownPosition = UIEventHubProperties.InvalidPoint;
        private DateTime _touchTime = DateTime.MinValue;

        #endregion

        #region Public Properties

        #endregion

    }
}
