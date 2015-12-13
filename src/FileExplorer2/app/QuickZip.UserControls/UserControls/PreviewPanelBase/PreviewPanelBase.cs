using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;
using QuickZip.UserControls;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace QuickZip.UserControls
{
    public class PreviewPanelBase : Selector
    {
        #region Constructor
        static PreviewPanelBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PreviewPanelBase),
                new FrameworkPropertyMetadata(typeof(PreviewPanelBase)));
        }

        public PreviewPanelBase()
        {
            playTimer = new DispatcherTimer(TimeSpan.FromHours(1), DispatcherPriority.Background,
                new EventHandler((o, e) =>
                {
                    NavigateNext();
                    if (!CanNavigateNext())
                        AutoPlay = false;

                }), this.Dispatcher);
            playTimer.Stop();

            
            
            RoutedEventHandler itemMouseDown = null;
            itemMouseDown = (o, e) =>
            {
                AutoPlay = false;
            };
            this.AddHandler(PreviewPanelBaseItem.PreviewMouseDownEvent, itemMouseDown);
        }

        #endregion


        #region Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            PreviewPanelBaseItem retVal = new PreviewPanelBaseItem()
                {
                    Background = ChildBackground,
                    Foreground = ChildForeground
                };


            //Selector.AddUnselectedHandler(retVal, (o, e) => { Debug.WriteLine("UnSelect"); });
            //Selector.AddSelectedHandler(retVal, (o, e) => { Debug.WriteLine("Select"); });            

            return retVal;


        }

        private void updateHorizOffset(double offset)
        {            
            
            VirtualStackPanel vsp = UITools.FindVisualChild<VirtualStackPanel>(this);            
                //ppanel.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(() =>
                CommandManager.InvalidateRequerySuggested();//));

                finalOFfset = offset;
                DoubleAnimation moveAnimation = new DoubleAnimation();
                
                moveAnimation.To = offset;
                bool largeChange = Math.Abs(ScrollOffset - offset) > ActualHeight;
                moveAnimation.Duration = new Duration(TimeSpan.FromSeconds(AutoPlay ? 0 : largeChange ? 1 : 0.5));
                this.BeginAnimation(PreviewPanelBase.ScrollOffsetProperty, moveAnimation);

            
        }

        protected virtual void OnMoveAnimationCompleted(object sender, EventArgs e)
        {

        }

        private void updateScrollViewer()
        {
            if (_scroll != null)
            {
                ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(this);
                VirtualStackPanel vsp = UITools.FindVisualChild<VirtualStackPanel>(ip);
                if (vsp != null)
                {
                    double itemFitInViewPort = vsp.ViewportWidth / ActualHeight;
                    double offsetMiddle = ((vsp.ViewportWidth + ActualHeight) / 2);
                    //double newOffset = (ChildWidth * (SelectedIndex - (itemFitInViewPort / 2)));
                    double newOffset = (ActualHeight * (SelectedIndex + 1)) - offsetMiddle;
                    updateHorizOffset(newOffset);
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            updateScrollViewer();
        }

        public bool CanNavigateNext()
        {
            return SelectedIndex < this.Items.Count - 1;
        }

        public void NavigateNext()
        {
            if (CanNavigateNext())
                SelectedIndex++;
        }

        public bool CanNavigateBack()
        {
            return SelectedIndex > 0;
        }

        public void NavigateBack()
        {
            if (CanNavigateBack())
                SelectedIndex--;
        }

        public void NavigateFirst()
        {
            SelectedIndex = 0;
        }

        public void NavigateLast()
        {
            SelectedIndex = this.Items.Count - 1;
        }

        private VirtualStackPanel getPanel()
        {
            if (_scroll != null)
            {
                ItemsPresenter ip = UITools.FindVisualChild<ItemsPresenter>(this);
                VirtualStackPanel vsp = UITools.FindVisualChild<VirtualStackPanel>(ip);
                return vsp;
            }
            return null;
        }

        public bool CanScrollLeft()
        {
            VirtualStackPanel vsp = getPanel();
            if (vsp != null)
                if (vsp.ExtentWidth < vsp.ViewportWidth) return false;
            return finalOFfset > 0;
        }

        public void ScrollLeft()
        {
            VirtualStackPanel vsp = getPanel();
            if (vsp != null)
            {
                updateHorizOffset(ScrollOffset - vsp.ViewportWidth);
            }

        }

        public bool CanScrollRight()
        {
            VirtualStackPanel vsp = getPanel();
            if (vsp != null)
                if (vsp.ExtentWidth < vsp.ViewportWidth) return false;
                else return finalOFfset < vsp.ExtentWidth - vsp.ViewportWidth;
            return false;
        }

        public void ScrollRight()
        {
            VirtualStackPanel vsp = getPanel();
            if (vsp != null)
                updateHorizOffset(ScrollOffset + vsp.ViewportWidth);
        }

        public void ScrollFirst()
        {
            VirtualStackPanel vsp = getPanel();
            if (vsp != null)
                updateHorizOffset(0);
        }

        public void ScrollLast()
        {
            VirtualStackPanel vsp = getPanel();
            if (vsp != null)
                updateHorizOffset(vsp.ExtentWidth);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _scroll = (ScrollViewer)this.Template.FindName("PART_ScrollViewer", this);
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            //Debug.WriteLine("PreviewKeyDown - " + e.Key.ToString());
            e.Handled = true;
            switch (e.Key)
            {
                case System.Windows.Input.Key.Left: NavigateBack(); break;
                case System.Windows.Input.Key.Right: NavigateNext(); break;
                default: e.Handled = false; break;
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            //Debug.WriteLine("SelectionChanged");            
            updateScrollViewer();            
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            AutoPlay = false;
            //SelectedIndex = 1;
            //SetIsSelected(, true);            
        }

        private static void OnScrollOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PreviewPanelBase ppanel = obj as PreviewPanelBase;

            if (ppanel != null && ppanel._scroll != null)
            {
                ScrollContentPresenter scp = UITools.FindVisualChild<ScrollContentPresenter>(ppanel._scroll);
                Panel vsp = UITools.FindVisualChild<Panel>(scp);
                IScrollInfo isi = (IScrollInfo)vsp;
                isi.SetHorizontalOffset(ppanel.ScrollOffset);


            }
        }

        private static void OnAutoPlayChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PreviewPanelBase ppanel = obj as PreviewPanelBase;
            if ((bool)args.NewValue)
            {
                ppanel.playTimer.Interval = ppanel.AutoPlayInterval;
                ppanel.playTimer.Start();
            }
            else
            {
                ppanel.playTimer.Stop();
            }
        }



        #endregion

        #region Data

        ScrollViewer _scroll = null;
        IScrollInfo _isi = null;
        double finalOFfset = 0;
        DispatcherTimer playTimer = null;

        #endregion

        #region Public Properties

        public Brush ChildBackground
        {
            get { return (Brush)GetValue(ChildBackgroundProperty); }
            set { SetValue(ChildBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ChildBackgroundProperty =
            DependencyProperty.Register("ChildBackground", typeof(Brush), typeof(PreviewPanelBase),
            new UIPropertyMetadata(Brushes.Gray));


        public Brush ChildForeground
        {
            get { return (Brush)GetValue(ChildForegroundProperty); }
            set { SetValue(ChildForegroundProperty, value); }
        }

        public static readonly DependencyProperty ChildForegroundProperty =
            DependencyProperty.Register("ChildForeground", typeof(Brush), typeof(PreviewPanelBase),
            new UIPropertyMetadata(Brushes.Silver));

        internal static readonly DependencyProperty ScrollOffsetProperty =
            DependencyProperty.Register("ScrollOffset", typeof(double), typeof(PreviewPanelBase),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnScrollOffsetChanged)));


        public double ScrollOffset
        {
            get { return (double)GetValue(ScrollOffsetProperty); }
            set { SetValue(ScrollOffsetProperty, value); }
        }




        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }
        
        public static readonly DependencyProperty AutoPlayProperty =
            DependencyProperty.Register("AutoPlay", typeof(bool), typeof(PreviewPanelBase), 
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoPlayChanged)));



        public TimeSpan AutoPlayInterval
        {
            get { return (TimeSpan)GetValue(AutoPlayIntervalProperty); }
            set { SetValue(AutoPlayIntervalProperty, value); }
        }
        
        public static readonly DependencyProperty AutoPlayIntervalProperty =
            DependencyProperty.Register("AutoPlayInterval", typeof(TimeSpan), typeof(PreviewPanelBase), 
            new UIPropertyMetadata(TimeSpan.FromSeconds(2)));

        

        


        #endregion
    }
}
