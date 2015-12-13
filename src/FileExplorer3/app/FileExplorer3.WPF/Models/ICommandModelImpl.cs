using FileExplorer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.WPF.Models
{
    public interface ISliderCommandModel : IDirectoryCommandModel
    {
        int SliderMaximum { get; }
        int SliderMinimum { get; }
        int SliderValue { get; set; }
    }

    public interface ISliderStepCommandModel : ICommandModel
    {
        int SliderStep { get; }
        double? ItemHeight { get; }
        VerticalAlignment VerticalAlignment { get; }
    }
}
