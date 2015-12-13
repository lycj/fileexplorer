//using System;
//using System.Reflection;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Diagnostics;
//using FileExplorer.WPF.Utils;
//using System.Linq.Expressions;
//using FileExplorer.Utils;
//using MetroLog;
//using FileExplorer.Script;
//using System.Windows;

//namespace FileExplorer.UIEventHub
//{


//    public static partial class HubScriptCommands
//    {
//        #region Add
        
//        public static IScriptCommand PointAdd(string value1Variable = "{Value1}", string value2Variable = "{Value2}",
//            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
//        {
//            return new PointArithmeticCommand()
//            {
//                Value1Key = value1Variable,
//                Value2Key = value2Variable,
//                VariableKey = destinationVariable,
//                OperatorType = ArithmeticOperatorType.Add,
//                NextCommand = (ScriptCommandBase)nextCommand
//            };
//        }

//        public static IScriptCommand PointAddValue(string value1Variable,
//            Point value2,
//            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
//        {            
//            string value2Variable = ParameterDic.RandomVariable();
//            return ScriptCommands.Assign(value2Variable, value2, false,
//                 PointAdd(value1Variable, value2Variable, destinationVariable, nextCommand));                
//        }
        
//        #endregion 

//        #region Subtract
        
//        public static IScriptCommand PointSubtract(string value1Variable = "{Value1}",
//            string value2Variable = "{Value2}",
//            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
//        {
//            return new PointArithmeticCommand()
//            {
//                Value1Key = value1Variable,
//                Value2Key = value2Variable,
//                VariableKey = destinationVariable,
//                OperatorType = ArithmeticOperatorType.Subtract,
//                NextCommand = (ScriptCommandBase)nextCommand
//            };
//        }

//        public static IScriptCommand PointSubtractValue(string value1Variable, Point value2, 
//            string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
//        {            
//            string value2Variable = ParameterDic.RandomVariable();
//            return ScriptCommands.Assign(value2Variable, value2, false,
//                 PointSubtract(value1Variable, value2Variable, destinationVariable, nextCommand));
//        }

//        #endregion

     
//    }
   
//    /// <summary>
//    /// Use Expression to do arithmetic operations (Take Value1 and Value2) , and assign the result to VariableKey.
//    /// </summary>
//    public class PointArithmeticCommand : Assign
//    {

//        /// <summary>
//        /// Specify which converter to use, default = GetProperty
//        /// </summary>
//        public ArithmeticOperatorType OperatorType { get; set; }

//        /// <summary>
//        /// Point to the first value (the , default = {Value1}.
//        /// </summary>
//        public string Value1Key { get; set; }

//        /// <summary>
//        /// Point to the second value (the , default = {Value2}.
//        /// Note: second value can be an array, in this case, the arithmetic operations will be done multiple times.
//        /// </summary>
//        public string Value2Key { get; set; }

//        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ArithmeticCommand>();

//        public PointArithmeticCommand()
//            : base("PointArithmeticCommand")
//        {
//            OperatorType = ArithmeticOperatorType.Add;
//            VariableKey = "{Result}";
//            Value1Key = "{Value1}";
//            Value2Key = "{Value2}";
//            Value = null;
//            SkipIfExists = false;
//        }

//        public override IScriptCommand Execute(ParameterDic pm)
//        {
//            Point firstValue = pm.GetValue<Point>(Value1Key);
//            Point secondValue = pm.GetValue<Point>(Value2Key);
            
//            switch (OperatorType)
//            {
//                case ArithmeticOperatorType.Add : 
//            }

//            return base.Execute(pm);
//        }
        
//    }


//}
