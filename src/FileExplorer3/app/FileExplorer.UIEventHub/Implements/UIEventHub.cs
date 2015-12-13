using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Defines;
using FileExplorer.UIEventHub;
using FileExplorer.UIEventHub.Defines;

namespace FileExplorer
{
    public static partial class ExtensionMethods
    {
        public static IUIEventHub RegisterEventProcessors(this UIElement control, params UIEventProcessorBase[] processors)
        {
            return new FileExplorer.WPF.BaseControls.UIEventHub(new ScriptRunner(), control, true, processors);
        }
    }
}

namespace FileExplorer.WPF.BaseControls
{




    public class UIEventHub : IUIEventHub
    {
        #region Constructor

        public UIEventHub(IScriptRunner runner, UIElement control,
            bool startIsEnabled = true, params UIEventProcessorBase[] eventProcessors)
        {
            Control = control;
            _eventProcessors = new List<UIEventProcessorBase>(eventProcessors);
            _inputProcessors = new UIInputManager(
                new FlickInputProcessor(),
                new ClickCountInputProcessor(),
                new DropInputProcessor(),
                new DragInputProcessor()
                        {
                            DragStartedFunc = inp =>
                              {                                  
                                  if (inp.InputType == UIInputType.Touch)
                                      Control_TouchDrag(inp.Sender, inp.EventArgs as InputEventArgs);
                                  else Control_MouseDrag(inp.Sender, inp.EventArgs as InputEventArgs);
                                  //ScrollViewer.SetPanningRatio(inp.Sender as DependencyObject, 0);
                                  ScrollViewer.SetPanningMode(inp.Sender as DependencyObject, PanningMode.None);
                              },
                            DragStoppedFunc = inp =>
                            {
                                //ScrollViewer.SetPanningRatio(inp.Sender as DependencyObject, 1);
                                ScrollViewer.SetPanningMode(inp.Sender as DependencyObject, PanningMode.Both);
                            }
                        },
                 new TouchDragMoveCountInputProcessor()
                );
            _scriptRunner = runner;
            IsEnabled = startIsEnabled;
        }

        #endregion

        #region Methods

        #region Static Helpers

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }


        #endregion



        private void setIsEnabled(bool value)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    if (value != _isEnabled)
                    {
                        var listenEvents = _eventProcessors.SelectMany(ep => ep.ProcessEvents).Union(
                        _inputProcessors.Processors.SelectMany(ip => ip.ProcessEvents))
                        .Distinct().ToArray();

                        _isEnabled = value; 

                        if (_isEnabled)
                        {
                            foreach (var e in listenEvents)
                            {
                                RoutedEventHandler handler = (RoutedEventHandler)(
                                    async (o, re) =>
                                    {
                                        var input = UIInputBase.FromEventArgs(o, re);
                                        if (input.IsValid())
                                        {
                                            _inputProcessors.Update(ref input);
                                            try
                                            {
                                                await executeAsync(_eventProcessors, e, input,
                                                        pd =>
                                                        {
                                                            if (pd.IsHandled)
                                                                re.Handled = true;
                                                        }
                                                    );
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine(ex);
                                            }
                                        }


                                    });
                                _registeredHandler.Add(e, handler);
                                Control.AddHandler(e, handler);

                                ScrollViewer.SetPanningMode(Control as DependencyObject, PanningMode.Both);
                            }
                        }
                        else
                        {

                            foreach (var evt in _registeredHandler.Keys)
                                Control.RemoveHandler(evt, _registeredHandler[evt]);
                            _registeredHandler.Clear();
                        }
                    }
                }));
        }

        private async Task<bool> executeAsync(IList<UIEventProcessorBase> processors, RoutedEvent eventId,
            IUIInput input, Action<ParameterDic> thenFunc = null)
        {
            //ParameterDic pd = ParameterDicConverters.ConvertUIInputParameter.Convert(null,
            //    eventId.Name, input, _inputProcessors);

            ParameterDic pd = _converter.Convert(null, eventId.Name, input, _inputProcessors);

            IScriptCommand[] commands = 
               processors
               .Where(p => p.ProcessEvents.Contains(input.EventArgs.RoutedEvent))
               .Select(p => p.OnEvent(eventId))
               .Where(c => c.CanExecute(pd)).ToArray();

            await _scriptRunner.RunAsync(pd, ScriptCommands.RunSequence(null, commands));

            if (thenFunc != null)
                thenFunc(pd);
            return pd.IsHandled;
        }



        public static RoutedEvent MouseDragEvent = EventManager.RegisterRoutedEvent(
               "MouseDrag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIEventHub));

        public async void Control_MouseDrag(object sender, InputEventArgs e)
        {
            FrameworkElement control = sender as FrameworkElement;
            var input = UIInputBase.FromEventArgs(sender, e);
            if (await executeAsync(_eventProcessors, UIEventHub.MouseDragEvent, input))
            {
                (_inputProcessors.Processors.First(p => p is DragInputProcessor) as DragInputProcessor).IsDragging = false;
                //await executeAsync(_eventProcessors, UIElement.PreviewMouseUpEvent, input);
            }
        }


        public static RoutedEvent TouchDragEvent = EventManager.RegisterRoutedEvent(
               "TouchDrag", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIEventHub));

        public async void Control_TouchDrag(object sender, InputEventArgs e)
        {            
            FrameworkElement control = sender as FrameworkElement;
            var input = UIInputBase.FromEventArgs(sender, e);
            input.TouchGesture = UITouchGesture.Drag;
            if (await executeAsync(_eventProcessors, UIEventHub.TouchDragEvent, input))
            {
                //(_inputProcessors.Processors.First(p => p is DragInputProcessor) as DragInputProcessor).IsDragging = false;
            }
        }

      

        #endregion

        #region Data

        private IList<UIEventProcessorBase> _eventProcessors;
        private Dictionary<RoutedEvent, RoutedEventHandler> _registeredHandler = new Dictionary<RoutedEvent, RoutedEventHandler>();
        private IScriptRunner _scriptRunner;
        private bool _isEnabled = false;
        private IUIInputManager _inputProcessors;
        private IParameterDicConverter _converter = new ParameterDicConverter(UIParameterDicConvertRule.ConvertUIInputParameters);

        #endregion

        #region Public Properties

        public bool IsEnabled { get { return _isEnabled; } set { setIsEnabled(value); } }
        public UIElement Control { get; private set; }
        public IList<IUIInputProcessor> InputProcessors { get { return _inputProcessors.Processors.ToList(); } }
        public IList<UIEventProcessorBase> EventProcessors
        {
            get { return _eventProcessors; }
            set { _eventProcessors = value; }
        }



        #endregion

    }


}
