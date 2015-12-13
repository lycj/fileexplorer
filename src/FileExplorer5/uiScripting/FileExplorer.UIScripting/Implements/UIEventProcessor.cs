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
    public abstract class UIEventProcessorBase : Freezable, IUIEventProcessor
    {
        public int Priority { get; protected set; }
        public virtual IEnumerable<RoutedEvent> ProcessEvents { get { return _processEvents; } }
        protected List<RoutedEvent> _processEvents = new List<RoutedEvent>();

        public UIEventProcessorBase()
        {

        }

        public class CheckTargetName : IfScriptCommand
        {
            public CheckTargetName(string targetName, IScriptCommand trueCommand, IScriptCommand falseCommand)
                : base(pm =>
                    {
                        IUIInput input = pm.GetValue<IUIInput>("{Input}");
                        object sender = input.Sender;
                        RoutedEventArgs eventArgs = input.EventArgs as RoutedEventArgs;

                        if (String.IsNullOrEmpty(targetName) || UITools.FindAncestor<FrameworkElement>(
                                               eventArgs.OriginalSource as DependencyObject,
                                               (ele) => ele.Name == targetName) != null)
                            return true;
                        return false;
                    }
                    , trueCommand, falseCommand)
            {
            }
        }
        public IScriptCommand OnEvent(RoutedEvent eventId)
        {
            return new CheckTargetName(TargetName, onEvent(eventId), ResultCommand.NoError);
        }

        protected virtual IScriptCommand onEvent(RoutedEvent eventId)
        {
            return ResultCommand.NoError;
        }

        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }

        public static DependencyProperty TargetNameProperty =
            DependencyProperty.Register("TargetName", typeof(string), typeof(UIEventProcessorBase));
        public string TargetName
        {
            get { return (string)GetValue(TargetNameProperty); }
            set { SetValue(TargetNameProperty, value); }
        }
    }



}
