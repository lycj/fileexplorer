using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Globalization;
using System.ComponentModel;

namespace Tomers.WPF.Localization
{
	public abstract class LanguageDictionary
	{
		#region Fields

		public static readonly LanguageDictionary Null = new NullLanguageDictionary();
		private static readonly Dictionary<CultureInfo, LanguageDictionary> _registeredDictionaries = new Dictionary<CultureInfo, LanguageDictionary>();

		#endregion

		#region Properties

		public CultureInfo Culture
		{
			get { return CultureInfo.GetCultureInfo(CultureName); }
		}

		public static LanguageDictionary Current
		{
			get { return LanguageDictionary.GetDictionary(LanguageContext.Instance.Culture); }
		}

		#endregion

		#region Public Methods

		public void Load()
		{
			OnLoad();
		}

		public void Unload()
		{
			OnUnload();
		}

		public TValue Translate<TValue>(string uid, string vid)
		{
			return (TValue)Translate(uid, vid, null, typeof(TValue));
		}

		public object Translate(string uid, string vid, object defaultValue, Type type)
		{
			return OnTranslate(uid, vid, defaultValue, type);
		}

		public static void RegisterDictionary(CultureInfo cultureInfo, LanguageDictionary dictionary)
		{
			if (!_registeredDictionaries.ContainsKey(cultureInfo))
			{
				_registeredDictionaries.Add(cultureInfo, dictionary);
			}
		}

		public static void UnregisterDictionary(CultureInfo cultureInfo)
		{
			if (_registeredDictionaries.ContainsKey(cultureInfo))
			{
				_registeredDictionaries.Remove(cultureInfo);
			}
		}

		public static LanguageDictionary GetDictionary(CultureInfo cultureInfo)
		{
			if (cultureInfo == null)
			{
				throw new ArgumentNullException("cultureInfo");
			}
			if (_registeredDictionaries.ContainsKey(cultureInfo))
			{
				LanguageDictionary dictionary = _registeredDictionaries[cultureInfo];
				return dictionary;
			}
			return LanguageDictionary.Null;
		} 

		#endregion

		#region Overrideables

		public abstract string CultureName { get; }
		public abstract string EnglishName { get; }
		protected abstract void OnLoad();
		protected abstract void OnUnload();
		protected abstract object OnTranslate(string uid, string vid, object defaultValue, Type type); 

		#endregion

		#region Null Dictionary
		private sealed class NullLanguageDictionary : LanguageDictionary
		{
			protected override void OnLoad() { }
			protected override void OnUnload() { }
			protected override object OnTranslate(string uid, string vid, object defaultValue, Type type) { return defaultValue; }
			public override string CultureName { get { return CultureInfo.InstalledUICulture.Name; } }
			public override string EnglishName { get { return CultureInfo.InstalledUICulture.EnglishName; } }
		}
		#endregion		
	}
}
