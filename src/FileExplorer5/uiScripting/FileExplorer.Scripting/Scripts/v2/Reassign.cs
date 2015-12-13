using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Scripting
{
    public static partial class ScriptCommands
    {
        public static IScriptCommand Reassign(string sourceVariableKey = "{SourceVariable}", string valueConverterKey = null,
            string destinationVariableKey = "{DestinationVariable}", IScriptCommand nextCommand = null)
        {
            return new Reassign()
            {
                SourceVariableKey = sourceVariableKey,
                ValueConverterKey = valueConverterKey,
                DestinationVariableKey = destinationVariableKey,
                SkipIfExists = false,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand ReassignToParameter(string sourceVariableKey = "{SourceVariable}", IScriptCommand nextCommand = null)
        {
            return Reassign(sourceVariableKey, null, "{Parameter}", nextCommand);
        }    
    }

    /// <summary>
    /// Serializable, assign value of a variable in ParameterDic to another variable.
    /// </summary>
    public class Reassign : ScriptCommandBase
    {
        /// <summary>
        /// Variable name to obtain from, default = "SourceVariable".
        /// </summary>
        public string SourceVariableKey { get; set; }

        /// <summary>
        /// Func[object,object] to convert SourceVariable to DestinationVariable, default = null.
        /// </summary>
        public string ValueConverterKey { get; set; }
        
        /// <summary>
        /// Variable name to set to, default = "DestinationVariable".
        /// </summary>
        public string DestinationVariableKey { get; set; }

        /// <summary>
        /// Whether skip (or override) if key already in dictionary, default = false.
        /// </summary>
        public bool SkipIfExists { get; set; }

        private static ILog logger = LogManager.GetLogger<Reassign>();

        public Reassign()
            : base("Reassign")
        {
            SourceVariableKey = "SourceVariable";
            ValueConverterKey = null;
            DestinationVariableKey = "DestinationVariable";
            SkipIfExists = false;
        }

        public override IScriptCommand Execute(IParameterDic pm)
        {
            object source = pm.Get<object>(SourceVariableKey);
            if (source == null)
                logger.Error("Source not found.");

            if (ValueConverterKey != null)
            {
                object valueConverter = pm.Get<object>(ValueConverterKey);

                if (valueConverter is Func<object, object>) //GetProperty, ExecuteMethod, GetArrayItem
                {
                    Func<object, object> valueConverterFunc = valueConverter as Func<object, object>;
                    object value = valueConverterFunc(source);
                    pm.Set(DestinationVariableKey, value, SkipIfExists);
                }
                else
                    if (valueConverter is Action<object, object>) //SetProperty
                {
                    Action<object, object> valueConverterAct = valueConverter as Action<object, object>;
                    object value = pm.Get<object>(DestinationVariableKey);
                    valueConverterAct(source, value);
                }
            }
            else pm.Set(DestinationVariableKey, source, SkipIfExists);
          
            return NextCommand;
        }
    }


}
