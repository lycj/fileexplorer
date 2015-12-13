using FileExplorer.Defines;
using FileExplorer.UIEventHub;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Test_NonShellDragDemo
{
    public class NumberListViewModel : NotifyPropertyChanged, ISupportDragHelper, ISupportDropHelper
    {
        public static NumberListViewModel GenerateRange(string displayName, int start, int end)
        {
            NumberListViewModel nvm = new NumberListViewModel(displayName);
            for (int i = start; i <= end; i++)
                nvm.Items.Add(new NumberViewModel(i));            
            return nvm;
        }

        public NumberListViewModel(string displayName)
        {
            Items = new ObservableCollection<NumberViewModel>();

            //Convert from IDraggable (implemented by ViewModel) 
            //to Model (value transferred from dragsource to drop target).
            var converter = new LambdaValueConverter<IDraggable, NumberModel>(
                d => (d is NumberViewModel) ? (d as NumberViewModel).Model : null,
                nm => new NumberViewModel(nm));

            DragHelper = new LambdaDragHelper<NumberModel>(converter,
                //GetModels()
                () => Items.Where(nvm => nvm.IsSelected).Select(nvm => nvm.Model).ToList(),
                nms => DragDropEffectsEx.Move, //OnQueryDrag(models)
                (nms, eff) =>
                {
                    //OnDropCompleted(models, dropEffect)
                    foreach (var nm in nms)
                        Items.Remove(new NumberViewModel(nm));
                }
                );
            DropHelper = new LambdaDropHelper<NumberModel>(converter,
                (nms, eff) =>
                {
                    //OnQueryDrop(models, possibleEffects)
                    foreach (var nm in nms)
                        if (Items.Any(ivm => ivm.Model.Equals(nm)))
                            return QueryDropEffects.None;
                    return QueryDropEffects.CreateNew(DragDropEffectsEx.Move);
                },
                (nms, eff) =>
                {
                    //OnDrop(models, possibleEffects)
                    foreach (var nm in nms)
                        Items.Add(new NumberViewModel(nm));
                    return DragDropEffectsEx.Move;
                }) { DisplayName = displayName };
        }

        public ObservableCollection<NumberViewModel> Items { get; set; }
        public ISupportDrag DragHelper { get; set; }
        public ISupportDrop DropHelper { get; set; }
    }
}
