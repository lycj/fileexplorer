using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer.WPF.UserControls;
using FileExplorer.WPF.Utils;
using FileExplorer.UIEventHub;
using FileExplorer.Defines;

namespace Test_NonShellDragDemo
{
    public class DragDropItemViewModel : NotifyPropertyChanged, ISupportDrag, ISupportDrop, IDraggable, ISelectable
    {
        public static string Format_DragDropItem = "DragDropItemVM";

        #region Constructor

        public DragDropItemViewModel(int value, bool isDroppable, bool isChildDroppable)
        {
            IsDroppable = isDroppable;
            IsChildDroppable = isChildDroppable;
            Value = value;
            DisplayName = Value.ToString();
            DropTargetLabel = "{MethodLabel} {ItemLabel} to {ISupportDrop.DisplayName}";
            //UnselectAllCommand = new SimpleCommand()
            //{
            //    ExecuteDelegate = (param) =>
            //    {
            //        foreach (var item in Items)
            //            item.IsSelected = false;
            //    }
            //};
        }


        public DragDropItemViewModel(int startId, int count, bool isDroppable, bool isChildDroppable)
            : this(-1, isDroppable, isChildDroppable)
        {
            for (int i = startId; i < startId + count; i++)
                _items.Add(new DragDropItemViewModel(i, isChildDroppable, isChildDroppable));


        }

        #endregion

        #region Methods



        public bool HasDraggables
        {
            get { return GetDraggables().Any(); }
        }

        public bool IsDroppable
        {
            get;
            set;
        }





        public bool IsChildDroppable { get; set; }

        public IEnumerable<IDraggable> GetDraggables()
        {
            return Items.Where(i => i.IsSelected).Cast<IDraggable>().ToList();
        }

        //public IDataObject GetDataObject(IEnumerable<IDraggable> draggables)
        //{
        //    return new DataObject(Format_DragDropItem,
        //        (from i in draggables.Cast<DragDropItemViewModel>() where i.IsSelected select i.Value).ToArray());
        //}

        public DragDropEffectsEx QueryDrag(IEnumerable<IDraggable> draggables)
        {
            return DragDropEffectsEx.Move | DragDropEffectsEx.Copy;
        }


        public void OnDragCompleted(IEnumerable<IDraggable> draggables, DragDropEffectsEx effect)
        {
            if (effect == DragDropEffectsEx.Move)
            {
                foreach (var item in draggables.Cast<DragDropItemViewModel>())
                    if (Items.Contains(item))
                        Items.Remove(item);
            }
        }




        public QueryDropEffects QueryDrop(IEnumerable<IDraggable> draggables, DragDropEffectsEx allowedEffects)
        {
            if (draggables.Count() == 0)
                return QueryDropEffects.None;
            foreach (var dm in draggables.Cast<DragDropItemViewModel>())             
                if (dm.Value == this.Value || this.Items.Contains(dm))
                    return QueryDropEffects.None;
            return QueryDropEffects.CreateNew(DragDropEffectsEx.Move | DragDropEffectsEx.Copy, DragDropEffectsEx.Move);
        }

        //public IEnumerable<IDraggable> QueryDropDraggables(IDataObject da)
        //{
        //    if (da.GetDataPresent(Format_DragDropItem))
        //    {
        //        var data = da.GetData(Format_DragDropItem) as int[];
        //        for (int i = 0; i < data.Length; i++)
        //            yield return new DragDropItemViewModel(data[i], IsChildDroppable, IsChildDroppable);
        //    }
        //}

        public DragDropEffectsEx Drop(IEnumerable<IDraggable> draggable, DragDropEffectsEx allowedEffects)
        {
            if (allowedEffects.HasFlag(DragDropEffectsEx.Move) ||
                allowedEffects.HasFlag(DragDropEffectsEx.Copy))
            {
                var draggableViewModels = draggable.Cast<DragDropItemViewModel>();
                if (draggableViewModels.Any())
                {
                    int idx = 0;
                    foreach (var d in draggableViewModels)
                        Items.Insert(idx++, d);
                }
                return DragDropEffectsEx.Move;
            }
            else return DragDropEffectsEx.None;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion

        #region Data

        private bool _isSelected = false;
        private ObservableCollection<DragDropItemViewModel> _items = new ObservableCollection<DragDropItemViewModel>();
        private bool _isDraggingOver, _isDraggingFrom;
        private bool _isDragging = false;
        private bool _isSelecting = false;

        #endregion

        #region Public Properties

        public string Id { get; set; }
        public ICommand UnselectAllCommand { get; set; }
        public ObservableCollection<DragDropItemViewModel> Items { get { return _items; } }
        public bool IsDraggingOver
        {
            get { return _isDraggingOver; }
            set { _isDraggingOver = value; NotifyOfPropertyChanged(() => IsDraggingOver); }
        }

        public bool IsDraggingFrom
        {
            get { return _isDraggingFrom; }
            set { _isDraggingFrom = value; NotifyOfPropertyChanged(() => IsDraggingFrom); }
        }
        public int Value { get; set; }
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyOfPropertyChanged(() => IsSelected); }
        }

        public bool IsSelecting
        {
            get { return _isSelecting; }
            set { _isSelecting = value; NotifyOfPropertyChanged(() => IsSelecting); }
        }


        public string DropTargetLabel
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }



        public bool IsDragging { get { return _isDragging; } set { _isDragging = value; NotifyOfPropertyChanged(() => IsDragging); } }


        #endregion










    }



}
