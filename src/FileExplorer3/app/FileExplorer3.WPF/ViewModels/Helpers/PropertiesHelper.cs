using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    public class PropertiesHelper<T> : DynamicObject, INotifyPropertyChanged
    {
        #region Constructors

        #endregion

        #region Methods


        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            T val; result = null;
            if (ValueDictionary.TryGetValue(binder.Name, out val))
            {
                result = val;
                return true;
            }
            return false;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            ValueDictionary[binder.Name] = (T)value;
            NotifyOfPropertyChanged(binder.Name);
            return true;
        }

        #region INotifyPropertyChanged
        protected virtual void NotifyOfPropertyChanged<T>(Expression<Func<T>> expression)
        {
            NotifyOfPropertyChanged(GetPropertyName<T>(expression));
        }
        protected virtual void NotifyOfPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        protected string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = (MemberExpression)expression.Body;
            return memberExpression.Member.Name;
        }
        #endregion

        #endregion

        #region Data

        private Dictionary<string, T> _valDic = new Dictionary<string, T>();

        #endregion

        #region Public Properties

        public event PropertyChangedEventHandler PropertyChanged;
        public Dictionary<string, T> ValueDictionary { get { return _valDic; } }

        #endregion
    }

    public class CategoryHelper<T> : PropertiesHelper<PropertiesHelper<T>>
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!base.TryGetMember(binder, out result))
                result = ValueDictionary[binder.Name] = new PropertiesHelper<T>();
            return true;
        }
    }
}
