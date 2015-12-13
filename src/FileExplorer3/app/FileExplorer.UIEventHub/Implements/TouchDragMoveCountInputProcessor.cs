using FileExplorer.UIEventHub;
using FileExplorer.UIEventHub.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer.WPF.BaseControls
{

    public class TouchDragMoveCountInputProcessor : InputProcessorBase
    {
        #region Constructors

        public TouchDragMoveCountInputProcessor()
        {

            ProcessAllEvents = false;
            _processEvents.AddRange(new[] {                 
                UIEventHub.TouchDragEvent,
                FrameworkElement.TouchMoveEvent,  
                FrameworkElement.TouchUpEvent
            });
        }

        #endregion

        #region Methods

        public override void Update(ref IUIInput input)
        {
            if (input.EventArgs.RoutedEvent == UIEventHub.TouchDragEvent)
            {
                _dragMoveCount = 0;
                _isDragging = true;
            }
            else if (input.EventArgs.RoutedEvent == FrameworkElement.TouchUpEvent)
            {
                _isDragging = false;
            }
            else if (_isDragging) _dragMoveCount++;
        }

        #endregion

        #region Data

        private int _dragMoveCount = 0;
        private bool _isDragging = false;

        #endregion

        #region Public Properties

        public int DragMoveCount { get { return _dragMoveCount; } }

        #endregion
    }
}
