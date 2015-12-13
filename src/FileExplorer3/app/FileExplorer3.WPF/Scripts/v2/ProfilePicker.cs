using Caliburn.Micro;
using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Script
{
    public static partial class UIScriptCommands
    {
        /// <summary>
        /// Serializable, Pick a Profile using UI from the list.
        /// </summary>
        /// <param name="profileVariable"></param>
        /// <param name="destinationVariable"></param>
        /// <param name="windowManagerVariable"></param>
        /// <param name="successCommand"></param>
        /// <param name="cancelCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ProfilePicker(
            string profileVariable = "{Profiles}", string destinationVariable = "{Destination}",
            string windowManagerVariable = "{WindowManager}", 
            IScriptCommand successCommand = null, IScriptCommand cancelCommand = null)
        {
            
            return new ProfilePicker()
            {
                ProfilesKey = profileVariable,
                DestinationKey = destinationVariable,
                WindowManagerKey = windowManagerVariable,
                NextCommand = (ScriptCommandBase)successCommand,
                CancelCommand = (ScriptCommandBase)cancelCommand
            };
        }
    }

    public class ProfilePicker : ScriptCommandBase
    {
        /// <summary>
        /// WindowManager used to show the window, optional, Default={WindowManager}
        /// </summary>
        public string WindowManagerKey { get; set; }

        /// <summary>
        /// A list of profiles (IProfile[]) to pick (via SelectProfileViewModel), Default = {Profiles}, 
        /// If one Profile only in array, no dialog will be shown.
        /// </summary>
        public string ProfilesKey { get; set; }

        /// <summary>
        /// Store selected profile (IProfile) to destination Key when success, and run NextCommand.
        /// </summary>
        public string DestinationKey { get; set; }

        /// <summary>
        /// Run if user cancelled.
        /// </summary>
        public ScriptCommandBase CancelCommand { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ProfilePicker>();

        public ProfilePicker()
            : base("ProfilePicker")
        {
            WindowManagerKey = "{WindowManager}";
            ProfilesKey = "{Profiles}";
            DestinationKey = "{Destination}";
            NextCommand = CancelCommand = ResultCommand.NoError;
            ContinueOnCaptureContext = true;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            IProfile[] rootProfiles = pm.GetValue<IProfile[]>(ProfilesKey);
            if (rootProfiles == null || rootProfiles.Length == 0)
                return ResultCommand.Error(new ArgumentException(ProfilesKey));            

            if (rootProfiles.Length == 1)
            {
                pm.SetValue(DestinationKey, rootProfiles[0]);
                return NextCommand;
            }
            else
            {
                IWindowManager wm = pm.GetValue<IWindowManager>(WindowManagerKey) ?? new WindowManager();
                logger.Debug("Showing");
                SelectProfileViewModel spvm = new SelectProfileViewModel(rootProfiles);
                if (wm.ShowDialog(spvm).Value)
                {
                    logger.Info(String.Format("Selected {0}", spvm.SelectedRootProfile));
                    pm.SetValue(DestinationKey, spvm.SelectedRootProfile);
                    return NextCommand;
                }
                else
                {
                    logger.Debug("Cancelled");
                    return CancelCommand;
                }
            }            
        }
    }
}
