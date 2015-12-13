///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using vbAccelerator.Components.ImageList;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using System.Windows.Resources;
using QuickZip.Tools;

namespace QuickZip.Converters
{   
    //public class LambdaIconConverter : IconConverterBase
    //{
    //    public struct KeyInfo
    //    {
    //        public string Key;
    //        public string FastKey;
    //        public bool DelayLoading;
    //    }

    //    public LambdaIconConverter(Func<object, KeyInfo> valueToKey,
    //        FuncEx<string, IconConverterBase.IconSize, Bitmap> keyToBitmap)
    //    {
    //        _valueToKey = valueToKey;
    //        _keyToBitmap = keyToBitmap;
    //    }

    //    protected override Bitmap KeyToBitmap(string key, IconConverterBase.IconSize size)
    //    {
    //        return _keyToBitmap(key, size);
    //    }

    //    protected override void ValueToKey(object value, out string key, out string fastKey, out bool delayLoading)
    //    {
    //        KeyInfo ki = _valueToKey(value);            
    //        key = ki.Key;
    //        fastKey = ki.FastKey;
    //        delayLoading = ki.DelayLoading;
    //    }

    //    private Func<object, KeyInfo> _valueToKey;
    //    private FuncEx<string, IconConverterBase.IconSize, Bitmap> _keyToBitmap;
    //}
}
