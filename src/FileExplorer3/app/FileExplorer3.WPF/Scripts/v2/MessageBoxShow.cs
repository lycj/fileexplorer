using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        /// <summary>
        /// Serializable Uses MessageBox.Show() to show a messagebox and store result to DestinationKey as string.
        /// </summary>
        /// <param name="buttons"></param>
        /// <param name="captionVariable"></param>
        /// <param name="contentVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand MessageBoxShow(string buttons = "OK", string captionVariable = "Header", 
            string contentVariable = "Content", string destinationVariable = "{Destination}", IScriptCommand nextCommand = null)
        {
            return new MessageBoxShow()
            {
                Buttons = buttons,
                CaptionKey = captionVariable,
                ContentKey = contentVariable,
                DestinationKey = destinationVariable,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand MessageBoxOK(string captionVariable = "Header", 
            string contentVariable = "Content", IScriptCommand nextCommand = null)
        {
            return MessageBoxShow("OK", captionVariable, contentVariable, "{MessageBoxOK}", nextCommand);
        }

        public static IScriptCommand MessageBoxOKCancel(string captionVariable = "Header",
            string contentVariable = "Content", IScriptCommand okCommand = null, IScriptCommand cancelCommand = null)
        {
            string destinationVariable = "{MessageBoxResult}";
            return MessageBoxShow("OKCancel", captionVariable, contentVariable, destinationVariable,
                ScriptCommands.IfEquals(destinationVariable, "OK", okCommand, cancelCommand));
        }

        public static IScriptCommand MessageBoxYesNoCancel(string captionVariable = "Header",
            string contentVariable = "Content", 
            IScriptCommand yesCommand = null, IScriptCommand noCommand = null, IScriptCommand cancelCommand = null)
        {
            string destinationVariable = "{MessageBoxResult}";
            return MessageBoxShow("YesNoCancel", captionVariable, contentVariable, destinationVariable,
                ScriptCommands.IfEquals(destinationVariable, "Yes", yesCommand, 
                  ScriptCommands.IfEquals(destinationVariable, "No", noCommand, cancelCommand)));
        }

        public static IScriptCommand MessageBoxYesNo(string captionVariable = "Header",
            string contentVariable = "Content", 
            IScriptCommand yesCommand = null, IScriptCommand noCommand = null)
        {
            string destinationVariable = "{MessageBoxResult}";
            return MessageBoxShow("YesNo", captionVariable, contentVariable, destinationVariable,
                ScriptCommands.IfEquals(destinationVariable, "Yes", yesCommand, noCommand));
        }
    }

    /// <summary>
    /// Uses MessageBox.Show() to show a messagebox and store result to DestinationKey as string.
    /// </summary>
    public class MessageBoxShow : ScriptCommandBase
    {
        /// <summary>
        /// MessageBox buttons to show (OK, OKCancel, YesNoCancel and YesNo), Default = OK
        /// </summary>
        public string Buttons { get; set; }

        /// <summary>
        /// Header of MessageBox, Default = "Header" 
        /// </summary>
        public string CaptionKey { get; set; }

        /// <summary>
        /// Content of MessageBox, Default = "Content"
        /// </summary>
        public string ContentKey { get; set; }

        /// <summary>
        /// Store result (string) to, Default = "{Destination}".
        /// </summary>
        public string DestinationKey { get; set; }

        public MessageBoxShow()
            : base("MessageBoxShow")
        {
            Buttons = "OK";
            CaptionKey = "Header";
            ContentKey = "Content";
            DestinationKey = "{Destination}";
        }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<MessageBoxShow>();

        public override IScriptCommand Execute(ParameterDic pm)
        {
            MessageBoxButton buttons;
            if (!Enum.TryParse<MessageBoxButton>(Buttons, out buttons))
                buttons = MessageBoxButton.OK;

            logger.Debug(String.Format("Showing {0} {1} {2}", buttons, CaptionKey, ContentKey));
            string content = pm.ReplaceVariableInsideBracketed(ContentKey);
            string caption = pm.ReplaceVariableInsideBracketed(CaptionKey);

            var result = MessageBox.Show(content, caption, buttons);
            pm.SetValue(DestinationKey, result.ToString());

            return NextCommand;
        }

    }
}
