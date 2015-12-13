///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace QuickZip.Converters
{
    public class DynamicConverter<T, T1> : IValueConverter
    {
        Func<T,T1> _convertFunc = null;
        Func<T1, T> _convertBackFunc = null;

        public DynamicConverter(Func<T,T1> convertFunc, Func<T1,T> convertBackFunc)
        {
            _convertFunc = convertFunc;
            _convertBackFunc = convertBackFunc;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_convertFunc != null)
                if (value is T)
                    return _convertFunc((T)value);
                else return default(T);
            else
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (_convertBackFunc != null)
                if (value is T1)
                    return _convertBackFunc((T1)value);
                else return default(T);
            else
            throw new NotImplementedException();
        }

        #endregion
    }
}
