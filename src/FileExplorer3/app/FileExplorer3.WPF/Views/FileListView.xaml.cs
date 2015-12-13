using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FileExplorer.WPF.BaseControls;


namespace FileExplorer.WPF.Views
{
    /// <summary>
    /// Interaction logic for FileListView.xaml
    /// </summary>
    [Export(typeof(UserControl))]
    public partial class FileListView : UserControl
    {
        public FileListView()
        {
            InitializeComponent();            
            //CommandBindings.Add(new CommandBinding(this.ProcessedItems.RenameCommand));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Caliburn.Micro.Bind.SetModel(ProcessedItems, this.DataContext);
            //this.ProcessedItems.ContextMenu.PlacementTarget = this.ProcessedItems;
            //this.RegisterEventProcessors(new DragDropEventProcessor(),
            //    new MultiSelectEventProcessor(vm1.UnselectAllCommand));
        }
    }
}
