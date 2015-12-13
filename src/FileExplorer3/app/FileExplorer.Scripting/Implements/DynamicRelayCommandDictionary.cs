using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public class DynamicRelayCommandDictionary : DynamicDictionary<IScriptCommand>
    {
        #region Constructor

        public static string RelayCommandsKey = "RelayCommands";

        public DynamicRelayCommandDictionary(IEqualityComparer<string> comparer)
            : base(comparer)
        {
            _relayCmdDictionary = new DynamicDictionary<ScriptRelayCommand>(comparer);
            ParameterDicConverter = new NullParameterDicConverter();
            ScriptRunner = new FileExplorer.Script.ScriptRunner();
        }

        public DynamicRelayCommandDictionary()
            : this(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        #endregion

        #region Methods       

        public void SetCommand(string commandName, IScriptCommand command)
        {
            base[commandName] = command;
            RelayCommandDictionary[commandName] =
                 new ScriptRelayCommand(command, ParameterDicConverter, ScriptRunner);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder.Name.Equals(RelayCommandsKey, StringComparison.CurrentCultureIgnoreCase) ||
                binder.Name.EndsWith("Command", StringComparison.CurrentCultureIgnoreCase))
            {
                //throw new ArgumentException("Cannot set " + binder.Name);
                return false;
            }

            if (base.TrySetMember(binder, value))
            {
                RelayCommandDictionary[binder.Name] =
                    new ScriptRelayCommand(value as IScriptCommand, ParameterDicConverter, ScriptRunner);
                FirePropertyChanged(binder.Name + "Command");
            }
            return true;

        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name.Equals(RelayCommandsKey, StringComparison.CurrentCultureIgnoreCase))
            {
                result = RelayCommandDictionary;
                return true;
            } else 
                if (binder.Name.EndsWith("Command", StringComparison.CurrentCultureIgnoreCase))
                {
                    string key = binder.Name.Substring(0, binder.Name.Length - "Command".Length);
                    result = RelayCommandDictionary.Dictionary.ContainsKey(key) ? RelayCommandDictionary[key] :
                        new RelayCommand(pm => { }) { IsEnabled = false };                    
                    return true;
                }


            return base.TryGetMember(binder, out result);
        }

        #endregion

        #region Data

        private DynamicDictionary<ScriptRelayCommand> _relayCmdDictionary;

        #endregion

        #region Public Properties

        public DynamicDictionary<ScriptRelayCommand> RelayCommandDictionary { get { return _relayCmdDictionary; } }
        public IParameterDicConverter ParameterDicConverter { get; set; }
        public IScriptRunner ScriptRunner { get; set; }

        #endregion
    }
}
