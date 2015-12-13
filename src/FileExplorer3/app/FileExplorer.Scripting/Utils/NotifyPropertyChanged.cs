using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    /// <summary>
    /// Helper use this instead of PropertyChangedBase so it can be port to other framework.
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        protected virtual void NotifyOfPropertyChanged<T>(params Expression<Func<T>>[] expressions)
        {
            foreach (var expression in expressions)
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
