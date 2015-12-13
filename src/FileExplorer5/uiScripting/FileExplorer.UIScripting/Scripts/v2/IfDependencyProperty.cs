using FileExplorer.Defines;
using FileExplorer.Script;
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

        public static IScriptCommand IfDependencyProperty<T>(string elementVariable = "{Sender}", 
            DependencyProperty property = null, ComparsionOperator op = ComparsionOperator.Equals, T value = default(T), 
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string destinationVariable = ParameterDic.CombineVariable(elementVariable, property.ToString() + "Value");

            return HubScriptCommands.GetDependencyProperty(elementVariable, property, destinationVariable,
                ScriptCommands.IfValue(op, destinationVariable, value, trueCommand, otherwiseCommand));
        }

        public static IScriptCommand IfDependencyPropertyEquals<T>(string elementVariable = "{Sender}", 
            DependencyProperty property = null, T value = default(T), 
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {            
            return IfDependencyProperty<T>(elementVariable, property, ComparsionOperator.Equals, value, trueCommand, otherwiseCommand);
        }
        
        public static IScriptCommand IfDependencyPropertyEqualDefaultValue<T>(string elementVariable = "{Sender}", 
            DependencyProperty property = null,  
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            return IfDependencyPropertyEquals<T>(elementVariable, property,
                (T)property.DefaultMetadata.DefaultValue, trueCommand, otherwiseCommand);
        }
        
    }

   
   
}
