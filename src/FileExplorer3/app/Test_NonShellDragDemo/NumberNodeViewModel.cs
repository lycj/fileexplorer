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
    public class NumberNodeViewModel : NumberViewModel, ISupportDragHelper, ISupportDropHelper,
        ISelectable, IDraggable
    {
        private object _parentNode;


        public NumberNodeViewModel(object parentNode, NumberModel model)
            : base(model)
        {
            _parentNode = parentNode;
            Items = new ObservableCollection<NumberNodeViewModel>();

            var converter = new LambdaValueConverter<IDraggable, NumberModel>(
                d => (d is NumberViewModel) ? (d as NumberViewModel).Model : null,
                nm => new NumberNodeViewModel(this, nm));

            DragHelper = new LambdaDragHelper<NumberModel>(converter,
                () => GetAllSubItems(true).Select(nvm => nvm.Model).ToList(),
                nms => DragDropEffectsEx.Move,
                (nms, eff) =>
                {
                    if (parentNode is NumberNodeViewModel)
                        (parentNode as NumberNodeViewModel).Items.Remove(this);
                    else if (parentNode is NumberTreeViewModel)
                        (parentNode as NumberTreeViewModel).Items.Remove(this);

                    //foreach (var nm in nms)
                    //    Items.Remove(new NumberNodeViewModel(this, nm));
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
                }) { DisplayName = "Node " + model.Value.ToString() };

        }

        public IEnumerable<NumberNodeViewModel> GetAllSubItems(bool includeCurrent)
        {
            if (includeCurrent)
                yield return this;
            foreach (var item in Items)
                foreach (var subitem in item.GetAllSubItems(true))
                    yield return subitem;
        }

        public ObservableCollection<NumberNodeViewModel> Items { get; set; }
        public ISupportDrag DragHelper { get; set; }
        public ISupportDrop DropHelper { get; set; }
    }
}
