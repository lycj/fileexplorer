using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Collections.Generic;


namespace FileExplorer.WPF.UserControls
{
    //This control is taken from the following web site 
    //http://blogs.msdn.com/b/mcsuksoldev/archive/2011/01/12/an-animated-custom-panel-base-class-for-wpf-and-silverlight.aspx    
    public class AnimatedVirtualizingPanel : VirtualizingPanel
    {
        Dictionary<UIElement, Rect> targetPositions = new Dictionary<UIElement, Rect>();
        Dictionary<UIElement, Rect> currentPositions = new Dictionary<UIElement, Rect>();
        Dictionary<UIElement, Rect> startingPositions = new Dictionary<UIElement, Rect>();
        DateTime lastUpdateTime = DateTime.Now;
        DateTime endTime = DateTime.Now;

        public bool IsAnimating { get; set; }

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(AnimatedVirtualizingPanel), new UIPropertyMetadata(TimeSpan.FromMilliseconds(0)));

        protected override Size ArrangeOverride(Size finalSize)
        {
            DateTime now = DateTime.Now;

            foreach (UIElement child in this.Children)
            {
                if (!this.targetPositions.ContainsKey(child))
                    throw new InvalidOperationException("Must call AnimatedPanel.AnimatedArrange for all children");

                if (!this.currentPositions.ContainsKey(child))
                    this.currentPositions[child] = this.targetPositions[child];

                if (!this.startingPositions.ContainsKey(child))
                    this.startingPositions[child] = this.targetPositions[child];
            }

            bool somethingMoved = false;
            foreach (UIElement child in this.Children)
            {
                if (this.startingPositions.ContainsKey(child))
                {
                    Rect s = this.startingPositions[child];
                    Rect t = this.targetPositions[child];
                    if (s.Left != t.Left || s.Top != t.Top || s.Width != t.Width || s.Height != t.Height)
                    {
                        somethingMoved = true;
                        break;
                    }
                }
            }

            if (somethingMoved)
            {
                // Start animating (make endTime later than now)
                this.IsAnimating = true;
                this.lastUpdateTime = now;
                this.endTime = this.lastUpdateTime.AddMilliseconds(this.Duration.TotalMilliseconds);
                foreach (UIElement child in this.Children)
                {
                    this.startingPositions[child] = this.targetPositions[child];
                }
            }

            double timeRemaining = (this.endTime - now).TotalMilliseconds;

            double deltaTimeSinceLastUpdate = (now - lastUpdateTime).TotalMilliseconds;
            if (deltaTimeSinceLastUpdate > timeRemaining)
                deltaTimeSinceLastUpdate = timeRemaining;

            DateTime startTime = this.endTime.AddMilliseconds(-this.Duration.TotalMilliseconds);
            double timeIntoAnimation = (now - startTime).TotalMilliseconds;

            double fractionComplete;
            if (timeRemaining > 0)
                fractionComplete = GetCurrentValue(timeIntoAnimation, 0, 1, this.Duration.TotalMilliseconds);
            else
                fractionComplete = 1;

            //            Debug.WriteLine("Arrange " + fractionComplete.ToString());

            foreach (UIElement child in this.Children)
            {
                Rect t = this.targetPositions[child];
                Rect c = this.currentPositions[child];
                double left = ((t.Left - c.Left) * fractionComplete) + c.Left;
                Rect pos = new Rect(left, ((t.Top - c.Top) * fractionComplete) + c.Top,
                    ((t.Width - c.Width) * fractionComplete) + c.Width, ((t.Height - c.Height) * fractionComplete) + c.Height);

                child.Arrange(pos);
                this.currentPositions[child] = pos;
            }

            this.lastUpdateTime = now;

            CompositionTarget.Rendering -= CompositionTarget_Rendering;

            if (timeRemaining > 0)
            {
                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            else
            {
                this.IsAnimating = false;
            }

            Clean(this.startingPositions);
            Clean(this.currentPositions);
            Clean(this.targetPositions);

            return base.ArrangeOverride(finalSize);
        }

        // Dictionary may reference children that have been removed
        private void Clean(Dictionary<UIElement, Rect> dictionary)
        {
            if (dictionary.Count != this.Children.Count)
            {
                Dictionary<UIElement, Rect> newDictionary = new Dictionary<UIElement, Rect>();
                foreach (UIElement child in this.Children)
                {
                    newDictionary[child] = dictionary[child];
                }

                dictionary = newDictionary;
            }
        }


        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            base.OnItemsChanged(sender, args);
            startingPositions.Clear();
            currentPositions.Clear();
            targetPositions.Clear();
        }

        public void AnimatedArrange(UIElement child, Rect finalSize)
        {
            this.targetPositions[child] = finalSize;
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            this.InvalidateArrange();
        }

        /**
        * Default easing equation: EaseInOutQuad 
        * Easing equation function for a quadratic (t^2) easing in/out: acceleration until halfway, then deceleration.
        *
        * time                 Current time (in milliseconds).
        * begin                Starting value.
        * change           Change needed in value.
        * duration         Expected easing duration (in milliseconds).
        * @return          The correct value.
        */
        public virtual double GetCurrentValue(double time, double begin, double change, double duration)
        {
            if ((time /= duration / 2) < 1)
                return change / 2 * time * time + begin;
            return -change / 2 * ((--time) * (time - 2) - 1) + begin;
        }
    }
}
