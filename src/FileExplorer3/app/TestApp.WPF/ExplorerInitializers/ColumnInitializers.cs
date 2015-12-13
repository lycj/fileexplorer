
using Caliburn.Micro;
using FileExplorer.Script;
using FileExplorer;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer.WPF.Models;
using FileExplorer.WPF;
using FileExplorer.Defines;
using FileExplorer.IO;

namespace TestApp
{
    public class ColumnInitializers : IViewModelInitializer<IExplorerViewModel>
    {
        public static void InitializeColumnInfo(IExplorerViewModel explorerModel)
        {
            explorerModel.FileList.Columns.ColumnList = new ColumnInfo[] 
                {
                    ColumnInfo.FromTemplate("Name", "GridLabelTemplate", "EntryModel.Label", new ValueComparer<IEntryModel>(p => p.Label), 200),   
                    ColumnInfo.FromBindings("Type", "EntryModel.Description", "", new ValueComparer<IEntryModel>(p => p.Description), 200),
                    
                    ColumnInfo.FromBindings("Time", "EntryModel.LastUpdateTimeUtc", "", 
                        new ValueComparer<IEntryModel>(p => 
                            (p is DiskEntryModelBase) ? (p as DiskEntryModelBase).LastUpdateTimeUtc
                            : DateTime.MinValue), 200), 
        
                    ColumnInfo.FromTemplate("Size", "GridSizeTemplate", "", 
                    new ValueComparer<IEntryModel>(p => 
                        (p is DiskEntryModelBase) ? (p as DiskEntryModelBase).Size
                        : 0), 200),  

                    ColumnInfo.FromBindings("FSI.Attributes", "EntryModel.Attributes", "", 
                        new ValueComparer<IEntryModel>(p => 
                            (p is FileSystemInfoModel) ? (p as FileSystemInfoModel).Attributes
                            : System.IO.FileAttributes.Normal), 200)   
                };

            explorerModel.FileList.Columns.ColumnFilters = new ColumnFilter[]
                {
                    ColumnFilter.CreateNew<IEntryModel>("0 - 9", "EntryModel.Label", e => Regex.Match(e.Label, "^[0-9]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("A - H", "EntryModel.Label", e => Regex.Match(e.Label, "^[A-Ha-h]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("I - P", "EntryModel.Label", e => Regex.Match(e.Label, "^[I-Pi-i]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("Q - Z", "EntryModel.Label", e => Regex.Match(e.Label, "^[Q-Zq-z]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("The rest", "EntryModel.Label", e => Regex.Match(e.Label, "^[^A-Za-z0-9]").Success),
                    ColumnFilter.CreateNew<IEntryModel>("Today", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year == dt.Year && 
                                e.LastUpdateTimeUtc.Month == dt.Month && 
                                e.LastUpdateTimeUtc.Day == dt.Day;
                        }),
                    ColumnFilter.CreateNew<IEntryModel>("Earlier this month", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year == dt.Year && e.LastUpdateTimeUtc.Month == dt.Month;
                        }),
                     ColumnFilter.CreateNew<IEntryModel>("Earlier this year", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year == dt.Year;
                        }), 
                    ColumnFilter.CreateNew<IEntryModel>("A long time ago", "EntryModel.LastUpdateTimeUtc", e => 
                        {
                            DateTime dt = DateTime.UtcNow;
                            return e.LastUpdateTimeUtc.Year != dt.Year;
                        }),    
                    ColumnFilter.CreateNew<IEntryModel>("Directories", "EntryModel.Description", e => e.IsDirectory),
                    ColumnFilter.CreateNew<IEntryModel>("Files", "EntryModel.Description", e => !e.IsDirectory)
                };
        }

        public async Task InitalizeAsync(IExplorerViewModel explorerModel)
        {
            InitializeColumnInfo(explorerModel);
        }
    }

}


