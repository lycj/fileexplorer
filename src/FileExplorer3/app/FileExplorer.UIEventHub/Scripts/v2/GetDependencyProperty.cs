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
        public static IScriptCommand GetDependencyProperty(
            string elementVariable = "{Sender}", string propertyVariable = "{Property}", string destinationVariable = "{Value}",
            IScriptCommand nextCommand = null)
        {
            return new GetDependencyProperty()
            {
                ElementKey = elementVariable,
                PropertyKey = propertyVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand GetDependencyProperty(string elementVariable,
            DependencyProperty property, string destinationVariable, IScriptCommand nextCommand)
        {
            string propertyVariable = ParameterDic.CombineVariable(elementVariable, "Property");            

            return ScriptCommands.Assign(propertyVariable, property, false,
                  GetDependencyProperty(elementVariable, propertyVariable, destinationVariable, nextCommand));
        }
     
    }

    public class GetDependencyProperty : UIScriptCommandBase
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
        /// Point to the value to store, Default={Value}
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<GetDependencyProperty>();

        public GetDependencyProperty()
            : base("GetDependencyProperty")
        {
            ElementKey = "{Sender}";
            PropertyKey = "{Property}";
            DestinationKey = "{Value}";
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            FrameworkElement ele = pm.GetValue<FrameworkElement>(ElementKey);
            DependencyProperty property = pm.GetValue<DependencyProperty>(PropertyKey);

            if (ele == null)
                return ResultCommand.Error(new ArgumentException(ElementKey));
            if (property == null)
                return ResultCommand.Error(new ArgumentException(PropertyKey));

            object value = ele.GetValue(property);
            logger.Debug(String.Format("{0}'s {1} is equal to {2}", ele, property, value));
            pm.SetValue(DestinationKey, value);
            
            return NextCommand;
        }
    }


   
}
