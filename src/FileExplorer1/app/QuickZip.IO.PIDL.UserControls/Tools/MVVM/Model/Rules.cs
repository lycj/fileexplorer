using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinch;

namespace QuickZip.IO.PIDL.UserControls.Model
{
    public static class Rules
    {
        public static SimpleRule NotEmptyRule = new SimpleRule("DataValue", "Value cannot be empty",
                      (Object domainObject)=>
                      {
                          DataWrapper<string> obj = (DataWrapper<string>)domainObject;
                          return !string.IsNullOrEmpty(obj.DataValue);
                      });

    }
}
