using FileExplorer.Models;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileExplorer.WPF.Defines.Converters
{
    public class EntryModelToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IEntryModel)
                return (value as IEntryModel).FullPath;
            else if (value is List<IEntryModel>)
                return String.Join(",", (value as List<IEntryModel>).Select(em => em.FullPath).ToArray());
            else if (value is IEntryModel[])
                return String.Join(",", (value as IEntryModel[]).Select(em => em.FullPath).ToArray());
            else if (value is IEntryViewModel)
                return (value as IEntryViewModel).EntryModel.FullPath;
            else if (value is List<IEntryViewModel>)
                return String.Join(",", (value as List<IEntryViewModel>).Select(evm => evm.EntryModel.FullPath).ToArray());
            else if (value is IEntryViewModel[])
                return String.Join(",", (value as IEntryViewModel[]).Select(evm => evm.EntryModel.FullPath).ToArray());

            else return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}
