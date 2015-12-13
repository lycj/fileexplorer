using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.Utils
{
    //http://reyrahadian.wordpress.com/2012/02/01/creating-a-dynamic-dictionary-with-c-4-dynamic/
    //http://blog.lab49.com/archives/3893
    public class DynamicDictionary<TValue> : DynamicObject, INotifyPropertyChanged
    {
        private Dictionary<string, TValue> _dictionary;
        protected IEqualityComparer<string> _comparer;

        public Dictionary<string, TValue> Dictionary { get { return _dictionary; } }


        public DynamicDictionary(IEqualityComparer<string> comparer, bool throwIfNotAssigned = false)
        {
            _dictionary = new Dictionary<string, TValue>(comparer);
            _comparer = comparer;
            _throwIfNotAssigned = throwIfNotAssigned;
        }

        public DynamicDictionary(bool throwIfNotAssigned = false)
            : this(StringComparer.CurrentCulture, throwIfNotAssigned)
        {            
        }

        public TValue this[string key] { get { return _dictionary[key]; }
            set
            {
                if (_dictionary.ContainsKey(key))
                    _dictionary[key] = value;
                else _dictionary.Add(key, value);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            TValue data;
            if (!_dictionary.TryGetValue(binder.Name, out data))
            {
                if (_throwIfNotAssigned)
                    throw new KeyNotFoundException("There's no key by that name");
            }
            result = data;
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (_dictionary.ContainsKey(binder.Name))
            {
                _dictionary[binder.Name] = (TValue)value;
            }
            else
            {
                _dictionary.Add(binder.Name, (TValue)value);
            }
            FirePropertyChanged(binder.Name);
            return true;
        }

        public void FirePropertyChanged(string propName)
        {
            var propChange = PropertyChanged;
            if (propChange != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

         public event PropertyChangedEventHandler PropertyChanged;
         private bool _throwIfNotAssigned;
    }
}
