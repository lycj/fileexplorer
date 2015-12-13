using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Scripting
{
    /// <summary>
    /// Property Dictionary 
    /// </summary>
    public interface IParameterDic 
    {
        /// <summary>
        /// Returns a property value from property key.
        /// </summary>
        /// <param name="variableKey"></param>
        /// <returns></returns>
        object Get(string variableKey);

        /// <summary>
        /// Returns a property value from property key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T Get<T>(string variableKey, T defaultValue = default(T));
        
        /// <summary>
        /// Assign a property from property key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="variableKey"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Set<T>(string variableKey, T value = default(T), bool skipIfExists = false);

        /// <summary>
        /// Remove a property from property dictionary.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        void Remove(params string[] variableKeys);
    
        /// <summary>
        /// List all property keys in the property dictionary.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> List();

        Task<IParameterDic> CloneAsync();



        #region Public Properties

        IParameterDicStore Store { get; }

        #endregion

    }
}
