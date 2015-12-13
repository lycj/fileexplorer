using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

namespace Tomers.WPF.Localization
{
    public class TranslatableRoutedUICommand : RoutedUICommand
    {
        public TranslatableRoutedUICommand(string text, string name, Type ownertype)
            : base(text, name, ownertype)
        {
            string keyName = "command" + Char.ToUpper(name[0]) + name.Substring(1);
            try
            {
                if (text != "")
                    Text = (string)LanguageContext.Instance.Dictionary.Translate(keyName, "Value", text, typeof(string));
            }
            catch
            {

            }
        }

    }
}