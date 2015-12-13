using FileExplorer.Defines;
using FileExplorer.Models;
using MetroLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace FileExplorer.Script
{
    public static partial class CoreScriptCommands
    {        
        public static IScriptCommand NotifyEntryChanged(
            ChangeType changeType, string sourceProfileKey, string sourceEntryKey,
         string destinationProfileKey, string destinationEntryKey, IScriptCommand nextCommand = null)
        {
            return new NotifyEntryChanged()
            {
                ChangeType = changeType,
                SourceProfileKey = sourceProfileKey,
                SourceEntryKey = sourceEntryKey,
                DestinationProfileKey = destinationProfileKey,
                DestinationEntryKey = destinationEntryKey,
                NextCommand = (ScriptCommandBase)nextCommand
            };
        }

        public static IScriptCommand NotifyEntryChanged(
           ChangeType changeType, string profileKey, string entryKey, IScriptCommand nextCommand = null)
        {
            return NotifyEntryChanged(changeType, null, null, profileKey, entryKey, nextCommand);
        }

        #region Profile, EntryKey

        public static IScriptCommand NotifyEntryChangedProfile(
          ChangeType changeType, IProfile sourceProfile, string sourceEntryKey,
          IProfile destinationProfile, string destinationEntryKey, IScriptCommand nextCommand = null)
        {
            string sourceProfileKey = "{SourceProfileKey}";
            string destinationProfileKey = "{DestinationProfileKey}";
            return ScriptCommands.Assign(sourceProfileKey, sourceProfile, false,
                ScriptCommands.Assign(destinationProfileKey, destinationProfile, false,
                NotifyEntryChanged(changeType, sourceProfileKey, sourceEntryKey,
                destinationProfileKey, destinationEntryKey, nextCommand)));
        }
              
        public static IScriptCommand NotifyEntryChangedProfile(
            ChangeType changeType, IProfile profile, string entryKey, IScriptCommand nextCommand = null)
        {
            return NotifyEntryChangedProfile(changeType, null, null, profile, entryKey, nextCommand);
        }

        #endregion

        #region Profile, EntryPaths
        public static IScriptCommand NotifyEntryChangedPath(
          ChangeType changeType, IProfile sourceProfile, string[] sourceEntryPaths,
          IProfile destinationProfile, string[] destinationEntryPaths, IScriptCommand nextCommand = null)
        {
            string sourceEntriesKey = "{SourceEntriesKey}";
            string destinationEntriesKey = "{DestinationEntriesKey}";
            return ScriptCommands.Assign(sourceEntriesKey, sourceEntryPaths, false,
                ScriptCommands.Assign(destinationEntriesKey, destinationEntryPaths, false,
                NotifyEntryChangedProfile(changeType, sourceProfile, sourceEntriesKey,
                destinationProfile, destinationEntriesKey, nextCommand)));
        }

        public static IScriptCommand NotifyEntryChangedPath(
             ChangeType changeType, IProfile profile, string[] entryPaths, IScriptCommand nextCommand = null)
        {
            return NotifyEntryChangedPath(changeType, null, new string[] {}, profile, entryPaths, nextCommand);
        }
        #endregion

        #region Entry

        public static IScriptCommand NotifyEntryChanged(
            ChangeType changeType, IEntryModel[] entries, IScriptCommand nextCommand = null)
        {
            return NotifyEntryChangedPath(changeType, null, null, entries.First().Profile, 
                entries.Select(e => e.FullPath).ToArray(), nextCommand);
        }

        public static IScriptCommand NotifyEntryChanged(
            ChangeType changeType, IEntryModel entry, IScriptCommand nextCommand = null)
        {
            return NotifyEntryChangedPath(changeType, null, null, entry.Profile, new string[] { entry.FullPath }, nextCommand);
        }
        #endregion



       

    }

    public class NotifyEntryChanged : ScriptCommandBase
    {
        /// <summary>
        /// Profile used report source changed, default = null.
        /// </summary>
        public string SourceProfileKey { get; set; }

        /// <summary>
        /// Can be either a path or path array (prase from sourceProfile) or IEntryModel, default = null.
        /// </summary>
        public string SourceEntryKey { get; set; }

        /// <summary>
        /// Profile used report destination changed, default = "Profile".
        /// </summary>
        public string DestinationProfileKey { get; set; }

        /// <summary>
        /// Can be either a path or path array (prase from destProfile) or IEntryModel, default = "Entry".
        /// </summary>
        public string DestinationEntryKey { get; set; }

        /// <summary>
        /// Change type (Changed, Created, Deleted, Moved) , default = Changed.
        /// </summary>
        public ChangeType ChangeType { get; set; }

        private static ILogger logger = LogManagerFactory.DefaultLogManager.GetLogger<NotifyEntryChanged>();

        public NotifyEntryChanged()
            : base("NotifyChanged")
        {
            SourceProfileKey = null;
            SourceEntryKey = null;
            DestinationProfileKey = "Profile";
            DestinationEntryKey = "Entry";
            ChangeType = Defines.ChangeType.Changed;
        }

        private bool parseEntryAndProfile(ParameterDic pm, string key, string profileKey,
            out string[] entryPath, out IProfile profile)
        {
            object value = pm.GetValue(key);

            if (value is string)
                value = new string[] { value as string };
            if (value is string[])
            {
                entryPath = value as string[];
                profile = pm.GetValue<IProfile>(profileKey);
                return true;
            }

            if (value is IEntryModel)
                value = new IEntryModel[] { value as IEntryModel };

            if (value is IEntryModel[])
            {
                IEntryModel[] ems = value as IEntryModel[];
                entryPath = ems.Select(em => em.FullPath).ToArray();
                profile = ems.First().Profile;
                return true;
            }

            entryPath = null;
            profile = null;
            return false;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            string[] sourcePaths, destinationPaths;
            IProfile sourceProfile, destinationProfile;

            parseEntryAndProfile(pm, SourceEntryKey, SourceProfileKey, out sourcePaths, out sourceProfile);


            if (!parseEntryAndProfile(pm, DestinationEntryKey, DestinationProfileKey, out destinationPaths, out destinationProfile))
            {
                logger.Error(String.Format("Failed to parse {0} or {1}", DestinationEntryKey, DestinationProfileKey));
                return NextCommand;
            }


            logger.Info(String.Format("({0}) {1} -> {2}", ChangeType, sourcePaths, destinationPaths));            
            if (ChangeType == ChangeType.Moved && sourceProfile != null && destinationProfile != null && sourcePaths != null)
            {
                if (sourceProfile != destinationProfile)
                {
                    sourceProfile.Events.PublishOnCurrentThread(new EntryChangedEvent(ChangeType.Deleted, sourcePaths));
                    destinationProfile.Events.PublishOnCurrentThread(new EntryChangedEvent(ChangeType.Created, destinationPaths));
                }
                else
                    destinationProfile.Events.PublishOnCurrentThread(new EntryChangedEvent(destinationPaths.FirstOrDefault(), sourcePaths.FirstOrDefault()));
            }
            else
                destinationProfile.Events.PublishOnCurrentThread(new EntryChangedEvent(ChangeType, destinationPaths));

            return NextCommand;
        }

        //public override bool Equals(object obj)
        //{
        //    return obj is NotifyChangedCommand && (obj as NotifyChangedCommand)._destProfile.Equals(_destProfile) &&
        //        (obj as NotifyChangedCommand)._destParseName.Equals(_destParseName);
        //}

        //public override int GetHashCode()
        //{
        //    return _destParseName.GetHashCode() + _changeType.GetHashCode();
        //}
    }
}
