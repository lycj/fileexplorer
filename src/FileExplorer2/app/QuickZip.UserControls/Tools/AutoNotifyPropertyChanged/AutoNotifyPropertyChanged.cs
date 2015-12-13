using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Globalization;

namespace QuickZip.Tools
{
    public class AutoNotifyPropertyChanged : INotifyPropertyChanged
    {
        private Dictionary<string, object> variableDic = new Dictionary<string, object>();

        protected Observable<T> getVar<T>(string name) //GetVariable
        {
            if (!variableDic.ContainsKey(name))
                variableDic.Add(name, new Observable<T>(new Action(() => { NotifyPropertyChanged(name); })));
            return (Observable<T>)variableDic[name];
        }

        protected void setVar<T>(string name, Observable<T> value)
        {
            if (variableDic.ContainsKey(name))
                variableDic.Remove(name);
            variableDic.Add(name, new Observable<T>(new Action(() => { NotifyPropertyChanged(name); })));
            ((Observable<T>)variableDic[name]).Value = value.Value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }

    public class ObservableConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object valueValue = value.GetType().GetProperty("Value").GetValue(value, null);
            return base.ConvertTo(context, culture, valueValue, destinationType);
        }
    }

    public static class Tools
    {
        public static T TryConvertTo<T>(this object o)
        {
            if (o is Observable<T>)
                return (o as Observable<T>).Value;
            return (T)o;
        }      

        //http://www.yoda.arachsys.com/csharp/genericoperators.html
        public static T Add<T>(T a, T b)
        {
            // declare the parameters
            ParameterExpression paramA = System.Linq.Expressions.Expression.Parameter(typeof(T), "a"),
                paramB = System.Linq.Expressions.Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = System.Linq.Expressions.Expression.Add(paramA, paramB);
            // compile it
            Func<T, T, T> add = System.Linq.Expressions.Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return add(a, b);
        }

        public static T Subtract<T>(T a, T b)
        {
            // declare the parameters
            ParameterExpression paramA = System.Linq.Expressions.Expression.Parameter(typeof(T), "a"),
                paramB = System.Linq.Expressions.Expression.Parameter(typeof(T), "b");
            // add the parameters together
            BinaryExpression body = System.Linq.Expressions.Expression.Subtract(paramA, paramB);
            // compile it
            Func<T, T, T> subtract = System.Linq.Expressions.Expression.Lambda<Func<T, T, T>>(body, paramA, paramB).Compile();
            // call it
            return subtract(a, b);
        }
    }

    [TypeConverter(typeof(ObservableConverter))]
    public class Observable<T> : INotifyPropertyChanged
    {
        T _value;
        Action _updateAction;

        public Observable(Action updateAction)
        {
            _updateAction = updateAction;
        }

        public T Value
        {
            get { return _value; }
            set
            {
                if (value == null || !value.Equals(_value))
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                    if (_updateAction != null)
                        _updateAction.Invoke();
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public static implicit operator Observable<T>(T value)
        {
            return new Observable<T>(null) { Value = value };
        }

        public static implicit operator T(Observable<T> value)
        {
            return value.Value;
        }        

        public static Observable<T> operator +(Observable<T> value1, Observable<T> value2)
        {
            return new Observable<T>(null) { Value = Tools.Add(value1.Value, value2.Value) };
        }

        public static Observable<T> operator -(Observable<T> value1, Observable<T> value2)
        {
            return new Observable<T>(null) { Value = Tools.Subtract(value1.Value, value2.Value) };
        }
    }
}
