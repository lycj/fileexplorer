using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer
{
    public class MemoryParameterDicStore  : Dictionary<string, object>, IParameterDicStore
    {
        public MemoryParameterDicStore(params ParameterPair[] ppairs)
            : base(StringComparer.CurrentCultureIgnoreCase)
        {
            foreach (var ppair in ppairs)
                this.Add( ppair.Key, ppair.Value);            
        }

        public virtual Task SaveAsync()
        {
            return Task.Delay(0);
        }

        public virtual Task LoadAsync()
        {
            return Task.Delay(0);
        }

        public IParameterDicStore Clone()
        {
            return new MemoryParameterDicStore(this.Keys.Select(k => ParameterPair.FromKey(k, this[k])).ToArray());
        }       
    }
}
