using FileExplorer.Defines;
using FileExplorer.Script;
using FileExplorer.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FileExplorer.UIEventHub
{
    public static partial class HubScriptCommands
    {                        
        /// <summary>
        /// Assumed {FindSelectionMode} is assigned (use DetermineFindSelectionMode())
        /// find selected items and assign to {SelectedList} and {SelectedIdList}
        /// </summary>
        /// <param name="nextCommand"></param>
        /// <returns></returns>
        public static IScriptCommand FindSelectedItems(IScriptCommand nextCommand = null)
        {            
            return 
                HubScriptCommands.DetermineFindSelectionMode("{FindSelectionMode}", 
                ScriptCommands.Switch("{FindSelectionMode}",
                      new Dictionary<FindSelectionMode, IScriptCommand>()
                      {
                          { FindSelectionMode.IChildInfo, 
                              HubScriptCommands.FindSelectedItemsUsingIChildInfo()  },
                          { FindSelectionMode.GridView, 
                              HubScriptCommands.FindSelectedItemsUsingGridView()  }
                      },                      
                      HubScriptCommands.FindSelectedItemsUsingHitTest(),
                      nextCommand));
        }


    }


}
