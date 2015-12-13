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
        /// Store position plus offset (Point) to destination.
        /// </summary>
        /// <param name="positionVariable"></param>
        /// <param name="offsetVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand OffsetPosition(string positionVariable = "{Position}", string offsetVariable = "{Offset}",
            string destinationVariable = "{Position}", IScriptCommand nextCommand = null)
        {
            return new OffsetPosition()
            {
                PositionKey = positionVariable,
                OffsetKey = offsetVariable,
                DestinationKey = destinationVariable,
                NextCommand =
                    (ScriptCommandBase)nextCommand
            };
        }

        /// <summary>
        /// Store position minus offset (Point) to destination.
        /// </summary>
        /// <param name="positionVariable"></param>
        /// <param name="offsetVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand OffsetPositionNeg(string positionVariable = "{Position}", string offsetVariable = "{Offset}",
            string destinationVariable = "{Position}", IScriptCommand nextCommand = null)
        {
            string offsetNegVariable = ParameterDic.CombineVariable(offsetVariable, "Neg", true);
            return HubScriptCommands.NegativePosition(offsetVariable, offsetNegVariable,
                HubScriptCommands.OffsetPosition(positionVariable, offsetNegVariable, destinationVariable, nextCommand));
        }

        public static IScriptCommand OffsetPositionValue(string positionVariable = "{Position}", Point offset = default(Point),
           string destinationVariable = "{Position}", IScriptCommand nextCommand = null)
        {
            return ScriptCommands.Assign("{OffsetPositionValue}", offset, false,
                OffsetPosition(positionVariable, "{OffsetPositionValue}", destinationVariable));
        }

        public static IScriptCommand NegativePosition(string positionVariable = "{Position}",
            string destinationVariable = "{Position}", IScriptCommand nextCommand = null)
        {
            return new NegativePosition()
            {
                PositionKey = positionVariable,
                DestinationKey = destinationVariable,
                NextCommand =
                    (ScriptCommandBase)nextCommand
            };
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="positionVariable"></param>
        ///// <param name="currentInput">Point to IUIInput.</param>
        ///// <param name="nextCommand"></param>
        ///// <returns></returns>
        //public static IScriptCommand OffsetScrollbarPosition(string positionVariable = "{Position}", string currentInput = "{Input}",
        //    string destinationVariable = "{Position}", IScriptCommand nextCommand = null)
        //{
        //    string offsetVariable = ParameterDic.CombineVariable(currentInput, (IUIInput inp) => inp.ScrollBarPosition, false);
        //    string offsetNegVariable = "{ScrollbarPos-Neg}";
        //    return
        //        HubScriptCommands.NegativePosition(offsetVariable, offsetNegVariable,
        //            HubScriptCommands.OffsetPosition(positionVariable, offsetNegVariable, destinationVariable, nextCommand));
        //}

        //public static IScriptCommand OffsetScrollbarPosition(string positionVariable = "{Position}", string currentInput = "{Input}",
        //    string startInput = "{StartInput}", string destinationVariable = "{Position}", IScriptCommand nextCommand = null)
        //{
        //    string currentOffsetVariable = ParameterDic.CombineVariable(currentInput, (IUIInput inp) => inp.ScrollBarPosition, false);
        //    string currentOffsetNegVariable = "{CurrentScrollbarPos-Neg}";
        //    string startOffsetVariable = ParameterDic.CombineVariable(startInput, (IUIInput inp) => inp.ScrollBarPosition, false);

        //    return
        //        HubScriptCommands.NegativePosition(currentOffsetVariable, currentOffsetNegVariable,
        //            HubScriptCommands.OffsetPosition(positionVariable, currentOffsetNegVariable, destinationVariable,
        //            HubScriptCommands.OffsetPosition(destinationVariable, startOffsetVariable, destinationVariable, nextCommand)));
        //}
    }

    public class OffsetPosition : ScriptCommandBase
    {
        /// <summary>
        /// Position (Point) to process, Default = {Position}.
        /// </summary>
        public string PositionKey { get; set; }

        /// <summary>
        /// Offset position (Point), Default = {Offset}.
        /// </summary>
        public string OffsetKey { get; set; }

        /// <summary>
        /// Store result to (Point), Default = {Position}.
        /// </summary>
        public string DestinationKey { get; set; }



        public OffsetPosition()
            : base("OffsetPosition")
        {
            PositionKey = "{Position}";
            OffsetKey = "{Offset}";
            DestinationKey = "{Position}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            Point pos = pm.GetValue<Point>(PositionKey);
            Point offsetpos = pm.GetValue<Point>(OffsetKey);

            pm.SetValue(DestinationKey, new Point(pos.X + offsetpos.X, pos.Y + offsetpos.Y));

            return NextCommand;
        }
    }

    public class NegativePosition : ScriptCommandBase
    {
        /// <summary>
        /// Position (Point) to process, Default = {Position}.
        /// </summary>
        public string PositionKey { get; set; }

        /// <summary>
        /// Store result to (Point), Default = {Position}.
        /// </summary>
        public string DestinationKey { get; set; }

        public NegativePosition()
            : base("OffsetPosition")
        {
            PositionKey = "{Position}";
            DestinationKey = "{Position}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            Point pos = pm.GetValue<Point>(PositionKey);

            pm.SetValue(DestinationKey, Point.Multiply(pos, new System.Windows.Media.Matrix(-1, 0, 0, -1, 0, 0)));

            return NextCommand;
        }
    }
}
