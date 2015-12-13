using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand FindUIElement(
            string elementVariable = "{Sender}",
            FindDirectionType direction = FindDirectionType.Parent,
            FindTreeType tree = FindTreeType.Visual,
            FindMethodType method = FindMethodType.Name,
            string findParameterVariable = "{Parameter}",
            string destinationVariable = "{Destination}",
            IScriptCommand nextCommand = null)
        {
            return new FindUIElement()
            {
                SenderKey = elementVariable,
                FindDirection = direction,
                FindTree = tree,
                FindMethod = method,
                FindParameterKey = findParameterVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand FindVisualChild(string elementVariable = "{Sender}",
            FindMethodType method = FindMethodType.Name,
            string findParameterVariable = "{Parameter}",
            string destinationVariable = "{Destination}",
            IScriptCommand nextCommand = null)
        {
            return FindUIElement(elementVariable, FindDirectionType.Child, FindTreeType.Visual, method, findParameterVariable, destinationVariable, nextCommand);
        }

        public static IScriptCommand IfExistsVisualChild(string elementVariable = "{Sender}",
            FindMethodType method = FindMethodType.Name,
            string findparameterVariable = "{Parameter}",
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string destinationVariable = ParameterDic.CombineVariable(elementVariable, "Destination");
            return FindVisualChild(elementVariable, method, findparameterVariable, destinationVariable,
                ScriptCommands.IfAssigned(destinationVariable, trueCommand, otherwiseCommand));
        }

        public static IScriptCommand FindVisualParent(string elementVariable = "{Sender}",
           FindMethodType method = FindMethodType.Name,
           string findParameterVariable = "{Parameter}",
           string destinationVariable = "{Destination}",
           IScriptCommand nextCommand = null)
        {
            return FindUIElement(elementVariable, FindDirectionType.Parent, FindTreeType.Visual, method, findParameterVariable, destinationVariable, nextCommand);
        }

        public static IScriptCommand IfExistsVisualParent(string elementVariable = "{Sender}",
            FindMethodType method = FindMethodType.Name,
            string findparameterVariable = "{Parameter}",
            IScriptCommand trueCommand = null, IScriptCommand otherwiseCommand = null)
        {
            string destinationVariable = ParameterDic.CombineVariable(elementVariable, "Destination");
            return FindVisualParent(elementVariable, method, findparameterVariable, destinationVariable,
                ScriptCommands.IfAssigned(destinationVariable, trueCommand, otherwiseCommand));
        }

        public static IScriptCommand FindLogicalChild(string elementVariable = "{Sender}",
            FindMethodType method = FindMethodType.Name,
            string findParameterVariable = "{Parameter}",
            string destinationVariable = "{Destination}",
            IScriptCommand nextCommand = null)
        {
            return FindUIElement(elementVariable, FindDirectionType.Child, FindTreeType.Logical, method, findParameterVariable, destinationVariable, nextCommand);
        }

        public static IScriptCommand FindLogicalParent(string elementVariable = "{Sender}",
           FindMethodType method = FindMethodType.Name,
           string findParameterVariable = "{Parameter}",
           string destinationVariable = "{Destination}",
           IScriptCommand nextCommand = null)
        {
            return FindUIElement(elementVariable, FindDirectionType.Parent, FindTreeType.Logical, method, findParameterVariable, destinationVariable, nextCommand);
        }
    }

    public enum FindMethodType { Name, Type, Level }
    public enum FindTreeType { Visual, Logical }
    public enum FindDirectionType { Parent, Child }

    public class FindUIElement : UIScriptCommandBase
    {
        public FindDirectionType FindDirection { get; set; }

        public FindTreeType FindTree { get; set; }

        public FindMethodType FindMethod { get; set; }

        /// <summary>
        /// Point to ElementName (string) if FindByMode equals Name, 
        /// Point to ElementType (string) if FindByMode equals Type, 
        /// Point to Level (int) if FindByMode equals Level.
        /// Default = "{Parameter}"
        /// </summary>
        public string FindParameterKey { get; set; }

        /// <summary>
        /// Location to assign output, Default = {Destination}
        /// </summary>
        public string DestinationKey { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<FindUIElement>();

        public FindUIElement()
            : base("FindUIElement")
        {
            FindDirection = FindDirectionType.Parent;
            FindTree = FindTreeType.Visual;
            FindMethod = FindMethodType.Name;
            FindParameterKey = "{Parameter}";
            DestinationKey = "{Destination}";
        }


        private FrameworkElement findVisualAncestor(ParameterDic pm, FrameworkElement sender)
        {
            switch (FindMethod)
            {
                case UIEventHub.FindMethodType.Name:
                    string name = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindAncestor<FrameworkElement>(sender, ele => name.Equals(ele.Name));
                case UIEventHub.FindMethodType.Type:
                    string type = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindAncestor<FrameworkElement>(sender, ele => ele.GetType().Name.Equals(type, StringComparison.CurrentCultureIgnoreCase));
                case UIEventHub.FindMethodType.Level:
                    int level = pm.GetValue<int>(FindParameterKey, -1);
                    if (level == -1)
                        return null;
                    FrameworkElement current = sender;
                    for (int i = 0; i < level; i++)
                        if (current != null)
                            current = VisualTreeHelper.GetParent(current) as FrameworkElement;
                    return current;
                default: throw new NotSupportedException(FindMethod.ToString());
            }
        }

        private FrameworkElement findLogicalAncestor(ParameterDic pm, FrameworkElement sender)
        {
            switch (FindMethod)
            {
                case UIEventHub.FindMethodType.Name:
                    string name = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindLogicalAncestor<FrameworkElement>(sender, ele => name.Equals(ele.Name));
                case UIEventHub.FindMethodType.Type:
                    string type = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindLogicalAncestor<FrameworkElement>(sender, ele => ele.GetType().Name.Equals(type, StringComparison.CurrentCultureIgnoreCase));
                case UIEventHub.FindMethodType.Level:
                    int level = pm.GetValue<int>(FindParameterKey, -1);
                    if (level == -1)
                        return null;
                    FrameworkElement current = sender;
                    for (int i = 0; i < level; i++)
                        if (current != null)
                            current = LogicalTreeHelper.GetParent(current) as FrameworkElement;
                    return current;
                default: throw new NotSupportedException(FindMethod.ToString());
            }
        }
        private FrameworkElement findVisualChild(ParameterDic pm, FrameworkElement sender)
        {
            switch (FindMethod)
            {
                case UIEventHub.FindMethodType.Name:
                    string name = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindVisualChild<FrameworkElement>(sender, ele => name.Equals(ele.Name));
                case UIEventHub.FindMethodType.Type:
                    string type = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindVisualChild<FrameworkElement>(sender, ele => ele.GetType().Name.Equals(type, StringComparison.CurrentCultureIgnoreCase));
                default: throw new NotSupportedException(FindMethod.ToString());
            }
        }

        private FrameworkElement findLogicalChild(ParameterDic pm, FrameworkElement sender)
        {
            switch (FindMethod)
            {
                case UIEventHub.FindMethodType.Name:
                    string name = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindLogicalChild<FrameworkElement>(sender, ele => name.Equals(ele.Name));
                case UIEventHub.FindMethodType.Type:
                    string type = pm.ReplaceVariableInsideBracketed(FindParameterKey) ?? "";
                    return UITools.FindLogicalChild<FrameworkElement>(sender, ele => ele.GetType().Name.Equals(type, StringComparison.CurrentCultureIgnoreCase));
                default: throw new NotSupportedException(FindMethod.ToString());
            }
        }


        private FrameworkElement findAncestor(ParameterDic pm, FrameworkElement sender)
        {
            if (FindTree == FindTreeType.Visual)
                return findVisualAncestor(pm, sender);
            return findLogicalAncestor(pm, sender);
        }

        private FrameworkElement findChild(ParameterDic pm, FrameworkElement sender)
        {
            if (FindTree == FindTreeType.Visual)
                return findVisualChild(pm, sender);
            return findLogicalChild(pm, sender);
        }

        public override Script.IScriptCommand Execute(ParameterDic pm)
        {
            FrameworkElement sender = pm.GetValue<FrameworkElement>(SenderKey);

            FrameworkElement ele = (FindDirection == FindDirectionType.Parent) ?
                findAncestor(pm, sender) : findChild(pm, sender);

            pm.SetValue(DestinationKey, ele, false);

            return NextCommand;
        }
    }
}
