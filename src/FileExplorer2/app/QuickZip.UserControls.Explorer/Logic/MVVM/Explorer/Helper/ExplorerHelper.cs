using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickZip.UserControls.Logic.Tools.DragnDrop;
using QuickZip.UserControls.MVVM.ViewModel;

namespace QuickZip.UserControls.MVVM
{
    public static class ExplorerHelper
    {
        public static void RegisterConstructMethod<FI, DI, FSI>(ExplorerViewModel<FI, DI, FSI> explorervm)
            where FI : FSI
            where DI : FSI
        {
            if (FileDragDropHelper<EntryViewModel<FI, DI, FSI>>.ConstructItem == null)
                FileDragDropHelper<EntryViewModel<FI, DI, FSI>>.ConstructItem = (str) =>
                    explorervm.Profile.ConstructEntryViewModel(explorervm.Profile.ConstructEntry(str));
        }

        public static void RegisterDragAndDrop<FI, DI, FSI>(this ExplorerViewModel<FI, DI, FSI> explorervm, FileList2 fileList)
            where FI : FSI
            where DI : FSI
        {
            FileDragDropHelper<EntryViewModel<FI, DI, FSI>>.SetEnableDrag(fileList, true);
            FileDragDropHelper<EntryViewModel<FI, DI, FSI>>.SetEnableDrop(fileList, true);
            RegisterConstructMethod(explorervm);
        }

        public static void RegisterDragAndDrop<FI, DI, FSI>(this ExplorerViewModel<FI, DI, FSI> explorervm, DirectoryTree2 dtree)
            where FI : FSI
            where DI : FSI
        {
            FileDragDropHelper<EntryViewModel<FI, DI, FSI>>.SetEnableDrag(dtree, true);
            FileDragDropHelper<EntryViewModel<FI, DI, FSI>>.SetEnableDrop(dtree, true);
            RegisterConstructMethod(explorervm);
        }

        public static void RegisterDragTemplate<FI, DI, FSI>(this ExplorerViewModel<FI, DI, FSI> explorervm, System.Windows.UIElement control, System.Windows.DataTemplate itemTemplate)
            where FI : FSI
            where DI : FSI
        {
            FileDragDropHelper<EntryViewModel<FI, DI, FSI>>.SetDragItemTemplate(control, itemTemplate);
        }
    }
}
