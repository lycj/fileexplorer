using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.WPF
{
    public enum LayoutType {  FixedStack, VariableStack, FixedWrap, HalfVariableWrap }

    public enum OffsetType {  Fixed, Relative }
    public enum LayoutState {  Invalidated, Measured, Arranged }
}
