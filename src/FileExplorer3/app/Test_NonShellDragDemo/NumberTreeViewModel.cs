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
    public class NumberTreeViewModel : ISupportDragHelper, ISupportDropHelper
    {


        public NumberTreeViewModel(string displayName)            
        {
            Items = new ObservableCollection<NumberNodeViewModel>();


            var converter = new LambdaValueConverter<IDraggable, NumberModel>(
                d => (d is NumberViewModel) ? (d as NumberViewModel).Model : null,
                nm => new NumberNodeViewModel(this, nm));

            DragHelper = new LambdaDragHelper<NumberModel>(converter,
                () => Items.Where(nvm => nvm.IsSelected).Select(nvm => nvm.Model).ToList(),
                nms => DragDropEffectsEx.Move,
                (nms, eff) =>
                {
                    foreach (var nm in nms)
                        Items.Remove(new NumberNodeViewModel(this, nm));
                }
                );
            DropHelper = new LambdaDropHelper<NumberModel>(converter,
                (nms, eff) =>
                {
                    foreach (var nm in nms)
                        if (Items.Any(ivm => ivm.Model.Equals(nm)))
                            return QueryDropEffects.None;
                    return QueryDropEffects.CreateNew(DragDropEffectsEx.Move);
                },
                (nms, eff) =>
                {
                    foreach (var nm in nms)
                        Items.Add(new NumberNodeViewModel(this, nm));
                    return DragDropEffectsEx.Move;
                }) { DisplayName = displayName };
        }

        public ObservableCollection<NumberNodeViewModel> Items { get; set; }
        public ISupportDrag DragHelper { get; set; }
        public ISupportDrop DropHelper { get; set; }
    }
}
