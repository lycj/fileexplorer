using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.Defines;
using FileExplorer.Models;

namespace FileExplorer.WPF.Models
{
    public class NotifyChangedCommand : ScriptCommandBase
    {

        private string _destParseName;
        private ChangeType _changeType;
        private IProfile _destProfile;
        private IProfile _srcProfile;
        private string _srcParseName;

        public NotifyChangedCommand(IProfile destProfile, string destParseName, ChangeType changeType,
             IScriptCommand nextCommand = null)
            : base("NotifyChanged")
        {
            _destProfile = destProfile;
            _destParseName = destParseName;
            _changeType = changeType;
            _nextCommand = nextCommand;
        }

        public NotifyChangedCommand(IProfile destProfile, string destParseName,
            IProfile srcProfile, string srcParseName, ChangeType changeType,
           IScriptCommand nextCommand = null)
            : this(destProfile, destParseName, changeType, nextCommand)
        {
            _srcProfile = srcProfile;
            _srcParseName = srcParseName;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {

            if (_changeType == ChangeType.Moved && _srcProfile != null && _srcParseName != null
                && _srcProfile != _destProfile)
            {
                _srcProfile.NotifyEntryChanges(this, _srcParseName, ChangeType.Deleted);
                _destProfile.NotifyEntryChanges(this, _destParseName, ChangeType.Created);
            }
            else 
                _destProfile.NotifyEntryChanges(this, _destParseName, _changeType, _srcParseName);

            return _nextCommand;
        }

        public override bool Equals(object obj)
        {
            return obj is NotifyChangedCommand && (obj as NotifyChangedCommand)._destProfile.Equals(_destProfile) &&
                (obj as NotifyChangedCommand)._destParseName.Equals(_destParseName);
        }

        public override int GetHashCode()
        {
            return _destParseName.GetHashCode() + _changeType.GetHashCode();
        }
    }



}
