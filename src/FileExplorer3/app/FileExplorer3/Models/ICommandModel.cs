using FileExplorer.Script;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace FileExplorer.Models
{
    /// <summary>
    /// Model for a Command, which is shown as ToolbarExItem in FileExplorer.
    /// </summary>
    public interface ICommandModel : IComparable<ICommandModel>, IComparable, INotifyPropertyChanged
    {        
        IScriptCommand Command { get; set; }

        /// <summary>
        /// Based on Segoe UI Symbol Icons.
        /// See - http://www.adamdawes.com/windows8/win8_segoeuisymbol.html
        /// </summary>
        char? Symbol { get; }


        IModelIconExtractor<ICommandModel> HeaderIconExtractor { get; }

        string ToolTip { get; }

        string Header { get; }

        /// <summary>
        /// Used by IComparable, Lowert priority show up earlier.
        /// </summary>
        int Priority { get; }

        bool IsChecked { get; }

        bool IsEnabled { get; }

        bool IsHeaderVisible { get; }
        bool IsHeaderAlignRight { get; }

        bool IsVisibleOnToolbar { get; }
        bool IsVisibleOnMenu { get; }

        void NotifySelectionChanged(IEntryModel[] appliedModels);
    }

    public interface ISeparatorModel : ICommandModel
    {        
    }

    public interface IDirectoryCommandModel : ICommandModel
    {
        List<ICommandModel> SubCommands { get; }
    }

    public interface ISelectorCommandModel : IDirectoryCommandModel
    {
        bool IsComboBox { get; }
    }

   
    
}
