using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Models
{
    public interface IEntryModelCache<M> where M : IEntryModel
    {
        M RegisterModel(M model);
        void RegisterChildModels(M parentModel, M[] childModels);
        M[] GetChildModel(M parentModel);

        /// <summary>
        /// Make sure input path is recognizable by ModelCache
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string CheckPath(string path);

        string GetUniqueId(string path);
        string GetPath(string uniqueId);
        M GetModel(string uniqueId);

    }

    public class EntryModelCache<M> : IEntryModelCache<M> where M : IEntryModel
    {
        
        #region Constructor

        public EntryModelCache(Func<M, string> uniqueIdFunc, Func<string> aliasFunc, bool ignoreCase = true)
        {
            _uniqueIdFunc = uniqueIdFunc;
            _aliasFunc = aliasFunc;
            _stringComparison = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
            var stringComparer = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;
            UniqueIdLookup = new ConcurrentDictionary<string, string>(2, 5, stringComparer);
            ChildLookup = new ConcurrentDictionary<string, IList<string>>(2, 5, stringComparer);
            ModelCache = new ConcurrentDictionary<string, M>(2, 5, stringComparer);
        }

        #endregion

        #region Methods


       
        public string CheckPath(string path)
        {
            string alias = _aliasFunc();
            if (string.IsNullOrEmpty(path))
                return alias;

            path = path.TrimStart('/');

            if (path.StartsWith(alias, _stringComparison))
                if (path.Equals(alias, _stringComparison))
                    path = "";
                else path = path.Substring((alias).Length + 1);

            return String.IsNullOrEmpty(path) ? alias : alias + '/' + path;
        }        

        public M RegisterModel(M model)
        {
            UniqueIdLookup[model.FullPath] = _uniqueIdFunc(model);
            ModelCache[_uniqueIdFunc(model)] = model;
            return model;
        }

        public string GetUniqueId(string fullPath)
        {
            if (!(UniqueIdLookup.ContainsKey(fullPath)))
                return null;
            else return UniqueIdLookup[fullPath];
        }

        public string GetPath(string uniqueId)
        {            
            var ppair = UniqueIdLookup.FirstOrDefault(pp => pp.Value == uniqueId);
            if (ppair.Equals(default(KeyValuePair<string, string>)))
                return null;
            else return ppair.Key;
        }

        public M GetModel(string uniqueId)
        {
            return ModelCache[uniqueId];
        }

        public void RegisterChildModels(M parentModel, M[] childModels)
        {
            foreach (var cm in childModels)
                RegisterModel(cm);
            ChildLookup[_uniqueIdFunc(parentModel)] = (from cm in childModels select _uniqueIdFunc(cm)).ToList();
        }

        public M[] GetChildModel(M parentModel)
        {
            if (ChildLookup.ContainsKey(_uniqueIdFunc(parentModel)))
            {
                return (from uid in ChildLookup[_uniqueIdFunc(parentModel)] select GetModel(uid)).ToArray();
            }
            else return null;
        }

        #endregion

        #region Data

        private Func<M, string> _uniqueIdFunc;
        private StringComparison _stringComparison;
        private Func<string> _aliasFunc;

        #endregion

        #region Public Properties

        /// <summary>
        /// Dictionary for path -> uniqueId
        /// </summary>
        public ConcurrentDictionary<string, string> UniqueIdLookup { get; private set; }

        /// <summary>
        /// Dictionary for uniqueId -> child UniqueId list
        /// </summary>
        public ConcurrentDictionary<string, IList<string>> ChildLookup { get; private set; }

        /// <summary>
        /// DIctionary for uniqueId -> IEntryModel
        /// </summary>
        public ConcurrentDictionary<string, M> ModelCache { get; private set; }


        #endregion

    }
}
