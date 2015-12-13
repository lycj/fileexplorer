using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using System.Threading;
using System.ComponentModel;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Tomers.WPF.Localization
{
	[ContentProperty("Parameters")]
	public class Translate : MarkupExtension
	{
		#region Fields

		private DependencyProperty	_property;
		private DependencyObject	_target;
		private object				_default;
		private string				_uid;
		
		private readonly Collection<BindingBase> _parameters = new Collection<BindingBase>();

		#endregion

		#region Initialization

		public Translate() { }

		public Translate(object defaultValue)
		{
			this._default = defaultValue;
		}
		
		#endregion

		#region Properties

		public object Default
		{
			get { return _default; }
			set { _default = value; }
		}

		public string Uid
		{
			get { return _uid; }
			set { _uid = value; }
		}

		public Collection<BindingBase> Parameters
		{
			get { return _parameters; }
		}

		#region UidProperty DProperty

		public static string GetUid(DependencyObject obj)
		{
			return (string)obj.GetValue(UidProperty);
		}

		public static void SetUid(DependencyObject obj, string value)
		{
			obj.SetValue(UidProperty, value);
		}

		public static readonly DependencyProperty UidProperty =
			DependencyProperty.RegisterAttached("Uid", typeof(string), typeof(Translate), new UIPropertyMetadata(string.Empty));		 

		#endregion

		#endregion

		#region Overrides

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
			if (service == null)
			{
				return this;
			}

			DependencyProperty property = service.TargetProperty as DependencyProperty;
			DependencyObject target = service.TargetObject as DependencyObject;
			if (property == null || target == null)
			{
				return this;
			}

			this._target = target;
			this._property = property;

			return BindDictionary(serviceProvider);
		}		

		#endregion

		#region Privates

		private object BindDictionary(IServiceProvider serviceProvider)
		{
			string uid = _uid ?? GetUid(_target);
			string vid = _property.Name;

			Binding binding = new Binding("Dictionary");
			binding.Source = LanguageContext.Instance;
			binding.Mode = BindingMode.TwoWay;
			LanguageConverter converter = new LanguageConverter(uid, vid, _default);
			if (_parameters.Count == 0)
			{
				binding.Converter = converter;
				object value = binding.ProvideValue(serviceProvider);
				return value;
			}
			else
			{
				MultiBinding multiBinding = new MultiBinding();
				multiBinding.Mode = BindingMode.TwoWay;
				multiBinding.Converter = converter;
				multiBinding.Bindings.Add(binding);
				if (string.IsNullOrEmpty(uid))
				{
					Binding uidBinding = _parameters[0] as Binding;
					if (uidBinding == null)
					{
						throw new ArgumentException("Uid Binding parameter must be the first, and of type Binding");
					}
				}
				foreach (Binding parameter in _parameters)
				{
					multiBinding.Bindings.Add(parameter);
				}
				object value = multiBinding.ProvideValue(serviceProvider);
				return value;
			}
		}
		#endregion
	}
}
