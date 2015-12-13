using FileExplorer.Script;
using FileExplorer.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FileExplorer.UIEventHub
{

    public static partial class HubScriptCommands
    {
        public static IScriptCommand AssignDataContext(
            string sourceElementVariable = "{EventArgs.OriginalSource}",
            DataContextType type = DataContextType.SupportDrag,
            string destVariable = "{Variable}",
            string destEleVariable = null,
            bool skipIfExists = false,
            IScriptCommand nextCommand = null)
        {
            return new AssignDataContext()
            {
                SourceElementKey = sourceElementVariable,
                DataContextType = type,
                VariableKey = destVariable,
                DataContextElementKey = destEleVariable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand IfHasDataContext(
           string sourceElementVariable = "{EventArgs.OriginalSource}",
           DataContextType type = DataContextType.SupportDrag,           
           IScriptCommand nextCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string destVariable = "{Temp_DataContextDest}";
            string destEleVariable = "{Temp_DataContextDestEle}";
            return AssignDataContext(sourceElementVariable, type, destVariable, destEleVariable, false,
                ScriptCommands.IfAssigned(destVariable, nextCommand, otherwiseCommand));
        }

    }

    public enum DataContextType { Any, SupportDrag, SupportShellDrag, SupportDrop, SupportShellDrop }

    /// <summary>
    /// Use DataContextFinder to lookup up from the Visual Tree of ElementKey to find a 
    /// DataContext that match  the type.
    /// </summary>
    public class AssignDataContext : Assign
    {
        /// <summary>
        /// Point to Element (FrameworkElement) to lookup, Default = {EventArgs.OriginalSource}
        /// </summary>
        public string SourceElementKey { get; set; }

        /// <summary>
        /// Optional, point to the element that host the lookup DataContext, Default = null
        /// </summary>
        public string DataContextElementKey { get; set; }


        public DataContextType DataContextType { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<AssignDataContext>();

        public AssignDataContext()
            : base("AssignDataContext")
        {
            SourceElementKey = "{EventArgs.OriginalSource}";
            DataContextElementKey = null;            
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            FrameworkElement origSource = pm.GetValue<FrameworkElement>(SourceElementKey);
            Value = null;
            FrameworkElement ele = null;

            if (origSource != null)
            {
                switch (DataContextType)
                {
                    case UIEventHub.DataContextType.Any:
                        Value = origSource.DataContext;
                        ele = origSource;
                        break;
                    case UIEventHub.DataContextType.SupportDrag:
                        Value = DataContextFinder.GetDataContext(origSource, out ele, DataContextFinder.SupportDrag);
                        break;
                    case UIEventHub.DataContextType.SupportShellDrag:
                        Value = DataContextFinder.GetDataContext(origSource, out ele, DataContextFinder.SupportShellDrag);
                        break;
                    case UIEventHub.DataContextType.SupportShellDrop:
                        Value = DataContextFinder.GetDataContext(origSource, out ele, DataContextFinder.SupportShellDrop);
                        break;
                    case UIEventHub.DataContextType.SupportDrop:
                        Value = DataContextFinder.GetDataContext(origSource, out ele, DataContextFinder.SupportDrop);
                        break;
                    default:
                        return ResultCommand.Error(new NotSupportedException("DataContextType"));
                }
            }

            pm.SetValue(VariableKey, Value, SkipIfExists);
            pm.SetValue(DataContextElementKey, ele, SkipIfExists);
            return NextCommand;
        }



    }
}
