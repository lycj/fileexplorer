using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.WPF.Utils;
using FileExplorer.Defines;
using FileExplorer.Models;
using System.ComponentModel;
using FileExplorer.IO;
using MetroLog;
using Caliburn.Micro;

namespace FileExplorer.Script
{
    public static partial class CoreScriptCommands
    {


        ///// <summary>
        ///// Serializable, Parse a path, requires Profile set to an IDiskProfile or IDiskProfile[].
        ///// </summary>
        ///// <param name="pathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        ///// <param name="destVariable"></param>
        ///// <param name="foundCommand"></param>
        ///// <param name="notFoundCommand"></param>
        ///// <returns></returns>
        //public static IScriptCommand ParsePathFromProfiles(string profilesVariable = "{Profiles}", string pathVariable = "{Path}", string destVariable = "{Entry}",
        //    IScriptCommand foundCommand = null, IScriptCommand notFoundCommand = null)
        //{
        //    return new ParsePath()
        //    {
        //        ProfileKey = null,
        //        ProfilesKey = profilesVariable,
        //        PathKey = pathVariable,
        //        DestinationKey = destVariable,
        //        NextCommand = (ScriptCommandBase)foundCommand,
        //        NotFoundCommand = (ScriptCommandBase)notFoundCommand
        //    };
        //}

        /// <summary>
        /// Serializable, Parse a path, requires Profile set to an IDiskProfile or IDiskProfile[].
        /// </summary>
        /// <param name="pathOrPathVariable">Actual path or reference variable (if Bracketed), e.g. C:\Temp or {Path}.</param>
        /// <param name="destVariable"></param>
        /// <param name="foundCommand"></param>
        /// <param name="notFoundCommand"></param>
        /// <returns></returns>
        public static IScriptCommand ParsePath(string profileVariable = "{Profile}", string pathOrPathVariable = "{Path}", string destVariable = "{Entry}",
            IScriptCommand foundCommand = null, IScriptCommand notFoundCommand = null)
        {
            return new ParsePath()
            {
                ProfileKey = profileVariable,
                PathKey = pathOrPathVariable,
                DestinationKey = destVariable,
                NextCommand = (ScriptCommandBase)foundCommand,
                NotFoundCommand = (ScriptCommandBase)notFoundCommand
            };
        }
    }


    /// <summary>
    /// Serializable, Parse path to "Entry" parameter, using IProfile[] from ParameterDic[ProfileKey].
    /// </summary>
    public class ParsePath : ScriptCommandBase
    {
        /// <summary>
        /// Profile to parse path (IProfile), default = Profile
        /// </summary>
        public string ProfileKey { get; set; }

        ///// <summary>
        ///// Profile to parse path (IProfile[]), default = Profiles
        ///// </summary>
        //public string ProfilesKey { get; set; }

        /// <summary>
        /// Required, path to parse, default = {Path} or C:\\Temp
        /// </summary>
        public string PathKey { get; set; }

        /// <summary>
        /// Where to store after parse completed, default = Entry
        /// </summary>
        public string DestinationKey { get; set; }

        /// <summary>
        /// If the path is not found, command to execute.  Optional, return ResultCommand.Error if not specified.
        /// </summary>
        public ScriptCommandBase NotFoundCommand { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<ParsePath>();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ParsePath() : base("ParsePath") { ProfileKey = "{Profile}"; PathKey = "{Path}"; DestinationKey = "{Entry}"; }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            string path = pm.ReplaceVariableInsideBracketed(PathKey);

            IProfile profile = pm.GetValue<IProfile>(ProfileKey, null);
            IProfile[] profiles = profile != null ? new IProfile[] { profile } :
                pm.GetValue<IProfile[]>(ProfileKey, null);
            if (profiles == null)
                return ResultCommand.Error(new ArgumentException("Profile/s not specified."));

            pm.SetValue<IEntryModel>(DestinationKey, null);
            foreach (var p in profiles)
                if (p.MatchPathPattern(path))
                {

                    var retVal = await p.ParseAsync(path);
                    if (retVal != null)
                    {
                        logger.Debug(String.Format("{0} = {1}", DestinationKey, retVal));
                        pm.SetValue<IEntryModel>(DestinationKey, retVal);
                        break;
                    }
                }

            if (pm.GetValue(DestinationKey) == null)
            {
                logger.Warn(String.Format("{0} = null", path));
                return NotFoundCommand ?? ResultCommand.Error(new FileNotFoundException(path + " is not found."));
            }

            return NextCommand;
        }

    }



}
