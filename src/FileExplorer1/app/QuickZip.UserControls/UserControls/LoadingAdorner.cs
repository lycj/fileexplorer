using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using CircularAnimations.Examples;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;

namespace QuickZip.UserControls
{
    public class LoadingAdorner : Adorner
    {

        #region Constructor
        public LoadingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);

            _stackPanel = new StackPanel();
            _stackPanel.Orientation = Orientation.Horizontal;            
            _loadingAnimation = new SilverLightMain();
            _stackPanel.Children.Add(_loadingAnimation);

            _stackPanel.Background = Brushes.White;
            _visualChildren.Add(_stackPanel);


            Binding visibilityBinding = new Binding("IsLoading");
            visibilityBinding.Source = this;
            visibilityBinding.Converter = new BooleanToVisibilityConverter();
            this.SetBinding(LoadingAdorner.VisibilityProperty, visibilityBinding);
        }

        #endregion

        #region Methods

        protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsVisible)
            {
                _loadingAnimation.Measure(availableSize);
                _stackPanel.Measure(availableSize);                
                return new Size(_stackPanel.DesiredSize.Width, _stackPanel.DesiredSize.Height);
            }
            else return new Size(0, 0);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {            
            if (IsVisible)
            {
                _loadingAnimation.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                _stackPanel.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));                
            }
            else _stackPanel.Arrange(new Rect(0,0,0,0));

            return base.ArrangeOverride(finalSize);
        }


        #endregion

        #region Data

        private VisualCollection _visualChildren; 
        private SilverLightMain _loadingAnimation;
        private StackPanel _stackPanel;        
        #endregion

        #region Public Properties

        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingAdorner),
            new PropertyMetadata(true));

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }


        #endregion

         
    }
}
