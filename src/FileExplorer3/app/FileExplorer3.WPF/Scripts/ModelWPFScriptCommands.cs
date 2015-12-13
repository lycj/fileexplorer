using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer;
using FileExplorer.Script;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.Models;
using FileExplorer.IO;
using System.Threading;
using System.IO;
using FileExplorer.WPF.Utils;

namespace FileExplorer.Script
{
    public static partial class WPFScriptCommands
    {
        [Obsolete("CoreScriptCommands.ParsePathFromProfiles")]
        public static IScriptCommand ParsePath(IProfile[] profiles, string path, Func<IEntryModel, IScriptCommand> ifFoundFunc,
            IScriptCommand ifNotFound)
        {
            return new ParsePathCommand(profiles, path, ifFoundFunc, ifNotFound);
        }

        [Obsolete("CoreScriptCommands.DiskCreatePath")]
        public static IScriptCommand CreatePath(IProfile profile, string path, bool isFolder, bool renameIfExists,
            Func<IEntryModel, IScriptCommand> thenFunc)
        {
            if (profile is IDiskProfile)
            {
                string parentPath = profile.Path.GetDirectoryName(path);
                string name = profile.Path.GetFileName(path);

                return new DiskCreateCommand(profile as IDiskProfile,
                    parentPath,
                    renameIfExists ? FileNameGenerator.Rename(name) : FileNameGenerator.NoRename(name),
                    isFolder, thenFunc);
            }
            return ResultCommand.Error(new NotSupportedException("Profile is not IDiskProfile."));
        }

        public static Func<IEntryModel, bool> FileOnlyFilter = em => !em.IsDirectory;
        public static Func<IEntryModel, bool> DirectoryOnlyFilter = em => em.IsDirectory;

        /// <summary>
        /// List contents (to Result:List of IEntryModel) of the specified Directory.
        /// Parameter - Directory, Recrusive, Refresh, Mask
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        [Obsolete("CoreScriptCommands.List")]
        public static IScriptCommand List(IEntryModel[] directories, Func<IEntryModel, bool> filter = null, Func<IEntryModel, bool> lookupFilter = null,
            bool recrusive = false, Func<IEntryModel[], IScriptCommand> nextCommandFunc = null)
        {
            return new ListDirectoryCommand(directories, filter, lookupFilter, recrusive, nextCommandFunc);
        }

        [Obsolete("CoreScriptCommands.List")]
        public static IScriptCommand List(IEntryModel directory, Func<IEntryModel, bool> filter = null, Func<IEntryModel, bool> lookupFilter = null,
           bool recrusive = false, Func<IEntryModel[], IScriptCommand> nextCommandFunc = null)
        {
            return List(new IEntryModel[] { directory }, filter, lookupFilter, recrusive, nextCommandFunc);
        }

        //public static IScriptCommand List(IEntryModel directory, Func<IEntryModel, bool> filter = null,
        //    bool recrusive = false, Func<IEntryModel[], IScriptCommand> nextCommandFunc = null)
        //{
        //    return new ListDirectoryCommand(directory, DirectoryOnlyFilter, filter, recrusive, nextCommandFunc);
        //}


        public static IScriptCommand CreatePath(IEntryModel parentModel, string name, bool isFolder,
            bool renameIfExists,
            Func<IEntryModel, IScriptCommand> thenFunc)
        {
            IProfile profile = parentModel.Profile;
            return CreatePath(profile, profile.Path.Combine(parentModel.FullPath, name), isFolder, renameIfExists, thenFunc);
        }

        public static IScriptCommand ParseOrCreatePath(IDiskProfile profile, string path,
            bool isFolder, Func<IEntryModel, IScriptCommand> thenFunc)
        {
            return WPFScriptCommands.ParsePath(new[] { profile }, path, thenFunc,
                WPFScriptCommands.CreatePath(profile, path, isFolder, false, thenFunc));
        }

