using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.UIEventHub.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test_InputProcessor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

                                
        }

        private void addOutput(string text)
        {
            if (_lastText != text)
            {
                _lastText = text;
                inputProcessorOutput.Items.Add(text);
                while (inputProcessorOutput.Items.Count > 10)
                    inputProcessorOutput.Items.RemoveAt(0);
            }
        }

        private string _lastText;
        private static IUIInputManager _inputProcessors;

        public void UIEventHub_Handler(object o, RoutedEventArgs e)
        {
            //Create an UIInput from event.
            IUIInput input = UIInputBase.FromEventArgs(o, e as InputEventArgs);
            if (cbEvent.IsChecked.Value && input.InputState != UIInputState.NotApplied)
                addOutput(input.ToString());

            //Use inputprocessors to process the input 
            _inputProcessors.Update(ref input);

            if (cbClickCount.IsChecked.Value && input.ClickCount > 1)
                addOutput("ClickCount " + input.ClickCount.ToString());

            if (cbTouchGesture.IsChecked.Value && input.TouchGesture != UITouchGesture.NotApplied && input.TouchGesture != UITouchGesture.Drag)
                addOutput(input.TouchGesture.ToString());

            if (cbRawEvent.IsChecked.Value)
                if (e is MouseButtonEventArgs)
                {
                    MouseButtonEventArgs me = e as MouseButtonEventArgs;
                    addOutput(String.Format("{0} : {1} {2}", me.RoutedEvent, me.ChangedButton,  me.GetPosition(null)));
                }
                else if (e is MouseEventArgs)
                {
                    MouseEventArgs me = e as MouseEventArgs;
                    addOutput(String.Format("{0} : {1}", me.RoutedEvent, me.GetPosition(null)));
                }
                else if (e is TouchEventArgs)
                {
                    TouchEventArgs te = e as TouchEventArgs;
                    addOutput(String.Format("{0} : {1}", te.RoutedEvent, te.GetTouchPoint(null)));
                }
                else addOutput(e.ToString());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var dragInputProcessor = new DragInputProcessor()
               {
                   DragStartedFunc = inp => { if (cbDrag.IsChecked.Value) addOutput("*DragStarted* " + inp.ToString()); },
                   DragStoppedFunc = inp => { if (cbDrag.IsChecked.Value) addOutput("*DragStopped* " + inp.ToString()); }
               };
            var clickCountInputProcessor = new ClickCountInputProcessor() { };
            var flickInputProcessor = new FlickInputProcessor() { };

            //Create a list of input processors.
            _inputProcessors = new UIInputManager(dragInputProcessor, flickInputProcessor, clickCountInputProcessor);

            //Register all events required by different processors.
            foreach (var routedEvent in _inputProcessors.Processors.SelectMany(p => p.ProcessEvents).Distinct())
                inputProcessorCanvas.AddHandler(routedEvent, (RoutedEventHandler)UIEventHub_Handler);

            //Clear list box if double click
            inputProcessorOutput.MouseDoubleClick += (o, e) =>
              inputProcessorOutput.Items.Clear();
        }
    }
}
