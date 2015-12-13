using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        /// <summary>
        /// Obtain dataobject from DragInputArgs and assign to a variable, or assign null if not found.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="iSupportDropVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AssignDataObject(string destinationVariable = "{DataObj}", bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new AssignDataObject()
            {                
                DestinationKey = destinationVariable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// When dragging, Obtain dataobject from Input (IUIDragInput) and assign to a variable, or assign null if not found.
    /// </summary>
    public class AssignDataObject : UIScriptCommandBase
    {        
        /// <summary>
        /// Point to where dataobject (IDataObject) is stored, Default = {DataObj}.
        /// </summary>
        public string DestinationKey { get; set; }

        public bool SkipIfExists { get; set; }

        public AssignDataObject()
            : base("AssignDataObject")
        {
            DestinationKey = "{DataObj}";
            SkipIfExists = false;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            if (SkipIfExists && pm.HasValue(DestinationKey))
                return NextCommand;

            IUIDragInput dragInp = pm.GetValue<IUIDragInput>(base.InputKey);
            if (dragInp != null)
                pm.SetValue(DestinationKey, dragInp.Data, SkipIfExists);
            return NextCommand;
        }
    }
}
