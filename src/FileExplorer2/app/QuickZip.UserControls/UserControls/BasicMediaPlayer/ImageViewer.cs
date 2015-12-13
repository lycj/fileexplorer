using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace QuickZip.UserControls
{
    public class ImageViewer : ContentControl
    {
        //http://stackoverflow.com/questions/741956/wpf-pan-zoom-image
        private Point origin;
        private Point start;
        private bool originMoved = true;

        static ImageViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageViewer),
                new FrameworkPropertyMetadata(typeof(ImageViewer)));


            //ZoomCommand = new RoutedUICommand("Zoom Image","Zoom", typeof(ImageViewer),
            //    GestureToGestureCollection(new KeyGesture(Key.Up)));
            //ZoomBinding = new CommandBinding(NavigationCommands.IncreaseZoom,
            //    new ExecutedRoutedEventHandler(ExecuteZoomCommand));
            //NavigationCommands.IncreaseZoom.InputGestures.Add(new KeyGesture(Key.Up));
            //CommandManager.RegisterClassCommandBinding(typeof(ImageViewer), ZoomBinding);
            //CommandManager.RegisterClassInputBinding(typeof(ImageViewer), 
            //    new InputBinding(NavigationCommands.IncreaseZoom, new KeyGesture(Key.Up)));

        }

        public void IncreaseZoom()
        {
            if (IsZoomEnabled)
                Scale += 0.05;
        }

        public void DecreaseZoom()
        {
            if (IsZoomEnabled)
                Scale -= 0.05;
        }

        //public static void ExecuteZoomCommand(object sender, ExecutedRoutedEventArgs e)
        //{
        //    Debug.WriteLine("Zoom"); 
        //    (sender as ImageViewer).Scale += 0.1;           
        //}

        ////public static RoutedUICommand ZoomCommand;
        //public static CommandBinding ZoomBinding;

        private static InputGestureCollection GestureToGestureCollection(InputGesture gesture)
        {
            InputGestureCollection retVal = new InputGestureCollection();
            retVal.Add(gesture);
            return retVal;
        }

        public ImageViewer()
        {





            //this.CommandBindings.Add(bindingZoom);

            //this.CommandBindings.Add(new CommandBinding(NavigationCommands.DecreaseZoom,
            //    new ExecutedRoutedEventHandler((o, e) => { Scale -= 0.1; Debug.WriteLine(Scale); })));

            //CommandManager.RegisterClassCommandBinding(typeof(ImageViewer), bindingZoom);

            //this.InputBindings.Add(new InputBinding(NavigationCommands.IncreaseZoom, new KeyGesture(Key.Up)));
            //this.InputBindings.Add(new InputBinding(NavigationCommands.DecreaseZoom, new KeyGesture(Key.Down)));            
        }

        #region Methods

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    if (e.ClickCount == 2)
        //    {
        //        this.Scale = Scale > 1 ? 1 : 1.5;
        //        e.Handled = true;
        //        return;
        //    }

        //    base.OnMouseLeftButtonDown(e);
        //}

        public static void registerClassCommandBinding(Type hostType, CommandBinding commandBinding, InputGesture inputGesture)
        {
            CommandManager.RegisterClassCommandBinding(hostType, commandBinding);
            CommandManager.RegisterClassInputBinding(hostType, new InputBinding(commandBinding.Command, inputGesture));
            (commandBinding.Command as RoutedCommand).InputGestures.Add(inputGesture);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            border = (Border)this.Template.FindName("PART_Border", this);
            image = (Image)this.Template.FindName("PART_Image", this);


            image.MouseWheel += image_MouseWheel;
            image.MouseLeftButtonDown += image_MouseLeftButtonDown;
            image.MouseLeftButtonUp += image_MouseLeftButtonUp;
            image.MouseMove += image_MouseMove;
        }

        private static void OnSourceChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ImageViewer viewer = (ImageViewer)sender;
            if (viewer.image != null)
                viewer.image.Source = (ImageSource)args.NewValue;
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            image.ReleaseMouseCapture();
        }

        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            originMoved = true;
            if (!image.IsMouseCaptured) return;

            Vector v = start - e.GetPosition(border);
            OriginX = origin.X - v.X;
            OriginY = origin.Y - v.Y;
          
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            image.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)image.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(border);
            origin = new Point(tt.X, tt.Y);
        }

        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (IsZoomEnabled)
            {
                Point midPoint = new Point(image.ActualWidth / 2, image.ActualHeight / 2);
                Point mousePosition = e.GetPosition(image);

                //Debug.WriteLine(midPoint);
                double newOriginX = ((midPoint.X - mousePosition.X) * Scale);
                double newOriginY = ((midPoint.Y - mousePosition.Y) * Scale);

                if (originMoved)
                {
                    OriginX = (newOriginX + OriginX) / 2;
                    OriginY = (newOriginY + OriginY) / 2;
                    originMoved = false;
                }

                double zoom = e.Delta > 0 ? .1 : -.1;
                //zoom = e.Delta > 0 ? 2 : -2;
                zoom = (float)e.Delta / 1000;
                Scale += zoom;

                e.Handled = true;
            }
           
        }

        #endregion


        #region Data

        Border border = null;
        Image image = null;

        #endregion

        #region Dependency Properties



        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = Image.SourceProperty.AddOwner(typeof(ImageViewer),
            new PropertyMetadata(OnSourceChanged));



        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ImageViewer), new UIPropertyMetadata(1.0d));



        public bool IsZoomEnabled
        {
            get { return (bool)GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsZoomEnabledProperty =
            DependencyProperty.Register("IsZoomEnabled", typeof(bool), typeof(ImageViewer), 
            new UIPropertyMetadata(true));


        public double OriginX
        {
            get { return (double)GetValue(OriginXProperty); }
            set { SetValue(OriginXProperty, value); }
        }
        public static readonly DependencyProperty OriginXProperty =
            DependencyProperty.Register("OriginX", typeof(double), typeof(ImageViewer));

        public double OriginY
        {
            get { return (double)GetValue(OriginYProperty); }
            set { SetValue(OriginYProperty, value); }
        }
        public static readonly DependencyProperty OriginYProperty =
            DependencyProperty.Register("OriginY", typeof(double), typeof(ImageViewer));






        #endregion



    }
}
