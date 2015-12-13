using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Model;
//using QuickZip.IO.COFE;
using QuickZip.Converters;
using System.IO;
using QuickZip.UserControls.MVVM.Command.Model;
using System.Windows.Input;

namespace QuickZip.UserControls.MVVM
{    

    public class ExProfile : Profile<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>
    {

        public ExProfile() 
            : base(new ExIconExtractor())            
        {
            RootDirectories = new DirectoryInfoEx[] { DirectoryInfoEx.DesktopDirectory };
        }

        public override DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> ConstructDirectoryModel(DirectoryInfoEx dir)
        {
            return new ExDirectoryModel(dir);
        }

        public override FileModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> ConstructFileModel(FileInfoEx file)
        {
            return new ExFileModel(file);
        }

        public override FileSystemInfoEx ConstructEntry(string parseName)
        {
            return FileSystemInfoEx.FromString(parseName);
        }

        public override string GetDiskPath(FileSystemInfoEx entry, out bool isDir, bool createNowIfNotExist = true)
        {
            isDir = entry is DirectoryInfoEx;
            return entry.FullName;
        }

        public override IEnumerable<Suggestion> Lookup(string lookupText)
        {
            yield return new Suggestion(lookupText + "_FileExplorer2");
            yield break;
        }

        public override IEnumerable<GenericMetadataModel> GetMetadata(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
        {
            if (appliedModels.Length == 1)
            {
                var model = appliedModels[0];
                var entry = model.EmbeddedEntry;

                yield return new EntryMetadataModel<String, FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>(appliedModels,
                       model.Label, "Key_Selected_0_Label");

                if (entry is FileInfoEx)
                    yield return new EntryMetadataModel<String, FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>(appliedModels,
                       UITools.SizeInK((ulong)model.Length), "Key_Selected_0_Size", "Selected size");

                yield return new EntryMetadataModel<short, FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>(appliedModels,
                      13, "Percent_Test", "13 Percent");
                yield return new EntryMetadataModel<DateTime, FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>(appliedModels,
                      DateTime.Now, "DateTime_Test", "Now");

            }
            yield break;
        }

        public override IEnumerable<CommandModel> GetCommands(EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
        {

            yield return new GenericCommandModel(ApplicationCommands.Close);
            yield break;
        }

        public override string ShowContextmenu(System.Windows.Point screenPos, EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
        {
            return null;
        }

        public override void ShowProperties(System.Windows.Point screenPos, EntryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx>[] appliedModels)
        {
            
        }

        public override IEnumerable<Notification.Model.NotificationSourceModel> GetNotificationSources()
        {
            yield break;
        }

        public override void Copy(FileSystemInfoEx[] entries, DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel, bool allowThread = true)
        {
            throw new NotImplementedException();
        }

        public override void Link(FileSystemInfoEx[] entries, DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel)
        {
            throw new NotImplementedException();
        }

        public override AddActions GetSupportedAddActions(FileSystemInfoEx[] entries, DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel)
        {
            return AddActions.Copy;
        }

        public override DirectoryInfoEx CreateDirectory(DirectoryModel<FileInfoEx, DirectoryInfoEx, FileSystemInfoEx> directoryModel, string name, string type)
        {
            throw new NotImplementedException();
        }
    }

}
