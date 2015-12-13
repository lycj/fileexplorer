using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.UIEventHub.Defines;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{



    public class InputBindingsEventProcessor : UIEventProcessorBase
    {

        public class QueryInputBindings : UIScriptCommandBase
        {
            InputBindingsEventProcessor _processor;
            private string _targetName;
            public QueryInputBindings(InputBindingsEventProcessor processor, string targetName)
                : base("QueryInputBindings")
            {
                _processor = processor;
                _targetName = targetName;
            }

            public override IScriptCommand Execute(ParameterDic pm)
            {
                IUIInput input = pm.GetValue<IUIInput>(InputKey);
                object sender = input.Sender;
                InputEventArgs eventArgs = input.EventArgs as InputEventArgs;

                if (!eventArgs.Handled)
                {
                    foreach (InputBinding ib in _processor.InputBindings)
                    {
                        bool match = ib.Gesture.Matches(sender, eventArgs);
                        if (!match && ib is MouseBinding &&
                            input.InputType == UIInputType.Touch &&
                            (ib as MouseBinding).MouseAction == MouseAction.LeftDoubleClick)
                            match = input.ClickCount == 2;

                        if (match && ib.Command != null)
                        {

                            if (!String.IsNullOrEmpty(_targetName))
                                if (UITools.FindAncestor<FrameworkElement>(
                                   eventArgs.OriginalSource as DependencyObject,
                                   (ele) => ele.Name == _targetName) == null)
                                {
                                    //If TargetName is set, and targetName is not found.
                                    //Then the event is outside scope, setting it handled
                                    //to prevent it being handled by parent's InputBindingEventProcessor.
                                    eventArgs.Handled = true;
                                    return ResultCommand.NoError;
                                }

                            if (ib.Command.CanExecute(ib.CommandParameter))
                            {
                                ib.Command.Execute(ib.CommandParameter);
                                return ResultCommand.OK;
                            }

                        }
                    }
                }
                return ResultCommand.NoError;
            }
        }

        public InputBindingsEventProcessor()
        {
            InputBindings = new InputBindingCollection();
            _processEvents.AddRange(
                new[] {
                    FrameworkElement.KeyDownEvent, 
                    FrameworkElement.PreviewMouseDownEvent,
                    FrameworkElement.PreviewTouchDownEvent,
                    FrameworkElement.PreviewMouseWheelEvent
                });
        }

        protected override IScriptCommand onEvent(RoutedEvent eventId)
        {
            return new QueryInputBindings(this, TargetName);
        }

        ///// <summary>
        ///// If set, only event came from (originalSouce) element 
        ///// under element with the specified name will trigger the bindings. 
        ///// </summary>
        //public static DependencyProperty TargetNameProperty =
        //     DependencyProperty.Register("TargetName", typeof(string), typeof(InputBindingsEventProcessor));
        //public string TargetName
        //{
        //    get { return (string)GetValue(TargetNameProperty); }
        //    set { SetValue(TargetNameProperty, value); }
        //}

        public static DependencyProperty InputBindingsProperty =
            DependencyProperty.Register("InputBindings", typeof(InputBindingCollection), typeof(InputBindingsEventProcessor),
            new PropertyMetadata());

        public InputBindingCollection InputBindings
        {
            get { return (InputBindingCollection)GetValue(InputBindingsProperty); }
            set { SetValue(InputBindingsProperty, value); }
        }

    }
}
