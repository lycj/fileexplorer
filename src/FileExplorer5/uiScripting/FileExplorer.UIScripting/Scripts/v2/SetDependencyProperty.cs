using FileExplorer.Defines;
using FileExplorer.Script;
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
        public static IScriptCommand SetDependencyProperty(
            string elementVariable = "{Sender}", string propertyVariable = "{Property}", string valueVariable = "{Value}",
            IScriptCommand nextCommand = null)
        {
            return new SetDependencyProperty()
            {
                ElementKey = elementVariable,
                PropertyKey = propertyVariable,
                ValueKey = valueVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand SetDependencyPropertyValue<T>(string elementVariable,
            DependencyProperty property, T value, IScriptCommand nextCommand = null)
        {
            string propertyVariable = ParameterDic.CombineVariable(elementVariable, "Property");
            string valueVariable = ParameterDic.CombineVariable(elementVariable, "Value");

            return ScriptCommands.Assign(propertyVariable, property, false,
                ScriptCommands.Assign(valueVariable, value, false,
                  SetDependencyProperty(elementVariable, propertyVariable, valueVariable, nextCommand)));
        }

        public static IScriptCommand SetDependencyProperty(string elementVariable,
            DependencyProperty property, string valueVariable, IScriptCommand nextCommand = null)
        {
            string propertyVariable = ParameterDic.CombineVariable(elementVariable, "Property");            

            return ScriptCommands.Assign(propertyVariable, property, false,                
                  SetDependencyProperty(elementVariable, propertyVariable, valueVariable, nextCommand));
        }

        /// <summary>
        /// Get the property and compare if it's equal to value before setting.
        /// </summary>
        /// <param name="elementVariable"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="ifChanged"></param>
        /// <param name="ifUnchanged"></param>
        /// <returns></returns>
        public static IScriptCommand SetDependencyPropertyIfDifferentValue(string elementVariable,
            DependencyProperty property, object value, IScriptCommand ifChanged, IScriptCommand ifUnchanged = null)
        {                       
            string valueVariable = ParameterDic.CombineVariable(elementVariable, "Value");
            return HubScriptCommands.GetDependencyProperty(elementVariable, property, valueVariable,
                     ScriptCommands.IfEquals(valueVariable, value,
                        ifUnchanged,
                        SetDependencyPropertyValue(elementVariable, property, value, ifChanged)));
        }
    }

    public class SetDependencyProperty : UIScriptCommandBase
    {
        /// <summary>
        /// Element to obtain the DependencyProperty, Default={Sender}
        /// </summary>
        public string ElementKey { get; set; }

        /// <summary>
        /// Point to DependencyProperty to get to, Default = {Property}
        /// </summary>
        public string PropertyKey { get; set; }

        /// <summary>
        /// Point to the value to obtain, Default={Value}
        /// </summary>
        public string ValueKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<SetDependencyProperty>();

        public SetDependencyProperty()
            : base("SetDependencyProperty")
        {
            ElementKey = "{Sender}";
            PropertyKey = "{Property}";
            ValueKey = "{Value}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {            
            FrameworkElement ele = pm.GetValue<FrameworkElement>(ElementKey);
            DependencyProperty property = pm.GetValue<DependencyProperty>(PropertyKey);

            if (ele == null)
                return ResultCommand.Error(new ArgumentException(ElementKey));
            if (property == null)
                return ResultCommand.Error(new ArgumentException(PropertyKey));

            object value = pm.GetValue(ValueKey);
            logger.Debug(String.Format("Set {0}'s {1} to {2}", ele, property, value));
            ele.SetValue(property, value);

            return NextCommand;
        }
    }



}
