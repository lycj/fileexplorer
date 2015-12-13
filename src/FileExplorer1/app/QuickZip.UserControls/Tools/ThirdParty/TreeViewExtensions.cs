//http://forums.silverlight.net/forums/p/65277/163239.aspx
using System.Windows.Controls;

public static class TreeViewExtensions
{
    public static TreeViewItem ContainerFromItem(this TreeView treeView, object item)
    {
        TreeViewItem containerThatMightContainItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);
        if (containerThatMightContainItem != null)
            return containerThatMightContainItem;
        else
            return ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);
    }

    private static TreeViewItem ContainerFromItem(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, object item)
    {
        foreach (object curChildItem in itemCollection)
        {
            TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
            TreeViewItem containerThatMightContainItem = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
            if (containerThatMightContainItem != null)
                return containerThatMightContainItem;
            TreeViewItem recursionResult = ContainerFromItem(parentContainer.ItemContainerGenerator, parentContainer.Items, item);
            if (recursionResult != null)
                return recursionResult;
        }
        return null;
    }

    public static object ItemFromContainer(this TreeView treeView, TreeViewItem container)
    {
        TreeViewItem itemThatMightBelongToContainer = (TreeViewItem)treeView.ItemContainerGenerator.ItemFromContainer(container);
        if (itemThatMightBelongToContainer != null)
            return itemThatMightBelongToContainer;
        else
            return ItemFromContainer(treeView.ItemContainerGenerator, treeView.Items, container);
    }

    private static object ItemFromContainer(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, TreeViewItem container)
    {
        foreach (object curChildItem in itemCollection)
        {
            TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
            TreeViewItem itemThatMightBelongToContainer = (TreeViewItem)parentContainer.ItemContainerGenerator.ItemFromContainer(container);
            if (itemThatMightBelongToContainer != null)
                return itemThatMightBelongToContainer;
            TreeViewItem recursionResult = ItemFromContainer(parentContainer.ItemContainerGenerator, parentContainer.Items, container) as TreeViewItem;
            if (recursionResult != null)
                return recursionResult;
        }
        return null;
    }
}