        public static IScriptCommand OpenFileStream(IEntryModel entryModel, FileExplorer.Defines.FileAccess access,
            Func<IEntryModel, Stream, IScriptCommand> streamFunc, Func<IEntryModel, IScriptCommand> thenCommandFunc)
        {
            return new OpenStreamCommand(entryModel, access, streamFunc, thenCommandFunc(entryModel));
        }

        public static IScriptCommand WriteBytes(IEntryModel entryModel, byte[] bytes,
            Func<IEntryModel, IScriptCommand> nextCommandFunc)
        {
            return OpenFileStream(entryModel, FileExplorer.Defines.FileAccess.Write,
                (em, s) => new SimpleScriptCommand("WriteBytes", pd =>
                    {
                        s.Write(bytes, 0, bytes.Length);
                        return ResultCommand.NoError;
                    }), nextCommandFunc);

        }
    }
}

namespace FileExplorer.WPF.Models
{
    public class ParsePathCommand : ScriptCommandBase
    {
        private IProfile[] _profiles;
        private string _path;
        private Func<IEntryModel, IScriptCommand> _ifFoundFunc;
        private IScriptCommand _ifNotFound;

        public ParsePathCommand(IProfile[] profiles, string path, Func<IEntryModel, IScriptCommand> ifFoundFunc,
                IScriptCommand ifNotFound)
            : base("ParsePath")
        {
            _profiles = profiles;
            _path = path;
            _ifFoundFunc = ifFoundFunc;
            _ifNotFound = ifNotFound;
        }


        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            foreach (var p in _profiles)
            {
                var result = await p.ParseAsync(_path);
                if (result != null)
                    return _ifFoundFunc(result);
            }
            return _ifNotFound;
        }
    }

    /// <summary>
    /// Open a file (which profile is IDiskProfile), the stream is closed when streamFunc is finished.
    /// </summary>
    public class OpenStreamCommand : ScriptCommandBase
    {
        private IEntryModel _entryModel;
        private FileExplorer.Defines.FileAccess _access;
        private Func<IEntryModel, Stream, IScriptCommand> _streamFunc;
        public OpenStreamCommand(IEntryModel entryModel, FileExplorer.Defines.FileAccess access,
            Func<IEntryModel, Stream, IScriptCommand> streamFunc, IScriptCommand thenCommand)
            : base("OpenStream", thenCommand)
        {
            if (!(entryModel.Profile is IDiskProfile))
                throw new ArgumentException("Profile isnt IDiskProfile.");
            _entryModel = entryModel;
            _access = access;
            _streamFunc = streamFunc;
        }

        public override IScriptCommand Execute(ParameterDic pm)
        {
            IDiskProfile profile = _entryModel.Profile as IDiskProfile;
            using (var stream = profile.DiskIO.OpenStreamAsync(_entryModel, _access, pm.CancellationToken).Result)
                ScriptRunner.RunScript(pm, _streamFunc(_entryModel, stream));                
            return _nextCommand;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            IDiskProfile profile = _entryModel.Profile as IDiskProfile;
            using (var stream = await profile.DiskIO.OpenStreamAsync(_entryModel, _access, pm.CancellationToken))
                await ScriptRunner.RunScriptAsync(pm, _streamFunc(_entryModel, stream));
            return _nextCommand;
        }
    }

    public class ListDirectoryCommand : ScriptCommandBase
    {
        Func<IEntryModel, bool> _filter = em => true;
        bool _recrusive = false;
        IEntryModel[] _directories = null;
        private Func<IEntryModel[], IScriptCommand> _nextCommandFunc;
        private Func<IEntryModel, bool> _lookupFilter;

        public ListDirectoryCommand(IEntryModel[] directories,
            Func<IEntryModel, bool> filter = null, Func<IEntryModel, bool> lookupFilter = null,
            bool recrusive = false, Func<IEntryModel[], IScriptCommand> nextCommandFunc = null)
            : base("List", "Directory", "Mask", "Refresh")
        {
            _directories = directories;
            _lookupFilter = lookupFilter;
            _filter = filter ?? (em => true);
            _recrusive = recrusive;
            _nextCommandFunc = nextCommandFunc ?? (ems => ResultCommand.NoError);
        }

        public ListDirectoryCommand(IEntryModel directory,
            Func<IEntryModel, bool> filter = null, Func<IEntryModel, bool> lookupFilter = null,
            bool recrusive = false, Func<IEntryModel[], IScriptCommand> nextCommandFunc = null)
            : this(new IEntryModel[] { directory }, filter, lookupFilter, recrusive, nextCommandFunc)
        {
        }

        async Task<IList<IEntryModel>> listAsync(IEntryModel dir, CancellationToken ct,
            Func<IEntryModel, bool> filter, Func<IEntryModel, bool> lookupFilter, bool refresh)
        {
            List<IEntryModel> retList = new List<IEntryModel>();

            if (lookupFilter(dir))
                foreach (var em in await dir.Profile.ListAsync(dir, ct, em => filter(em) || lookupFilter(em), refresh))
                {
                    if (filter(em))
                        retList.Add(em);
                    ct.ThrowIfCancellationRequested();
                    if (_recrusive && em.IsDirectory && lookupFilter(em))
                        retList.AddRange(await listAsync(em, ct, filter, lookupFilter, refresh));
                }
            return retList;
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {            

            Func<IEntryModel, bool> filter;
            if (pm.ContainsKey("Mask"))
            {
                string mask = pm["Mask"] as string;
                filter = (em => _filter(em) && PathFE.MatchFileMask(em.Name, mask));
            }
            else filter = _filter;
            _lookupFilter = _lookupFilter ?? (em => em.IsDirectory);

            bool refresh = pm.ContainsKey("Refresh") && (bool)pm["Refresh"];

            List<IEntryModel> result = new List<IEntryModel>();
            foreach (var directory in _directories)
            {
                result.AddRange(await listAsync(directory, pm.CancellationToken, filter, _lookupFilter, refresh));
            }

            return _nextCommandFunc(result.ToArray());
        }
    }

    public class DiskCreateCommand : ScriptCommandBase
    {
        private IDiskProfile _profile;
        private Func<IEntryModel, IScriptCommand> _thenFunc;
        private bool _isFolder;
        private string _parentPath;
        private IFileNameGenerator _fileNameGenerator;

        public DiskCreateCommand(IDiskProfile profile, string parentPath, IFileNameGenerator fileNameGenerator, bool isFolder,
            Func<IEntryModel, IScriptCommand> thenFunc)
            : base("ParsePath")
        {
            _profile = profile;
            _parentPath = parentPath;
            _fileNameGenerator = fileNameGenerator;
            _thenFunc = thenFunc;
            _isFolder = isFolder;
        }

        public override bool CanExecute(ParameterDic pm)
        {
            return !_parentPath.StartsWith("::{"); //Cannot execute if GuidPath
        }

        public override async Task<IScriptCommand> ExecuteAsync(ParameterDic pm)
        {
            string fileName = _fileNameGenerator.Generate();
            while (fileName != null &&
                await _profile.ParseAsync(_profile.Path.Combine(_parentPath, fileName)) != null)
                fileName = _fileNameGenerator.Generate();
            if (fileName == null)
                return ResultCommand.Error(new ArgumentException("Already exists."));

            string newEntryPath = _profile.Path.Combine(_parentPath, fileName);
            var createddModel = await _profile.DiskIO.CreateAsync(newEntryPath, _isFolder, pm.CancellationToken);

            return new NotifyChangedCommand(_profile, newEntryPath, FileExplorer.Defines.ChangeType.Created,
                _thenFunc(createddModel));
        }
    }
}
