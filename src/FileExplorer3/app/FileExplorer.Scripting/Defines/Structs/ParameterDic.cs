using FileExplorer.Defines;
using FileExplorer.Utils;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace FileExplorer
{
    public class ParameterPair
    {
        public object Variable { get { return "{" + Key + "}"; } }
        public string Key { get; set; }
        public object Value { get; set; }
        private ParameterPair(string key, object value) { Key = key; Value = value; }

        public static ParameterPair FromKey(string key, object value) { 
            return new ParameterPair(key, value); }
        public static ParameterPair FromVariable(string variable, object value) { 
            return new ParameterPair(ParameterDic.GetVariable(variable), value); }
    }

    public static partial class ExtensionMethods
    {
        public static string ReplaceVariableInsideBracketed(this ParameterDic pd, string variableKey)
        {
            if (variableKey == null)
                return null;

            Regex regex = new Regex("{(?<TextInsideBrackets>[^}]+)}");
            string value = variableKey;

            Match match = regex.Match(value);

            while (!value.StartsWith("::") && match.Success)
            {
                string key = "{" + match.Groups["TextInsideBrackets"].Value + "}";
                object val = pd.GetValue(key);
                value = value.Replace(key, val == null ? "" : val.ToString());
                match = regex.Match(value);
            }

            return value;
        }
    }

    public class ParameterDic : ICollection<KeyValuePair<string, object>>
    {
        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ParameterDic>();
        public static ParameterDic Empty = new ParameterDic();
        private IParameterDicStore _store;

        #region Constructor

        public ParameterDic(IParameterDicStore store = null)
        {
            _store = store ?? new MemoryParameterDicStore();
        }

        public ParameterDic(params ParameterPair[] ppairs)
            : this(new MemoryParameterDicStore(ppairs))
        {

        }

        #endregion

        #region Static function

        public static string GetVariable(string variableKey)
        {
            if (!(variableKey.StartsWith("{") && variableKey.EndsWith("}")))
                throw new ArgumentException(variableKey + " is not a valid variable.");
            return variableKey.TrimStart('{').TrimEnd('}');
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableKey"></param>
        /// <param name="combineStr"></param>
        /// <param name="checkVariablekey">If true, all dot in variableKey is removed.</param>
        /// <returns></returns>
        public static string CombineVariable(string variableKey, string combineStr, bool checkVariablekey = true)
        {
            if (checkVariablekey)
                variableKey = variableKey.Replace(".", "");
            return "{" + GetVariable(variableKey) + combineStr + "}";
        }

        public static string CombineVariable<C, T>(string variableKey, Expression<Func<C, T>> expression, bool checkVariablekey = true)
        {
            string combineStr = "." + ExpressionUtils.GetPropertyName(expression);
            return CombineVariable(variableKey, combineStr, checkVariablekey);
        }

        public static string RandomVariable(string prefix = "")
        {
            return "{" + prefix + new Random().Next().ToString() + "}";
        }

        private static ParameterDic combine(ParameterDic orginalDic, ParameterDic newDic)
        {
            ParameterDic retDic = orginalDic.Clone();
            if (newDic != null)
                foreach (var v in newDic.VariableNames)
                    retDic.SetValue(v, newDic.GetValue(v), true);
            return retDic;
        }

        public static ParameterDic Combine(params ParameterDic[] dics)
        {
            ParameterDic retDic = dics.First();
            foreach (var d in dics.Skip(1))
                retDic = combine(retDic, d);
            return retDic;
        }

        #endregion

        #region Has / Get / Set / Clear Value

        public bool HasValue(string variableKey)
        {
            return HasValue<Object>(variableKey);
        }

        public bool HasValue<T>(string variableKey)
        {
            if (variableKey == null)
                return false;
            return GetValue(variableKey) != null;
            //string variable = GetVariable(variableKey);
            //string[] variableSplit = variable.Split('.');
            //return _store.ContainsKey(variableSplit.First()) && _store[variableSplit.First()] is T;
        }

        public T GetValue<T>(string variableKey, T defaultValue)
        {
            if (variableKey == null)
                return defaultValue;

            string variable = GetVariable(variableKey);

            string[] variableSplit = variable.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            var match = Regex.Match(variableSplit[0], RegexPatterns.ParseArrayCounterPattern);
            string varName = match.Groups["variable"].Value;
            int idx = match.Groups["counter"].Success ? Int32.Parse(match.Groups["counter"].Value) : -1;

            if (_store.ContainsKey(varName))
            {
                object initValue = _store[varName];
                if (initValue == null)
                    return default(T);
                if (initValue is ParameterDic && idx == -1 && variableSplit.Length > 1)
                {
                    //Omit the first variable.
                    string trailVariable = "{" + String.Join(".", variableSplit.Skip(1).ToArray()) + "}";
                    return (initValue as ParameterDic).GetValue<T>(trailVariable);
                }
                if (idx != -1 && initValue is Array)
                    initValue = (initValue as Array).GetValue(idx);
                var val = TypeInfoUtils.GetPropertyOrMethod(initValue, variableSplit.Skip(1).ToArray());
                if (val is T)
                    return (T)val;
            }

            return defaultValue;
        }

        public T GetValue<T>(string variableKey)
        {
            return GetValue<T>(variableKey, default(T));
        }

        public object GetValue(string variableKey)
        {
            return GetValue<Object>(variableKey);
        }

        public bool ClearValue(string variableKey)
        {
            string variable = GetVariable(variableKey);
            if (_store.ContainsKey(variable))
            {
                _store.Remove(variable);
                return true;
            }
            return false;
        }

        public bool SetValue<T>(string variableKey, T value, bool skipIfExists = false)
        {
            if (variableKey == null)
                return false;

            string variable = GetVariable(variableKey);
            string[] variableSplit = variable.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (variableSplit.Length > 1)
            {        
                string parentKey = "{" + String.Join(".", variableSplit.Take(variableSplit.Length -1)) + "}";
                string childPath = variableSplit.Last();
                string childKey = "{" + childPath + "}";
                object parentObject = GetValue(parentKey);
                if (parentObject is ParameterDic)
                {
                    (parentObject as ParameterDic).SetValue<T>(childKey, value, skipIfExists);
                    return true;
                }
                else 
                {
                    if (parentObject != null)
                    {
                        TypeInfoUtils.SetProperty(parentObject, childPath, value);
                        return true;
                    }
                }
                return false;
            }
            else
                if (_store.ContainsKey(variable))
                {
                    if (!skipIfExists)
                    {
                        if (!(_store[variable] is T) || !(_store[variable].Equals(value)))
                        {
                            this[variable] = value;
                            return true;
                        }
                        else return false;

                    }
                    else
                    {

                        return false;
                    }
                }
                else
                {
                    _store.Add(variable, value);
                    return true;

                }
        }

        #endregion

        #region Clone

        public ParameterDic Clone()
        {
            return new ParameterDic(_store.Clone());
        }

        #endregion

        #region Obsoluted function.

        private string convertVariable(string key)
        {
            return key.StartsWith("{") ? key : "{" + key + "}";
        }

        [Obsolete("SetValue")]
        public void AddOrUpdate(string key, object value)
        {
            key = convertVariable(key);
            SetValue(key, value);
        }

        [Obsolete("Variables")]
        public IEnumerable<string> Keys { get { return _store.Keys; } }

        [Obsolete()]
        public object this[string key]
        {
            get { return Store[key]; }
            set { Store[key] = value; }
        }

        #endregion

        #region Public Properties

        public bool IsHandled
        {
            get { return GetValue<bool>("{Handled}", false); }
            set { SetValue<bool>("{Handled}", value); }
        }

        public object Parameter
        {
            get { return GetValue<object>("{Parameter}", null); }
            set { SetValue<object>("{Parameter}", value); }
        }

        public CancellationToken CancellationToken
        {
            get { return GetValue<CancellationToken>("{CancellationToken}", CancellationToken.None); }
            set { SetValue<CancellationToken>("{CancellationToken}", value); }
        }

        public IParameterDicStore Store { get { return _store; } }

        public IEnumerable<ParameterPair> Variables { get { return VariableNames.Select(v => ParameterPair.FromKey(v, GetValue(v))); } }

        /// <summary>
        /// Most exception is throw directly, if not, it will set the Error property, which will be thrown 
        /// in PropertyInvoker.ensureNoError() method.
        /// </summary>
        public Exception Error
        {
            get { return GetValue<Exception>("{Error}", null); }
            set { SetValue<Exception>("{Error}", value); }
        }

        public IEnumerable<string> VariableNames { get { return _store.Keys.Select(k => "{" + k + "}"); } }

        public List<string> CommandHistory = new List<string>();

        #endregion

        #region ICollection<KeyValuePair<string, object>> members

        public void Add(string key, object value)
        {
            Store.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return Store.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return Store.Remove(key);
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get { return Store.Count; }
        }

        public bool IsReadOnly
        {
            get { return Store.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Store.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Store.GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Store.Add(item.Key, item.Value);
        }

        #endregion
    }
}
