using FileExplorer.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileExplorer.Defines
{

    public class QueryDropEffects
    {
        public QueryDropEffects(DragDropEffectsEx supportedEffects, DragDropEffectsEx preferredEffects)
        {
            SupportedEffects = supportedEffects;
            PreferredEffect = preferredEffects;
        }

        public static QueryDropEffects None = new QueryDropEffects(DragDropEffectsEx.None, DragDropEffectsEx.None);

        public static QueryDropEffects CreateNew(DragDropEffectsEx supportedEffects, DragDropEffectsEx preferredEffects)
        {
            return new QueryDropEffects(supportedEffects, preferredEffects);
        }

        public static QueryDropEffects CreateNew(DragDropEffectsEx supportedEffects)
        {
            return new QueryDropEffects(supportedEffects, supportedEffects);
        }


        public DragDropEffectsEx SupportedEffects { get; set; }
        public DragDropEffectsEx PreferredEffect { get; set; }
    }

  
}
