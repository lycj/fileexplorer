using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using FileExplorer.WPF.BaseControls;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Defines;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.UserControls
{
    public static class ListViewColumnUtils
    {
        public static GridViewColumn createColumn(ListView listView, ColumnInfo colInfo)
        {
            DataTemplate dt = null;
            if (colInfo.TemplateKey != null)
                dt = listView.FindResource(colInfo.TemplateKey) as DataTemplate;
            else
            {
                dt = new DataTemplate();
                FrameworkElementFactory label = new FrameworkElementFactory(typeof(TextBlock));
                label.SetBinding(TextBlock.TextProperty, new Binding(colInfo.ValuePath));
                if (!(String.IsNullOrEmpty(colInfo.TooltipPath)))
                    label.SetValue(TextBlock.ToolTipProperty, new Binding(colInfo.TooltipPath));
                label.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Left);
                label.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left);

                dt.VisualTree = label;
            }
            return new GridViewColumnEx()
            {
                Header = colInfo.Header,
                CellTemplate = dt,
                
                ColumnWidth = colInfo.Width,
                //ColumnFilters = colInfo.Filters.ToArray()
            };
        }

        public static void AddColumn(ListView listView, GridViewColumnCollection columnCol, ColumnInfo[] colInfos)
        {
            foreach (var colInfo in colInfos)
            {
                if (columnCol.Any(c => c.Header.Equals(colInfo.Header))) //Prevent re-add.
                    break;
                else columnCol.Add(createColumn(listView, colInfo));
            }
        }

        public static void UpdateColumn(ListView listView, ColumnInfo[] colInfos)
        {
            GridViewColumnCollection columnCol = getColumnCols(listView);
            if (columnCol != null)
            {
                columnCol.Clear();
                AddColumn(listView, columnCol, colInfos);
            }
        }

        private static GridViewColumnCollection getColumnCols(ListView listView)
        {
            if (listView.View is GridView)
                return (listView.View as GridView).Columns;
            else
                if (listView.View is VirtualWrapPanelView)
                    return (listView.View as VirtualWrapPanelView).Columns;
                else if (listView.View is VirtualStackPanelView)
                    return (listView.View as VirtualStackPanelView).Columns;
            return null;
        }

        private static GridViewColumnHeader findColumnHeader(GridViewHeaderRowPresenter headerPresenter, ColumnInfo col)
        {
            Func<TextBlock, bool> filter = tb =>
            {
                var value = tb.GetValue(TextBlock.TextProperty);
                var match = col.Header != null && col.Header.Equals(value);
                return match;
            };
            GridViewColumnHeader foundHeader = null;

            TextBlock lookup = UITools.FindVisualChild<TextBlock>(headerPresenter, filter);
            if (lookup != null)
                foundHeader = lookup.FindAncestor<GridViewColumnHeader>();

            return foundHeader;
        }

        /// <summary>
        /// Apply header template to GridViewColumns depending if it's selected and ascending.
        /// </summary>
        public static void UpdateSortSymbol(ListView listView, ColumnInfo sortCol,
            ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            GridViewColumnCollection columns = getColumnCols(listView);

            if (columns == null)
                return;

            GridViewHeaderRowPresenter presenter = UITools.FindVisualChild<GridViewHeaderRowPresenter>(listView);

            if (presenter != null)
            {
                GridViewColumnHeader foundHeader = findColumnHeader(presenter, sortCol);
                IEnumerable<GridViewColumnHeader> headers = UITools.FindAllVisualChildren<GridViewColumnHeader>(presenter);

                foreach (var curHeader in headers)
                {
                    if (curHeader.Equals(foundHeader))
                    {
                        if (sortDirection == ListSortDirection.Ascending)
                            ListViewEx.SetColumnHeaderSortDirection(curHeader, -1);
                        else ListViewEx.SetColumnHeaderSortDirection(curHeader, 1);
                    }
                    else
                        ListViewEx.SetColumnHeaderSortDirection(curHeader, 0);
                }
            }
        }

        /// <summary>
        /// If a filter is checked or unchecked, raise FilterChangedEvent.
        /// </summary>
        /// <param name="listView"></param>
        public static void RegisterFilterEvent(ListViewEx listView)
        {
            var p = UITools.FindVisualChild<GridViewHeaderRowPresenter>(listView);
            if (p != null)
            {
                var handler = (RoutedEventHandler)((o, e) =>
                {
                    ColumnFilter filter = (e.OriginalSource as MenuItem).DataContext as ColumnFilter;
                    if (filter != null)
                    {
                        ColumnInfo colInfo = ((VisualTreeHelper.GetParent(e.OriginalSource as DependencyObject))
                            as StackPanel).DataContext as ColumnInfo;
                        listView.RaiseEvent(new FilterChangedEventArgs(e.Source) { ColumnInfo = colInfo, Filter = filter });
                    }
                });
                p.AddHandler(MenuItem.CheckedEvent, handler);
                p.AddHandler(MenuItem.UncheckedEvent, handler);
            }
        }

        public static void UpdateFilterPanel(ListView listView, ColumnInfo[] columns, ColumnFilter[] filters)
        {
            Func<ColumnFilter, MenuItem> createMenuItem =
                f =>
                {
                    var style = listView.FindResource("ListViewExHeaderMenuItemHeaderStyle") as Style;
                    var retVal = new MenuItem()
                    {

                        IsCheckable = true,
                        Style = style,
                        Header = f,
                        DataContext = f
                    };
                    retVal.SetBinding(MenuItem.IsCheckedProperty, new Binding("IsChecked"));
                    return retVal;
                };

            if (columns == null)
                return;
            GridViewHeaderRowPresenter presenter = UITools.FindVisualChild<GridViewHeaderRowPresenter>(listView);

            if (presenter != null)
                foreach (var col in columns)
                {
                    GridViewColumnHeader foundHeader = findColumnHeader(presenter, col);
                    if (foundHeader != null)
                    {
                        var dropDown = UITools.FindVisualChildByName<DropDown>(foundHeader, "PART_DropDown");
                        if (dropDown != null)
                        {
                            StackPanel sp = new StackPanel() { Name = "PART_Menu", DataContext = col };
                            foreach (var f in filters)
                                if (f.ValuePath == col.ValuePath)
                                    sp.Children.Add(createMenuItem(f));
                            if (sp.Children.Count > 0)
                                dropDown.Content = sp;
                            else dropDown.Content = null;
                        }
                    }
                }


        }
    }
}
