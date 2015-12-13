using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FileExplorer.WPF.BaseControls
{
    /// <summary>
    /// Get Parent, value and subentries proc using lambda (if not null) instead of 
    /// using PropertyPath to improve speed.
    /// </summary>
    public class LambdaHierarchyHelper : PathHierarchyHelper
    {
        #region Constructor

        public LambdaHierarchyHelper(string parentPath, string valuePath, string subEntriesPath,
            Func<object, object> getParentProc = null, Func<object, string> getValueProc = null,
            Func<object, IEnumerable> getSubentriesProc = null)
            : base(parentPath, valuePath, subEntriesPath)
        {
            _getParentProc = getParentProc;
            _getValueProc = getValueProc;
            _getSubentriesProc = getSubentriesProc;
        }
        

        #endregion

        #region Methods

        public override string ExtractPath(string pathName)
        {
            if (pathName.EndsWith(":\\")) //Drive
                return "";
            else return base.ExtractPath(pathName);
        }

        public override string ExtractName(string pathName)
        {
            if (pathName.EndsWith(":\\"))  //Drive
                return pathName;
            else return base.ExtractName(pathName);
        }
        
        #endregion

        #region Data

        Func<object, object> _getParentProc;
        Func<object, string> _getValueProc;
        Func<object, IEnumerable> _getSubentriesProc;
        
        #endregion

        #region Public Properties

        protected override object getParent(object item)
        {
            return _getParentProc == null ? base.getParent(item) : _getParentProc(item);
        }

        protected override string getValuePath(object item)
        {
            return _getSubentriesProc == null ? base.getValuePath(item) : _getValueProc(item);
        }

        protected override IEnumerable getSubEntries(object item)
        {
            return _getSubentriesProc == null ? base.getSubEntries(item) : _getSubentriesProc(item);
        }
        
        #endregion

    }
}
