using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using QuickZip.IO.PIDL.UserControls.ViewModel;
using QuickZip.IO.PIDL.UserControls.Model;
using System.IO;

namespace QuickZip.IO.PIDL.UserControls
{
    /// <summary>
    /// This class simply converts a Boolean to a Visibility
    /// with an optional invert
    /// </summary>
    [ValueConversion(typeof(ExViewModel),  typeof(FileSystemInfoEx))]
    public class ModelToExConverter : IValueConverter
    {
        #region IValueConverter implementation
        /// <summary>
        /// Converts Boolean to Visibility
        /// </summary>
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is ExViewModel)
                return (value as ExViewModel).EmbeddedModel.EmbeddedEntry;
            else if (value is ExModel)
                return (value as ExModel).EmbeddedEntry;
            return null;
        }

        /// <summary>
        /// Convert back, but its not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is FileSystemInfoEx)
                return (value as FileSystemInfoEx).FullName;
            else return "";
        }
        #endregion
    }
}
