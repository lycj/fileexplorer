using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Utils;
using System.Text.RegularExpressions;
using FileExplorer.Defines;

namespace FileExplorer.Scripting
{
    public class ParameterDic : IParameterDic
    {
        #region Constructor

        public ParameterDic(IParameterDicStore store, params IParameterDic[] dics)
        {
            _store = store ?? new MemoryParameterDicStore();

            foreach (var dic in dics)
                foreach (string key in dic.List())
                    this._store.Add(key, dic.Get(key));
        }

        public ParameterDic(IParameterDicStore store, ParameterPair[] ppairs)
            : this(store)
        {
            foreach (var ppair in ppairs)
                this._store.Add(ppair.Key, ppair.Value);
        }

        public ParameterDic(params IParameterDic[] dics)
            : this(new MemoryParameterDicStore(), dics)
        {
        }

        #endregion

        #region Methods

        public T Get<T>(string variableKey, T defaultValue = default(T))
        {
            string variable = ParameterDicUtils.GetVariable(variableKey);
            string[] variableSplit = variable.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            var match = Regex.Match(variableSplit[0], RegexPatterns.ParseArrayCounterPattern);
            string varName = match.Groups["variable"].Value;
            //If the variable has an array or list index (e.g. array[0])
            int idx = match.Groups["counter"].Success ?
                Int32.Parse(match.Groups["counter"].Value) : -1;

            if (_store.ContainsKey(varName))
            {
                object initValue = _store[varName];
                if (initValue == null)
                    return defaultValue;
                if (initValue is ParameterDic && idx == -1 && variableSplit.Length > 1)
                {
                    //Omit the first variable.
                    string trailVariable = "{" + String.Join(".", variableSplit.Skip(1).ToArray()) + "}";
                    return (initValue as ParameterDic).Get<T>(trailVariable, defaultValue);
                }
                if (idx != -1 && initValue is Array)
                    initValue = (initValue as Array).GetValue(idx);
                var val = TypeInfoUtils.GetPropertyOrMethod(initValue, variableSplit.Skip(1).ToArray());
                if (val is T)
                    return (T)val;
            }

            return defaultValue;
        }

        public object Get(string variableKey)
        {
            return Get<object>(variableKey, null);
        }

        public IEnumerable<string> List()
        {
            return _store.Keys.Select((variable) => "{" + variable + "}");
        }

        public void Remove(params string[] variableKeys)
        {
            foreach (string variableKey in variableKeys)
            {
                string variable = ParameterDicUtils.GetVariable(variableKey);
                if (_store.ContainsKey(variable))
                    _store.Remove(variable);
            }
        }

        public bool Set<T>(string variableKey, T value = default(T), bool skipIfExists = false)
        {
            string variable = ParameterDicUtils.GetVariable(variableKey);
            string[] variableSplit = variable.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (variableSplit.Length > 1)
            {
                //If have more than one hierarchy (e.g. {Code.Directory.Temp})
                //Remove last node, and construct as variable key (e.g. {Code.Directory}))
                string parentKey = "{" + String.Join(".", variableSplit.Take(variableSplit.Length - 1)) + "}";
                string childPath = variableSplit.Last();
                string childKey = "{" + childPath + "}";
                object parentObject = Get(parentKey);

                //If ParentObject not found, create as ParameterDic.
                if (parentObject == null)
                {
                    parentObject = new ParameterDic();
                    Set<ParameterDic>(parentKey, (ParameterDic)parentObject, false);
                }

                if (parentObject is IParameterDic)
                {
                    (parentObject as IParameterDic).Set<T>(childKey, value, skipIfExists);
                    return true;
                }
                else
                {
                    TypeInfoUtils.SetProperty(parentObject, childPath, value);
                    return true;
                }
            }
            else
                if (_store.ContainsKey(variable))
            {
                if (!skipIfExists)
                {
                    if (!(_store[variable] is T) || !(_store[variable].Equals(value)))
                    {
                        _store[variable] = value;
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

        public async Task<IParameterDic> CloneAsync()
        {
            return new ParameterDic(this);
        }




        #endregion


        #region Data

        private IParameterDicStore _store;

        #endregion

        #region Public Properties

        public IParameterDicStore Store { get { return _store; } }

        #endregion
    }
}
