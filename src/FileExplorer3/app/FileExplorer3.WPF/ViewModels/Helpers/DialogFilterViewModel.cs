using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF.ViewModels.Helpers
{
    


    //public class FileNameFilterHelper : NotifyPropertyChanged
    //{
        
    //    #region Constructor

    //    public FileNameFilterHelper(string filterStr)
    //    {
    //        string[] filterSplit = filterStr.Split('|');
    //        List<FileNameFilter> ff = new List<FileNameFilter>();
    //        for (int i = 0; i < filterSplit.Count() / 2; i++)
    //            ff.Add(new FileNameFilter(filterSplit[i*2], filterSplit[(i*2)+1]));
    //        All = ff.ToArray();
    //    }

    //    #endregion

    //    #region Methods

    //    #endregion

    //    #region Data

    //    private FileNameFilter _selectedFilter = null;
    //    private FileNameFilter[] _allFilters = null;

    //    #endregion

    //    #region Public Properties

    //    public FileNameFilter[] All { get { return _allFilters; } set { _allFilters = value; NotifyOfPropertyChanged(() => All); } }
    //    public FileNameFilter SelectedFilter
    //    {
    //        get { return _selectedFilter; } 
    //        set { _selectedFilter = value; NotifyOfPropertyChanged(() => SelectedFilter); } }

    //    #endregion
    //}
}
