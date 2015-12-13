using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.UIEventHub
{
    /// <summary>
    /// if MultiSelectEventProcessor is used on a 
    /// ItemsControl with Virtual Panel, and the VirtualPanel does not support 
    /// IChildInfo nor is GridView, HitTest is used, the ViewModel should implement 
    /// IContainer[ISelectable] so FindSelectedItemsUsingHitTest ScriptCommand can 
    /// locate non-visible items.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IContainer<T>
    {
        IEnumerable<T> GetChildItems();
    }
}
