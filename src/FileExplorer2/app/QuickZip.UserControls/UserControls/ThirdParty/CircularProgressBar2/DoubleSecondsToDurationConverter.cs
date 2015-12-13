using System;
using System.Windows;
using System.Windows.Data;

namespace ProgressBarTest
{
   public class DoubleSecondsToDurationConverter : IValueConverter
   {
      public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
      {
         var newDuration = new Duration();


         if ( value != null )
         {
            Double doubleSeconds;

            if ( Double.TryParse( value.ToString(), out doubleSeconds ) )
            {
               newDuration = TimeSpan.FromSeconds( doubleSeconds );
            }
         }

         return newDuration;
      }

      public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
      {
         throw new NotImplementedException();
      }
   }
}