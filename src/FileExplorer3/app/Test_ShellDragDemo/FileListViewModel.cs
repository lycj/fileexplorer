using FileExplorer;
using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.BaseControls;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Test_ShellDragDemo
{
    public class FileListViewModel : NotifyPropertyChanged,
        ISupportDropHelper, ISupportDragHelper, IContainer<ISelectable>
    {

        #region Constructor

        public FileListViewModel(string label)
        {
            Items = new ObservableCollection<FileViewModel>();

            #region Initialize DynamicCommandDictionary

            Commands = new DynamicRelayCommandDictionary()
            {
                ParameterDicConverter = ParameterDicConverters.FromParameterDic(
                new ParameterDic()
                {
                    { "FileListVM", this }
                })
            };

            Commands.UnselectAll = _unselectCommand; 

            #endregion
          
            #region Initialize DragHelper and DropHelper

            DragHelper = new LambdaShellDragHelper<FileModel>(
                _fileModelConverter,  //Convert IDraggable (FileViewModel) to M (FileModel), ConvertBack is used.
                _dataObjectConverter, //Convert IEnumerable<M> (IEnumerable<FileModel>) to IDataObject, Convert is used.
                () => Items.Where(fvm => fvm.IsSelected).Select(fvm => fvm.Model),
                (fmList) => DragDropEffectsEx.Link | DragDropEffectsEx.Copy | DragDropEffectsEx.Move, //QueryDrag
                (fmList, da, effect) => //OnDragCompleted
                {
                    if (effect == DragDropEffectsEx.Move)
                        foreach (var f in fmList)
                        {
                            var foundItem = Items.FirstOrDefault(fvm => fvm.Model.Equals(f));
                            if (foundItem != null)
                                Items.Remove(foundItem);
                        }
                });

            DropHelper = new LambdaShellDropHelper<FileModel>(
                _fileModelConverter,  //Convert IDraggable (FileViewModel) to M (FileModel), Convert is used.
                _dataObjectConverter, //Convert IEnumerable<M> (IEnumerable<FileModel>) to IDataObject, ConvertBack is used.
                (fms, eff) => //QueryDrop(IEnumerable<FileModel>, DragDropEffectsEx) : QueryDropEffects
                {
                    IEnumerable<FileModel> itemsModel = Items.Select(fvm => fvm.Model);
                    if (fms.Any(f => itemsModel.Contains(f)))
                        return QueryDropEffects.None;
                    return QueryDropEffects.CreateNew(eff & (DragDropEffectsEx.Link | DragDropEffectsEx.Move), eff & DragDropEffectsEx.Move);

                },
                (fms, da, eff) => //Drop(IEnumerable<FileModel>, IDataObject, DragDropEffectsEx) : DragDropEffectsEx
                {
                    foreach (var existingFvm in Items) existingFvm.IsSelected = false;
                    foreach (var fm in fms) Items.Add(new FileViewModel(fm));
                    if (eff.HasFlag(DragDropEffectsEx.Move))
                        return DragDropEffectsEx.Move;
                    if (eff.HasFlag(DragDropEffectsEx.Copy))
                        return DragDropEffectsEx.Copy;
                    return DragDropEffectsEx.Link;
                }) { DisplayName = label };

            #endregion
        }

        #endregion

        #region Methods


        IEnumerable<ISelectable> IContainer<ISelectable>.GetChildItems()
        {
            return Items;
        }
        

        #endregion

        #region Data        
        
        private static IScriptCommand _unselectCommand = new SimpleScriptCommand("UnSelectAll",
            (pd) =>
            {
                FileListViewModel flvm = pd.GetValue<FileListViewModel>("{FileListVM}");
                foreach (var item in flvm.Items)
                    item.IsSelected = false;
                return ResultCommand.NoError;
            });

        //Convert IDraggable (FileViewModel) to M (FileModel).
        private static IValueConverter _fileModelConverter =
            new LambdaValueConverter<FileViewModel, FileModel>(fvm => fvm.Model, fm => new FileViewModel(fm));

        //Convert IEnumerable<M> (IEnumerable<FileModel>) to IDataObject.
        private static IValueConverter _dataObjectConverter = new LambdaValueConverter<IEnumerable<FileModel>, IDataObject>(fmList =>
        {
            var da = new DataObject();
            var fList = new StringCollection();
            foreach (var fm in fmList)
            {
                if (!File.Exists(fm.FileName))
                    using (var sw = File.CreateText(fm.FileName))
                        sw.WriteLine(fm.FileName);
                fList.Add(fm.FileName);
            }
            da.SetFileDropList(fList);
            return da;
        },
                    da =>
                    {
                        List<FileModel> retVal = new List<FileModel>();
                        if ((da as DataObject).ContainsFileDropList())
                            foreach (var file in (da as DataObject).GetFileDropList())
                                retVal.Add(new FileModel(file));
                        return retVal;
                    }
                    );

        #endregion

        #region Public Properties

        public ObservableCollection<FileViewModel> Items { get; set; }
        public ISupportDrag DragHelper { get; set; }
        public ISupportDrop DropHelper { get; set; }
        public dynamic Commands { get; private set; }

        #endregion

        

        



    }
}
