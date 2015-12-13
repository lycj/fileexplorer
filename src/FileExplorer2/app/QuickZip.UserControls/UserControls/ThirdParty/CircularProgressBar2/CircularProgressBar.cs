using System;
using System.Windows;
using System.Windows.Controls;

namespace ProgressBarTest
{
   public class CircularProgressBar : ProgressBar
   {
      // todo:
      // - get center for the clipping circle
      // - 
      public CircularProgressBar()
      {
         Loaded += CircularProgressBar_Loaded;
         ValueChanged += CircularProgressBar_ValueChanged;
      }

      void CircularProgressBar_Loaded( object sender, RoutedEventArgs e )
      {
         UpdatePoints();
      }

      void CircularProgressBar_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
      {
         UpdatePoints();
      }

      public static readonly DependencyProperty FirstBoundingPointProperty =
         DependencyProperty.Register( "FirstBoundingPoint",
                                     typeof( Point ),
                                     typeof( CircularProgressBar ),
                                     new FrameworkPropertyMetadata( new Point( 0, 0 ) ) );
      public Point FirstBoundingPoint
      {
         get { return (Point)GetValue( FirstBoundingPointProperty ); }
         set { SetValue( FirstBoundingPointProperty, value ); }
      }

      public static readonly DependencyProperty SecondBoundingPointProperty =
         DependencyProperty.Register( "SecondBoundingPoint",
                                     typeof( Point ),
                                     typeof( CircularProgressBar ),
                                     new FrameworkPropertyMetadata( new Point( 0, 0 ) ) );
      public Point SecondBoundingPoint
      {
         get { return (Point)GetValue( SecondBoundingPointProperty ); }
         set { SetValue( SecondBoundingPointProperty, value ); }
      }

      public static readonly DependencyProperty ThirdBoundingPointProperty =
         DependencyProperty.Register( "ThirdBoundingPoint",
                                     typeof( Point ),
                                     typeof( CircularProgressBar ),
                                     new FrameworkPropertyMetadata( new Point( 0, 0 ) ) );
      public Point ThirdBoundingPoint
      {
         get { return (Point)GetValue( ThirdBoundingPointProperty ); }
         set { SetValue( ThirdBoundingPointProperty, value ); }
      }

      public static readonly DependencyProperty ProgressPointProperty =
         DependencyProperty.Register( "ProgressPoint",
                                     typeof( Point ),
                                     typeof( CircularProgressBar ),
                                     new FrameworkPropertyMetadata( new Point( 0, 0 ) ) );
      public Point ProgressPoint
      {
         get { return (Point)GetValue( ProgressPointProperty ); }
         set { SetValue( ProgressPointProperty, value ); }
      }

      private void UpdatePoints()
      {
         // todo: need to look at width of object
         var ratioComplete = Value / Maximum;
         var radius = ActualWidth / 2;

         if ( ratioComplete == 0 )
         {
            ProgressPoint = new Point( 0, radius );

            FirstBoundingPoint = ProgressPoint;
            SecondBoundingPoint = ProgressPoint;
            ThirdBoundingPoint = ProgressPoint;
         }
         else if ( ratioComplete < .25 )
         {
            var angleInRadians = ratioComplete * ( ( Math.PI / 2 ) / .25 );

            // sin(x) = o/h should get the y value
            var yValue = Math.Sin( angleInRadians ) * radius;
            // subtract the radius to get the distance from the top
            yValue = radius - yValue;

            // cos(x) = a/h should get the x value
            var xValue = Math.Cos( angleInRadians ) * radius;
            // subtract the radius to get the distance from the left
            xValue = radius - xValue;

            ProgressPoint = new Point( xValue, yValue );

            FirstBoundingPoint = ProgressPoint;
            SecondBoundingPoint = ProgressPoint;
            ThirdBoundingPoint = ProgressPoint;
         }
         else if ( ratioComplete >= .25 && ratioComplete < .5 )
         {
            //set bounding points
            FirstBoundingPoint = new Point( ActualWidth, 0 );

            var adjustedRatio = ratioComplete - .25;
            var angleInRadians = adjustedRatio * ( ( Math.PI / 2 ) / .25 );
            angleInRadians = ( Math.PI / 2 ) - angleInRadians;

            // sin(x) = o/h should get the y value
            var yValue = Math.Sin( angleInRadians ) * radius;
            // subtract the radius to get the distance from the top
            yValue = radius - yValue;

            // cos(x) = a/h should get the x value
            var xValue = Math.Cos( angleInRadians ) * radius;
            // subtract the radius to get the distance from the left
            xValue = radius + xValue;

            ProgressPoint = new Point( xValue, yValue );

            SecondBoundingPoint = ProgressPoint;
            ThirdBoundingPoint = ProgressPoint;
         }
         else if ( ratioComplete >= .5 && ratioComplete < .75)
         {
            //set bounding points
            FirstBoundingPoint = new Point( ActualWidth, 0 );
            SecondBoundingPoint = new Point( ActualWidth, ActualWidth );

            var adjustedRatio = ratioComplete - .5;
            var angleInRadians = adjustedRatio * ( ( Math.PI / 2 ) / .25 );

            // sin(x) = o/h should get the y value
            var yValue = Math.Sin( angleInRadians ) * radius;
            // subtract the radius to get the distance from the top
            yValue = radius + yValue;

            // cos(x) = a/h should get the x value
            var xValue = Math.Cos( angleInRadians ) * radius;
            // subtract the radius to get the distance from the left
            xValue = radius + xValue;

            ProgressPoint = new Point( xValue, yValue );

            ThirdBoundingPoint = ProgressPoint;
         }
         else if ( ratioComplete >= .75 && ratioComplete < 1 )
         {
            //set bounding points
            FirstBoundingPoint = new Point( ActualWidth, 0 );
            SecondBoundingPoint = new Point( ActualWidth, ActualWidth );
            ThirdBoundingPoint = new Point( 0, ActualWidth );

            var adjustedRatio = ratioComplete - .5;
            var angleInRadians = adjustedRatio * ( ( Math.PI / 2 ) / .25 );

            // sin(x) = o/h should get the y value
            var yValue = Math.Sin( angleInRadians ) * radius;
            // subtract the radius to get the distance from the top
            yValue = radius + yValue;

            // cos(x) = a/h should get the x value
            var xValue = Math.Cos( angleInRadians ) * radius;
            // subtract the radius to get the distance from the left
            xValue = radius + xValue;

            ProgressPoint = new Point( xValue, yValue );
         }
         else
         {
            //set bounding points
            FirstBoundingPoint = new Point( ActualWidth, 0 );
            SecondBoundingPoint = new Point( ActualWidth, ActualWidth );
            ThirdBoundingPoint = new Point( 0, ActualWidth );

            ProgressPoint = new Point( 0, ActualWidth / 2 );
         }


      }
   }
}
