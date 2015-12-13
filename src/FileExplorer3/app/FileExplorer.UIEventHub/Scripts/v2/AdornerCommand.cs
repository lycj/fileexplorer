using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {
        public static IScriptCommand AttachAdorner(string adornerLayerVariable = "{AdornerLayer}",
            string adornerVariable = "{Adorner}", IScriptCommand nextCommand = null)
        {
            return
                new AdornerCommand()
                {
                    Mode = AdornerCommandMode.Attach,
                    AdornerLayerKey = adornerLayerVariable,
                    AdornerKey = adornerVariable,
                    NextCommand = (ScriptCommandBase)nextCommand
                };
        }

        public static IScriptCommand DetachAdorner(string adornerLayerVariable = "{AdornerLayer}",
            string adornerVariable = "{Adorner}", IScriptCommand nextCommand = null)
        {
            return
                new AdornerCommand()
                {
                    Mode = AdornerCommandMode.Detach,
                    AdornerLayerKey = adornerLayerVariable,
                    AdornerKey = adornerVariable,
                    NextCommand = (ScriptCommandBase)nextCommand
                };
        }
    }

    public enum AdornerCommandMode { Attach, Detach }

    public class AdornerCommand : ScriptCommandBase
    {
        /// <summary>
        /// Point to where to store the found adorner layer. Default = {AdornerLayer}
        /// </summary>
        public string AdornerLayerKey { get; set; }

        /// <summary>
        /// Point to where to store the found adorner layer. Default = {Adorner}
        /// </summary>
        public string AdornerKey { get; set; }

        public AdornerCommandMode Mode { get; set; }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            AdornerLayer adornerLayer = pm.GetValue<AdornerLayer>(AdornerLayerKey);
            Adorner adorner = pm.GetValue<Adorner>(AdornerKey);

            if (adornerLayer != null && adorner != null)
                switch (Mode)
                {
                    case AdornerCommandMode.Attach: adornerLayer.Add(adorner); break;
                    case AdornerCommandMode.Detach: adornerLayer.Remove(adorner); break;
                }

            return NextCommand;
        }

    }
}
