///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of ATC Avalon Team's work                                                                 //
// (http://blogs.msdn.com/atc_avalon_team/archive/2006/03/14/550934.aspx)                                        //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Input;
using System.Diagnostics;

namespace FileExplorer.WPF.UserControls
{
    public class EditBoxAdorner : Adorner
    {
        #region Constructor
        
        public EditBoxAdorner(EditBox editBox, UIElement adornedElement)
            : base(adornedElement)
        {
            _editBox = editBox;
            _visualChildren = new VisualCollection(this);
            BuildTextBox();

            #region Block Enter command            

            enterCommandBinding = new CommandBinding(ApplicationCommands.Open,
               (ExecutedRoutedEventHandler)delegate(object sender, ExecutedRoutedEventArgs e)
               {
                   e.Handled = true;
               },
               (CanExecuteRoutedEventHandler)delegate(object sender, CanExecuteRoutedEventArgs e)
               {
                   e.CanExecute = true;
               });
            CommandManager.RegisterClassCommandBinding(typeof(EditBoxAdorner), enterCommandBinding);                        
            //CommandManager.RegisterClassInputBinding(typeof(EditBoxAdorner), new InputBinding(ApplicationCommands.Open, new KeyGesture(Key.Enter)));
            #endregion
        }
        #endregion

        #region Methods


        /// <summary>
        /// Public method for EditBox to update staus of TextBox when IsEditing property is changed.
        /// </summary>
        public void UpdateVisibilty(bool isVisible)
        {
            _isVisible = isVisible;
            InvalidateMeasure();
        }

        protected override int VisualChildrenCount { get { return 1; } }
        protected override Visual GetVisualChild(int index) { return _canvas; }
        protected override Size MeasureOverride(Size constraint)
        {
            _textBox.IsEnabled = _isVisible;
            //if in editable mode, measure the space the adorner element should cover.
            if (_isVisible)
            {
                
                AdornedElement.Measure(constraint);
                _textBox.Measure(constraint);
                //since the adorner is to cover the EditBox, it should return the AdornedElement.Width, 
                //the extra 15 is to make it more clear.                          
                return new Size(_textBox.DesiredSize.Width + _extraWidth, _textBox.DesiredSize.Height);
                //return new Size(constraint.Width, _textBox.DesiredSize.Height);                
            }
            else  //if it is not in editable mode, no need to show anything.
                return new Size(0, 0);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_isVisible)
            {
                _textBox.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            else // if is not is editable mode, no need to show elements.
            {
                _textBox.Arrange(new Rect(0, 0, 0, 0));
            }
            return finalSize;
        }


        /// <summary>
        /// Inialize necessary properties and hook necessary events on TextBox, then add it into tree.
        /// </summary>
        private void BuildTextBox()
        {

            if (_textBox == null)
            {
                _textBox = new TextBox();
                _textBox.BorderThickness = new Thickness(1);
                _textBox.BorderBrush = Brushes.Black;
                _textBox.Background = Brushes.White;
                _textBox.Padding = new Thickness(0);
                _textBox.Margin = new Thickness(0);
                _textBox.IsHitTestVisible = true;
                _textBox.KeyDown +=
                    (KeyEventHandler)delegate(object sender, KeyEventArgs args)
                    {
                        if (_editBox.IsEditing && (args.Key == Key.Enter || args.Key == Key.Escape))
                        {
                            if (args.Key == Key.Escape)
                            {
                                BindingExpression bexp = _editBox.GetBindingExpression(EditBox.ActualValueProperty);
                                bexp.UpdateTarget();
                            }
                            _editBox.IsEditing = false;
                            _editBox._canBeEdit = false;
                            args.Handled = true;
                        }
                    };
                _textBox.GotKeyboardFocus +=
                    (KeyboardFocusChangedEventHandler)delegate(object sender, KeyboardFocusChangedEventArgs e)
                    {
                        TextBox tb = (_textBox as TextBox); 
                        tb.SelectionStart = tb.Text.Length;
                    };
                _textBox.LostKeyboardFocus +=
                    (KeyboardFocusChangedEventHandler)delegate(object sender, KeyboardFocusChangedEventArgs e)
                    {
                        BindingExpression bexp = _textBox.GetBindingExpression(TextBox.TextProperty);                
                        bexp.UpdateSource();
                        _editBox.IsEditing = false;
                    };
            }

            _canvas = new Canvas();
            _canvas.Children.Add(_textBox);
            _visualChildren.Add(_canvas);

            //Bind Text onto AdornedElement.
            Binding binding = new Binding("ActualValue");
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
            binding.Source = _editBox;


            _textBox.SetBinding(TextBox.TextProperty, binding);


            //Binding binding = new Binding("Text");

            //Update TextBox's focus status when layout finishs.            
            _textBox.LayoutUpdated += new EventHandler(
                (EventHandler)delegate(object sender, EventArgs args)
                {
                    if (_isVisible)
                        _textBox.Focus();
                });
            _textBox.Background = Brushes.Transparent;
            _textBox.TextWrapping = TextWrapping.NoWrap;

            ScrollViewer.SetCanContentScroll(_textBox, false);
            ScrollViewer.SetVerticalScrollBarVisibility(_textBox, ScrollBarVisibility.Disabled);
            ScrollViewer.SetHorizontalScrollBarVisibility(_textBox, ScrollBarVisibility.Disabled);

            _textBox.GotFocus += delegate
            {
                _textBox.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                    new ThreadStart(delegate
                    {
                        int pos = _textBox.Text.LastIndexOf(".");
                        if (pos == -1)
                            _textBox.SelectAll();
                        else _textBox.Select(0, pos);
                    }));
            };
        }


        #endregion

        #region Data

        private CommandBinding enterCommandBinding;
        private VisualCollection _visualChildren; //visual children.
        private EditBox _editBox;
        private TextBox _textBox;  //The TextBox this adorner needs to cover.
        private bool _isVisible;   //whether is in editabl mode.
        private Canvas _canvas;    //canvas to contain the TextBox to enable it can expand out of cell.
        private const double _extraWidth = 5; //exstra space for TextBox to make the text in it clear.
        #endregion




    }
}
