using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;

namespace Tomers.WPF.Localization
{
	public class LanguageConverter : IValueConverter, IMultiValueConverter
	{
		#region Fields

		private string _uid;
		private string _vid;
		private object _defaultValue;
		private bool _isStaticUid; 
		
		#endregion

		#region Initialization

		public LanguageConverter(string uid, string vid, object defaultValue)
		{
			this._uid			= uid;
			this._vid			= vid;
			this._defaultValue	= defaultValue;
			this._isStaticUid	= true;
		}
 
		#endregion

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			LanguageDictionary dictionary = ResolveDictionary();
			object translation = dictionary.Translate(_uid, _vid, _defaultValue, targetType);
			return translation;
		}		

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Binding.DoNothing;
		}

		#endregion

		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				int parametersCount = _isStaticUid ? values.Length - 1 : values.Length - 2;
				if (string.IsNullOrEmpty(_uid))
				{
					if (values[1] == null)
					{
						throw new ArgumentNullException("Uid must be provided as the first Binding element, and must not be null");
					}
					_isStaticUid = false;
					_uid = values[1].ToString();
					--parametersCount;
				}
				LanguageDictionary dictionary = ResolveDictionary();
				object translatedObject = dictionary.Translate(_uid, _vid, _defaultValue, targetType);
				if (translatedObject != null && parametersCount != 0)
				{
					object[] parameters = new object[parametersCount];
					Array.Copy(values, values.Length - parametersCount, parameters, 0, parameters.Length);
					try
					{
						translatedObject = string.Format(translatedObject.ToString(), parameters);
					}
					catch (Exception)
					{
						#region Trace
						Debug.WriteLine(string.Format("LanguageConverter failed to format text {0}", translatedObject.ToString()));
						#endregion
					}
				}
				return translatedObject;
			}
			catch (Exception ex)
			{
				#region Trace
				Debug.WriteLine(string.Format("LanguageConverter failed to convert text: {0}", ex.Message));
				#endregion
			}
			return null;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[0];
		}		

		#endregion

		#region Privates

		private bool ShouldTranslateText
		{
			get { return string.IsNullOrEmpty(_vid); }
		}

		private static LanguageDictionary ResolveDictionary()
		{
			LanguageDictionary dictionary = LanguageDictionary.GetDictionary(
				LanguageContext.Instance.Culture);
			if (dictionary == null)
			{
				throw new InvalidOperationException(string.Format("Dictionary for language {0} was not found",
					LanguageContext.Instance.Culture));
			}
			return dictionary;
		}

		#endregion
	}
}
