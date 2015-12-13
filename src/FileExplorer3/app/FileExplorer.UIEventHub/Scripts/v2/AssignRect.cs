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
        /// Create a rect object from two position.
        /// </summary>
        /// <param name="position1Variable"></param>
        /// <param name="position2Variable"></param>
        /// <param name="variable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AssignRectFromPosition(string position1Variable, string position2Variable, string variable,
            bool skipIfExists = false,
            IScriptCommand nextCommand = null)
        {
            return new AssignRect()
            {
                Position1Key = position1Variable,
                Position2Key = position2Variable,
                VariableKey = variable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Create a rect object from a position and a size.
        /// </summary>
        /// <param name="position1Variable"></param>
        /// <param name="sizeVariable"></param>
        /// <param name="variable"></param>
        /// <param name="skipIfExists"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand AssignRectFromSize(string position1Variable, string sizeVariable, string variable,
            bool skipIfExists = false,
            IScriptCommand nextCommand = null)
        {
            return new AssignRect()
            {
                Position1Key = position1Variable,
                SizeKey = sizeVariable,
                VariableKey = variable,
                SkipIfExists = skipIfExists,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }
    }

    /// <summary>
    /// Given two positions (or a position and a size), create a rect at destination in parameterdic.
    /// </summary>
    public class AssignRect : Assign
    {
        public string Position1Key { get; set; }
        public string Position2Key { get; set; }
        public string SizeKey { get; set; }

        public AssignRect()
            : base("AssignRect")
        {

        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            Point pos1 = pm.GetValue<Point>(Position1Key);
            if (pm.HasValue(Position2Key))
            {
                Point pos2 = pm.GetValue<Point>(Position2Key);
                Value = new Rect(pos1, pos2);
            }
            else
            {
                Size size = pm.GetValue<Size>(SizeKey);
                Value = new Rect(pos1, size);
            }

            return base.Execute(pm);
        }
    }
}
