using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FileExplorer.Script;
using FileExplorer;
using System.Windows.Input;
using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub.Defines;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{    
    

    public class TouchGestureEventProcessor : SimpleUIEventProcessor
    {
        #region Constructors

        public TouchGestureEventProcessor()
        {
            registerEvent(UIElement.PreviewTouchUpEvent,
                new SimpleScriptCommand("TouchGesture",
                    pd =>
                    {
                        if (Gesture == pd.GetValue<IUIInput>("{Input}").TouchGesture)
                            if (Command.CanExecute(CommandParameter))
                            {
                                Command.Execute(CommandParameter);
                                return ResultCommand.OK;
                            }

                        return ResultCommand.NoError;
                    }));
        }

        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public static DependencyProperty GestureProperty =
          DependencyProperty.Register("Gesture", typeof(UITouchGesture), typeof(TouchGestureEventProcessor),
          new PropertyMetadata(null));

        public UITouchGesture Gesture
        {
            get { return (UITouchGesture)GetValue(GestureProperty); }
            set { SetValue(GestureProperty, value); }
        }


        public static DependencyProperty CommandProperty =
           DependencyProperty.Register("Command", typeof(ICommand), typeof(TouchGestureEventProcessor),
           new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static DependencyProperty CommandParameterProperty =
           DependencyProperty.Register("CommandParameter", typeof(object), typeof(TouchGestureEventProcessor),
           new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        #endregion
    }

}
