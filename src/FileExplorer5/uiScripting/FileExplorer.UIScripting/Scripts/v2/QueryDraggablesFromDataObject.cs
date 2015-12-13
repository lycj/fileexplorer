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
        /// Obtain draggables (IDraggable[]) and assign to a variable, or assign null if not found.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="iSupportDropVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand QueryDraggablesFromDataObject(
            string iSupportDropVariable = "{ISupportDrop}", string dataObjectVariable = "{DataObj}",
            string destinationVariable = "{Draggables}", bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            return new QueryDraggablesFromDataObject()
            {
                ISupportDropKey = iSupportDropVariable,
                DestinationKey = destinationVariable,
                DataObjectKey = dataObjectVariable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };

            //IScriptCommand cmd = ScriptCommands.ExecuteFunc(iSupportDropVariable,
            //   (ISupportShellDrop isd) => isd.QueryDropDraggables(null), 
            //   new object[] { dataObjectVariable }, destinationVariable, nextCommand);
            //return skipIfExists ? ScriptCommands.IfNotAssigned(destinationVariable, cmd) : cmd;
        }

        /// <summary>
        /// Obtain DataObject (IDataObject) and assign to a variable, or assign null if not found.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="iSupportDropVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand QueryDataObjectFromDraggables(
            string iSupportDragVariable = "{ISupportDrag}", string draggablesVariable = "{Draggables}",
            string destinationVariable = "{DataObj}", bool skipIfExists = false, IScriptCommand nextCommand = null)
        {
            IScriptCommand cmd = ScriptCommands.ExecuteFunc(iSupportDragVariable,
                (ISupportDrag isd) => isd.GetDraggables(),
                new object[] { }, destinationVariable, nextCommand);
            return skipIfExists ? ScriptCommands.IfNotAssigned(destinationVariable, cmd) : cmd;
        }


        /// <summary>
        /// Obtain DataObject, Draggables and DropEffects, by calling AssignDataObject, QuerryDraggablesFromDataObject
        /// and QueryDropEffects() script commands.
        /// </summary>
        /// <param name="iSupportDropVariable"></param>
        /// <param name="destinationDataObjectVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand QueryShellDragInfo(
            string iSupportDropVariable = "{ISupportDrop}", string destinationDataObjectVariable = "{DataObj}",
           string destinationVariable = "{Draggables}", string queryDropResultVariable = "{QueryDropResult}",
            bool skipIfExists = false, IScriptCommand successCommand = null, IScriptCommand otherwiseCommand = null)
        {

            return HubScriptCommands.AssignDataObject(destinationDataObjectVariable, false,
                        HubScriptCommands.QueryDraggablesFromDataObject(iSupportDropVariable, destinationDataObjectVariable, destinationVariable, false,
                //And if there's draggables,
                            ScriptCommands.IfAssigned("{DragDrop.Draggables}",
                              ScriptCommands.IfNotEquals("{DragDrop.Draggables.Count()}", 0,
                //Call ISupportDrop.QueryDropEffects() to get QueryDropEffect.
                               HubScriptCommands.QueryDropEffects(iSupportDropVariable, destinationVariable, destinationDataObjectVariable, null,
                                  queryDropResultVariable, false,
                                  ScriptCommands.IfEquals(queryDropResultVariable, FileExplorer.Defines.QueryDropEffects.None,
                                  otherwiseCommand,
                                  successCommand)), otherwiseCommand), otherwiseCommand)));
        }

    }

    /// <summary>
    /// Obtain draggables array (IDraggable[]) and assign to a variable, or assign null if not found.
    /// </summary>
    public class QueryDraggablesFromDataObject : ScriptCommandBase
    {
        /// <summary>
        /// Point to a ViewModel that support ISupportShellDrop or ISupportDrop, Default = {ISupportDrop}.
        /// </summary>
        public string ISupportDropKey { get; set; }

        /// <summary>
        /// Point to DataObject, or obtain from  Default = {DataObj}.
        /// </summary>
        public string DataObjectKey { get; set; }

        /// <summary>
        /// Point to where draggable (IDraggable[]) is stored, Default = {Draggables}.
        /// </summary>
        public string DestinationKey { get; set; }

        public bool SkipIfExists { get; set; }

        public QueryDraggablesFromDataObject()
            : base("AssignDraggables")
        {
            ISupportDropKey = "{ISupportDrop}";
            DataObjectKey = "{DataObj}";
            DestinationKey = "{Draggables}";
            SkipIfExists = false;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            if (SkipIfExists && pm.HasValue(DestinationKey))
                return NextCommand;

            IEnumerable<IDraggable> value = new List<IDraggable>();

            ISupportShellDrop issd = pm.GetValue<ISupportShellDrop>(ISupportDropKey);
            IDataObject dataObj = pm.GetValue<IDataObject>(DataObjectKey);

            if (dataObj.GetDataPresent(typeof(ISupportDrag)))
            {
                ISupportDrag isd = (ISupportDrag)dataObj.GetData(typeof(ISupportDrag));
                value = isd.GetDraggables();
            }
            else
                if (issd != null && dataObj != null)
                    value = (issd.QueryDropDraggables(dataObj) ?? new List<IDraggable>());

            pm.SetValue(DestinationKey, value, SkipIfExists);
            return NextCommand;
        }
    }
}
