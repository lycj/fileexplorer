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
using System.Windows.Shapes;

namespace QuickZip.IO.PIDL.UserControls
{
    public class FileListLookupBoxAdorner : Adorner
    {
        #region Constructor
        public FileListLookupBoxAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            _adornedElement = adornedElement;
            _visualChildren = new VisualCollection(this);
            BuildTextBox();
        }
        #endregion

        #region Methods

        public void UpdateVisibilty(bool isVisible)
        {
            if (_isVisible != isVisible)
                Text = "";
            _isVisible = isVisible;            
            InvalidateMeasure();
            if (isVisible)
                _textBox.Focus();
        }

        protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }
        protected override Size MeasureOverride(Size constraint)
        {
            _textBox.IsEnabled = _isVisible;            
            if (_isVisible)
            {
                _border.Measure(constraint);
                return new Size(_border.DesiredSize.Width, _border.DesiredSize.Height);
            }
            else return new Size(0, 0);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_isVisible)
            {
                _border.Arrange(new Rect(_adornedElement.ActualWidth - finalSize.Width - 15, 
                    _adornedElement.ActualHeight - finalSize.Height + 15,
                    finalSize.Width, finalSize.Height));                
            }
            else // if is not is editable mode, no need to show elements.
            {
                _border.Arrange(new Rect(0, 0, 0, 0));
            }
            return finalSize;
        }


        /// <summary>
        /// Inialize necessary properties and hook necessary events on TextBox, then add it into tree.
        /// </summary>
        private void BuildTextBox()
        {
            _border = new Border();

            _border.BorderBrush = SystemColors.ActiveBorderBrush;// Brushes.Black;
            _border.Background = Brushes.White;
            _border.BorderThickness = new Thickness(2);
            _border.CornerRadius = new CornerRadius(8);
            _border.Padding = new Thickness(2);

            _dock = new DockPanel();

            #region TextBox
            _textBox = new TextBox();
            _textBox.BorderThickness = new Thickness();            
            _textBox.Background = Brushes.White;
            _textBox.Padding = new Thickness();
            _textBox.Margin = new Thickness();            
            _textBox.Width = 100;
            _textBox.KeyDown +=
                (KeyEventHandler)delegate(object sender, KeyEventArgs args)
                {
                    if ( args.Key == Key.Escape)
                    {
                        UpdateVisibilty(false);
                    }
                };
            _textBox.GotKeyboardFocus +=
                (KeyboardFocusChangedEventHandler)delegate(object sender, KeyboardFocusChangedEventArgs e)
                {
                    TextBox tb = (_textBox as TextBox);
                    tb.SelectionStart = tb.Text.Length;
                };


            Binding textBinding = new Binding("Text");
            textBinding.Source = this;
            textBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //textBinding.Mode = BindingMode.OneWayToSource;
            _textBox.SetBinding(TextBox.TextProperty, textBinding);
            #endregion

            _crossButton = new Button();
            _crossButton.Focusable = false;
            _crossButton.Background = Brushes.Transparent;
            _crossButton.BorderBrush = Brushes.Transparent;
            _crossButton.Width = 16;
            _crossPath = new Path();
            _crossPath.Stroke = Brushes.Brown;
            _crossPath.Data = Geometry.Parse("M1,1 L9,9 M1,9 L9,1");
            _crossButton.Content = _crossPath;
            this.AddHandler(Button.ClickEvent, (RoutedEventHandler)delegate
            {
                Text = "";
                UpdateVisibilty(false);
            });


            _dock.Children.Add(_crossButton);
            DockPanel.SetDock(_crossButton, Dock.Right);
            _dock.Children.Add(_textBox);
            _border.Child = _dock;
            _visualChildren.Add(_border);

            _textBox.TextWrapping = TextWrapping.NoWrap;

            ScrollViewer.SetCanContentScroll(_textBox, false);
            ScrollViewer.SetVerticalScrollBarVisibility(_textBox, ScrollBarVisibility.Disabled);
            ScrollViewer.SetHorizontalScrollBarVisibility(_textBox, ScrollBarVisibility.Disabled);
            
        }


        #endregion

        #region Data

        private VisualCollection _visualChildren; //visual children.        
        private FrameworkElement _adornedElement;
        private Border _border;
        private Button _crossButton;
        private TextBox _textBox;
        private Path _crossPath;
        private DockPanel _dock;
        private bool _isVisible;   
        private const double _extraWidth = 5; //exstra space for TextBox to make the text in it clear.
        #endregion

        #region Public Properties
        

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(FileListLookupBoxAdorner),
            new PropertyMetadata((PropertyChangedCallback)delegate { /* Debug.WriteLine("Changed"); */ }));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }


        #endregion



    }
}
