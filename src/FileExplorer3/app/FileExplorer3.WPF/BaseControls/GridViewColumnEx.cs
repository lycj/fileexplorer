using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;

namespace FileExplorer.WPF.UserControls
{
    public class GridViewColumnEx : GridViewColumn
    {
        #region Cosntructor

        #endregion

        #region Methods

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public static readonly DependencyProperty ColumnIdProperty =
            DependencyProperty.Register("ColumnId", typeof(string), typeof(GridViewColumnEx), new PropertyMetadata(""));

        /// <summary>
        /// Used to identify a GridViewColumn
        /// </summary>
        public string ColumnId
        {
            get { return (string)GetValue(ColumnIdProperty); }
            set { SetValue(ColumnIdProperty, value); }
        }

        public static readonly DependencyProperty ColumnWidthProperty =
           DependencyProperty.Register("ColumnWidth", typeof(double), typeof(GridViewColumnEx));
        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        //public static readonly DependencyProperty ColumnFiltersProperty =
        //   DependencyProperty.Register("ColumnFilters", typeof(ColumnFilter[]), typeof(GridViewColumnEx),
        //   new PropertyMetadata(new ColumnFilter[] { }));

        ///// <summary>
        ///// Used to identify a GridViewColumn
        ///// </summary>
        //public ColumnFilter[] ColumnFilters
        //{
        //    get { return (ColumnFilter[])GetValue(ColumnFiltersProperty); }
        //    set { SetValue(ColumnFiltersProperty, value); }
        //}

        #endregion

    }
}
