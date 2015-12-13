using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using QuickZip.UserControls.MVVM.ViewModel;
using QuickZip.UserControls.MVVM.Model;
using QuickZip.UserControls.MVVM;
using System.Collections;
using System.Windows.Documents;
using System.ComponentModel;
using Cinch;
using System.Windows.Media;
using System.Diagnostics;
using System.IO;
using QuickZip.Translation;
using System.Windows.Input;
using QuickZip.UserControls.Logic.Tools.DragnDrop;
using System.Windows;

namespace QuickZip.UserControls.MVVM
{
    /// <summary>
    /// File Listing View.
    /// </summary>
    /// <typeparam name="FI"></typeparam>
    /// <typeparam name="DI"></typeparam>
    /// <typeparam name="FSI"></typeparam>
    public abstract class DndDirectoryViewerViewModel<FI, DI, FSI> : BaseDirectoryViewerViewModel<FI, DI, FSI>,
        ISupportDrag<EntryViewModel<FI, DI, FSI>>, ISupportDrop<EntryViewModel<FI, DI, FSI>>, IDropTarget<EntryViewModel<FI, DI, FSI>>
        where DI : FSI
        where FI : FSI
    {
        #region Constructor

        public DndDirectoryViewerViewModel(Profile<FI, DI, FSI> profile,
           DirectoryModel<FI, DI, FSI> embedDirectoryModel)
            : base(profile, embedDirectoryModel)
        {
           
        }

        public DndDirectoryViewerViewModel(Profile<FI, DI, FSI> profile)
            : base(profile)
        {
            
        }
        #endregion     

        #region Methods and Public Properties - ISupportDrag implementations


        public DragDropItemInfo<EntryViewModel<FI, DI, FSI>> GetItemInfo(EntryViewModel<FI, DI, FSI> item)
        {
            bool isDir;
            string path = _profile.GetDiskPath(item.EmbeddedModel.EmbeddedEntry, out isDir, false);
            return new DragDropItemInfo<EntryViewModel<FI, DI, FSI>>()
            {
                EmbeddedItem = item,
                FileSystemPath = _profile.GetDiskPath(item.EmbeddedModel.EmbeddedEntry, false),
                IsTemp = item.EmbeddedModel.IsVirtual,
                IsFolder = isDir
            };
        }

        public bool BeforeDrag(DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
        {
            //Debug.WriteLine("Before Drag");
            return true;
        }

        public void PrepareDrop(DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
        {
            this.EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.PrepareDrop(_profile, dropInfo);
        }

        public DragDropEffects SupportedDragDropEffects
        {
            get { return SelectedItems.Length > 0 ? DragDropEffects.Copy | DragDropEffects.Link : DragDropEffects.None; }
        }

        public EntryViewModel<FI, DI, FSI>[] SelectedItems
        {
            get { return (from vm in EmbeddedDirectoryViewModel.SubEntries where vm.IsSelected select vm).ToArray(); }
        }


        #endregion

        #region Methods and Public Properties - ISupportDrop implementations

        public IDropTarget<EntryViewModel<FI, DI, FSI>> CurrentDropTarget
        {
            get { return this; }
        }
        
        public bool IsDropEnabled
        {
            get { return this.EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.IsSupportAdd; }
        }

        public DragDropEffects QueryDrop(DragDropEffects sourceEffects, DragDropInfo<EntryViewModel<FI, DI, FSI>> dragInfo)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.QueryDrop(_profile, sourceEffects, dragInfo);            
        }

        public DragDropEffects QueryDrop(DragDropEffects sourceEffects, string[] draggingFiles)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.QueryDrop(_profile,  sourceEffects, draggingFiles);    
        }

        public DragDropEffects Drop(DragDropEffects sourceEffects, DragDropInfo<EntryViewModel<FI, DI, FSI>> dropInfo)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.Drop(_profile, sourceEffects, dropInfo);    
        }

        public DragDropEffects Drop(DragDropEffects sourceEffects, string[] droppingFiles)
        {
            return EmbeddedDirectoryViewModel.EmbeddedDirectoryModel.Drop(_profile, sourceEffects, droppingFiles);    
        }

        public void NotifyRefresh()
        {
            //throw new NotImplementedException();
        }

        #endregion


      
    }
}
