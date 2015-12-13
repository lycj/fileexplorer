using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FileExplorer.Defines;
using FileExplorer.WPF.Models;
using FileExplorer.WPF.Utils;
using FileExplorer.WPF.Defines;
using FileExplorer.Models;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.ViewModels
{
    //Used by FiListCommandsHelper (IFileListViewModel.ToolbarCommands)
    public class ViewModeCommand : SliderCommandModel
    {
        //ViewModes, name then slider steps.
        public static string[] ViewModes = new string[]
        {
             "ExtraLargeIcon,200,60",
             "LargeIcon,100,60",
             "Icon,65",
             "SmallIcon,60",
             "Separator,-1",
             "List,55",
             "Separator,-1",
             "Grid,50"
        };

        //Given a view mode string (LargeIcon,100,60), parse usable value.
        internal static void parseViewMode(string viewModeStr, out string viewMode, out int step, out int itemHeight)
        {
            string[] vmSplit = viewModeStr.Split(',');
            viewMode = null;
            step = -1;
            itemHeight = 0;
            if (vmSplit.Count() >= 2)
            {
                viewMode = vmSplit[0];
                step = Int32.Parse(vmSplit[1]);
                itemHeight = 0;
                if (vmSplit.Count() >= 3)
                    itemHeight = Int32.Parse(vmSplit[2]);
            }
        }

        //Find ViewMode using a step number  (e.g. 103 return LargeIcon = 2)
        internal static int findViewMode(string[] viewModes, int forStep)
        {
            int retVal = 0; int id = 0;
            var parsedViewMode = viewModes.Select(vm =>
                {
                    string viewMode; int step; int itemHeight;
                    parseViewMode(vm, out viewMode, out step, out itemHeight);
                    return new { Id = id++, ViewMode = viewMode, Step = step, ItemHeight = itemHeight, 
                    StepDifference = Math.Abs(step - forStep) };
                });

            return parsedViewMode.MinBy(vmInfo => vmInfo.StepDifference).Id;

            for (int i = viewModes.Count() - 1; i >= 0; i--)
            {
                string viewMode; int step; int itemHeight;
                parseViewMode(viewModes[i], out viewMode, out step, out itemHeight);
                if (step != -1)
                    if (forStep >= step)
                    {
                        retVal = i;
                    }
                    else break;
            }
            return retVal;
        }



        private class ViewModeStepCommandModel : SliderStepCommandModel
        {
            public ViewModeStepCommandModel(string view)
            {
                IsVisibleOnMenu = true;
                IsVisibleOnToolbar = true; 
                Header = view;
                Stream imgStream = Application.GetResourceStream(
                       new Uri(String.Format(ViewModeViewModel.iconPathMask, view.ToLower()))).Stream;
                if (imgStream != null)
                    HeaderIconExtractor = 
                        ModelIconExtractor<ICommandModel>.FromStream(imgStream);
            }
        }

        private static IEnumerable<ICommandModel> generateCommandModel()
        {
            foreach (string vm in ViewModes)
            {
                string viewMode; int step; int itemHeight;
                parseViewMode(vm, out viewMode, out step, out itemHeight);

                if (viewMode != null)
                {
                    if (step == -1)
                        yield return new SeparatorCommandModel();
                    else
                    {
                        var scm = new ViewModeStepCommandModel(viewMode) { SliderStep = step };
                        if (itemHeight != 0)
                            scm.ItemHeight = itemHeight;
                        yield return scm;
                    }
                }
            }

        }

        private IFileListViewModel _flvm;
        public ViewModeCommand(IFileListViewModel flvm)
            : base(ExplorerCommands.ToggleViewMode,
                 generateCommandModel().ToArray()
            )
        {
            IsVisibleOnMenu = true; IsVisibleOnToolbar = true;
            _flvm = flvm;
            IsHeaderVisible = false;
            SliderValue = flvm.Parameters.ItemSize;
        }

        internal static void updateViewMode(IFileListViewModel flvm, string viewMode, int step)
        {
            flvm.Parameters.ItemSize = step;
            switch (viewMode)
            {
                case "ExtraLargeIcon":
                case "LargeIcon":
                    flvm.Parameters.ViewMode = "Icon";
                    break;
                //case "Grid":
                //    AsyncUtils.RunSync(() => flvm.ProcessedEntries.EntriesHelper.UnloadAsync());
                //    flvm.ViewMode = viewMode;
                //    break;
                default:
                    flvm.Parameters.ViewMode = viewMode;
                    break;
            }
        }


        public override void NotifyOfPropertyChange(string propertyName = "")
        {
            base.NotifyOfPropertyChange(propertyName);
            switch (propertyName)
            {
                case "SliderValue":

                    int curIdx = findViewMode(ViewModes, SliderValue);
                    string viewMode; int step; int itemHeight;
                    parseViewMode(ViewModes[curIdx], out viewMode, out step, out itemHeight);
                    ViewModeStepCommandModel commandModel = SubCommands
                        .Where(c => c is ViewModeStepCommandModel)
                        .First(c => (c as ViewModeStepCommandModel).SliderStep == step) as ViewModeStepCommandModel;

                    if (commandModel != null)
                        this.HeaderIconExtractor = commandModel.HeaderIconExtractor;

                    if (_flvm.Parameters.ItemSize != SliderValue)
                    {
                        updateViewMode(_flvm, commandModel.Header, SliderValue);
                        //Debug.WriteLine(commandModel.Header + SliderValue.ToString());                            
                    }

                    break;
            }
        }
    }


    public class SelectGroupCommand : DirectoryCommandModel
    {
        public SelectGroupCommand(IFileListViewModel flvm)
            : base(ApplicationCommands.SelectAll,
            new CommandModel(ApplicationCommands.SelectAll) { 
                //Symbol = Convert.ToChar(0xE14E),
                HeaderIconExtractor = ResourceIconExtractor<ICommandModel>.ForSymbol(0xE14E),
                IsVisibleOnMenu = true, IsVisibleOnToolbar = true },
            new CommandModel(ExplorerCommands.ToggleCheckBox) { 
                //Symbol = Convert.ToChar(0xe1ef), 
                HeaderIconExtractor = ResourceIconExtractor<ICommandModel>.ForSymbol(0xE1EF),
                IsVisibleOnMenu = true, IsVisibleOnToolbar = true })
        {
            IsVisibleOnMenu = true; IsVisibleOnToolbar = true;
            //Symbol = Convert.ToChar(0xE10B);
            HeaderIconExtractor = ResourceIconExtractor<ICommandModel>.ForSymbol(0xE10B);
            Header = Strings.txtSelection;
        }
    }


}
