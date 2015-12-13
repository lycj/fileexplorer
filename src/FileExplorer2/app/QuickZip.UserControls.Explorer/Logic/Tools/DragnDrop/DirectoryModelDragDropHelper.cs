using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using QuickZip.UserControls.MVVM.Model;
using System.IO;
using QuickZip.UserControls.MVVM;
using QuickZip.UserControls.MVVM.ViewModel;

namespace QuickZip.UserControls.Logic.Tools.DragnDrop
{
    public static class DirectoryModelDragDropHelper        
    {
        public static void PrepareDrop<FI, DI, FSI>(this DirectoryModel<FI, DI, FSI> dirModel, Profile<FI, DI, FSI> profile,
            DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
              where FI : FSI
              where DI : FSI
        {
            Func<bool> isCommonBaseDirectory
                = () =>
                {
                    if (dropInfo.SelectedItems.Count == 0)
                        return false;
                    string parentPath = PathEx.GetDirectoryName(dropInfo.SelectedItems[0].FileSystemPath);

                    foreach (var itemInfo in dropInfo.SelectedItems)
                        if (PathEx.GetDirectoryName(dropInfo.SelectedItems[0].FileSystemPath) != parentPath)
                            return false;

                    return true;
                };

            if (isCommonBaseDirectory())
            {
                FSI[] tempItems =
                    (from ii in dropInfo.SelectedItems where ii.IsTemp select ii.EmbeddedItem.EmbeddedModel.EmbeddedEntry).ToArray();
                if (tempItems.Length > 0)
                {
                    string parentPath = PathEx.GetDirectoryName(dropInfo.SelectedItems[0].FileSystemPath);
                    profile.Put(tempItems, (DI)profile.ConstructEntry(parentPath), false);
                    foreach (var itemInfo in dropInfo.SelectedItems)
                        itemInfo.IsTemp = false;
                }
            }
            else
                foreach (var itemInfo in dropInfo.SelectedItems)
                    if (itemInfo.IsTemp)
                    {
                        itemInfo.IsTemp = false;
                        profile.GetDiskPath(itemInfo.EmbeddedItem.EmbeddedModel.EmbeddedEntry, true);
                    }
        }

        public static DragDropEffects QueryDrop<FI, DI, FSI>(this DirectoryModel<FI, DI, FSI> dirModel, Profile<FI, DI, FSI> profile, 
            DragDropEffects sourceEffects,
            DragDropInfo<EntryViewModel<FI, DI, FSI>> dragInfo)
            where FI : FSI
            where DI : FSI
        {
            if (dirModel.IsSupportAdd)
            {
                DragDropEffects retDropEffects = DragDropEffects.None;

                foreach (DragDropItemInfo<EntryViewModel<FI, DI, FSI>> item in dragInfo.SelectedItems)
                    if (!dirModel.IsDroppableOverItself && (dirModel.EmbeddedDirectory.Equals(item.EmbeddedItem.EmbeddedModel.EmbeddedEntry) ||
                        dirModel.EmbeddedDirectory.Equals(item.EmbeddedItem.EmbeddedModel.Parent)))
                        return DragDropEffects.None;

                var fsis = from si in dragInfo.SelectedItems select si.EmbeddedItem.EmbeddedModel.EmbeddedEntry;

                if ((sourceEffects & DragDropEffects.Copy) != 0 &&
                    (profile.GetSupportedAddActions(fsis.ToArray(), dirModel) & AddActions.Copy) != 0)
                    retDropEffects |= DragDropEffects.Copy;

                if ((sourceEffects & DragDropEffects.Link) != 0 &&
                  (profile.GetSupportedAddActions(fsis.ToArray(), dirModel) & AddActions.Link) != 0)
                    retDropEffects |= DragDropEffects.Link;

                return retDropEffects;
            }
            return DragDropEffects.None;
        }

        public static DragDropEffects QueryDrop<FI, DI, FSI>(this DirectoryModel<FI, DI, FSI> dirModel, Profile<FI, DI, FSI> profile, 
            DragDropEffects sourceEffects, string[] draggingFiles)
            where FI : FSI
            where DI : FSI
        {
            if (draggingFiles.Length > 0 && dirModel.IsSupportAdd)
            {
                DragDropEffects retDropEffects = DragDropEffects.None;

                foreach (var file in draggingFiles)
                    if (!dirModel.IsDroppableOverItself && 
                        (dirModel.ParseName.Equals(file) || dirModel.ParseName.IndexOf(PathEx.GetDirectoryName(file)) == 0))
                        return DragDropEffects.None;

                FSI[] dummyFSIList = new FSI[] { profile.ConstructEntry(draggingFiles[0]) };

                if ((sourceEffects & DragDropEffects.Copy) != 0 &&
                    (profile.GetSupportedAddActions(dummyFSIList, dirModel) & AddActions.Copy) != 0)
                    retDropEffects |= DragDropEffects.Copy;

                if ((sourceEffects & DragDropEffects.Link) != 0 &&
                    (profile.GetSupportedAddActions(dummyFSIList, dirModel) & AddActions.Link) != 0)
                    retDropEffects |= DragDropEffects.Link;

                return retDropEffects;
            }
            return DragDropEffects.None;
        }


        public static DragDropEffects Drop<FI, DI, FSI>(this DirectoryModel<FI, DI, FSI> dirModel, Profile<FI, DI, FSI> profile,
            DragDropEffects sourceEffects, DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
            where FI : FSI
            where DI : FSI
        {
            var models = from si in dropInfo.SelectedItems select si.EmbeddedItem.EmbeddedModel;

            profile.Put(models.ToArray(), dirModel);

            return DragDropEffects.Copy;
        }

        public static DragDropEffects Drop<FI, DI, FSI>(this DirectoryModel<FI, DI, FSI> dirModel, Profile<FI, DI, FSI> profile,
            DragDropEffects sourceEffects, string[] droppingFiles)
            where FI : FSI
            where DI : FSI
        {
            var fsis = from f in droppingFiles select profile.ConstructEntry(f);

            profile.Put(fsis.ToArray(), dirModel.EmbeddedDirectory);

            return DragDropEffects.Copy;
        }

    }
}
