using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;

namespace FileExplorer.WPF.BaseControls
{
    public class OverlappingPanel : Panel
    {


        #region Methods

        protected override Size MeasureOverride(Size availableSize)
        {

            Size desiredSize = new Size(0, 0);
            int itemCount = base.InternalChildren.Count;

            for (int i = 0; i < Math.Min(InternalChildren.Count, MaxItems); i++)
            {
                UIElement element = InternalChildren[i];
                Size eleAvailSize = new Size(Math.Max(0, availableSize.Width - (i * Math.Abs(OverlapX * availableSize.Width))),
                    Math.Max(0,availableSize.Height - (i * Math.Abs(OverlapY * availableSize.Height))));
                element.Measure(eleAvailSize);
                desiredSize = element.DesiredSize;
            }

            //desiredSize store last element's desired size;

            //double overlappedSizeX = itemCount * Math.Abs(OverlapX);
            //double overlappedSizeY = itemCount * Math.Abs(OverlapY);

            double overlappedSizeX = itemCount * Math.Abs(desiredSize.Width * OverlapX);
            double overlappedSizeY = itemCount * Math.Abs(desiredSize.Height * OverlapY); 

            return new Size(desiredSize.Width + overlappedSizeX, desiredSize.Height + overlappedSizeY);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 1)
            {
                int posX = Math.Max(0, (int)((finalSize.Width - InternalChildren[0].DesiredSize.Width) /2));
                int posY = Math.Max(0, (int)((finalSize.Height - InternalChildren[0].DesiredSize.Height) /2));

                InternalChildren[0].Arrange(new Rect(posX, posY, (int)InternalChildren[0].DesiredSize.Width,
                    (int)InternalChildren[0].DesiredSize.Height));
            }
            else
                for (int i = 0; i < Math.Min(InternalChildren.Count, MaxItems); i++)
                {
                    int posX = (int)(i * (OverlapX * finalSize.Width));
                    int posY = (int)(i * (OverlapY * finalSize.Height));
                    int width = (int)InternalChildren[i].DesiredSize.Width;
                    int height = (int)InternalChildren[i].DesiredSize.Height;
                    //width = (int)(finalSize.Width - posX - 15);
                    //height = (int)(finalSize.Height - posY - 15);
                    //Debug.WriteLine(width);
                    InternalChildren[i].Arrange(new Rect(posX, posY, width, height));
                }

            return finalSize;
        }

        #endregion

        #region Public Properties

        public double OverlapX
        {
            get { return (double)GetValue(OverlapXProperty); }
            set { SetValue(OverlapXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverlapX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlapXProperty =
            DependencyProperty.Register("OverlapX", typeof(double), typeof(OverlappingPanel),
            new FrameworkPropertyMetadata(0.1, FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange));



        public double OverlapY
        {
            get { return (double)GetValue(OverlapYProperty); }
            set { SetValue(OverlapYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OverlapY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlapYProperty =
            DependencyProperty.Register("OverlapY", typeof(double), typeof(OverlappingPanel),
            new FrameworkPropertyMetadata(0.1, FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public int MaxItems
        {
            get { return (int)GetValue(MaxItemsProperty); }
            set { SetValue(MaxItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxItemsProperty =
            DependencyProperty.Register("MaxItems", typeof(int), typeof(OverlappingPanel),
            new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsParentArrange));


        #endregion


    }
}
