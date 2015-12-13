using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using QuickZip.MiniHtml2;
using System.IO;

namespace QuickZip.UserControls.HtmlTextBlock
{
    public class HtmlTextBlock : TextBlock 
    {
        public static DependencyProperty HtmlProperty = DependencyProperty.Register("Html", typeof(string),
                typeof(HtmlTextBlock), new UIPropertyMetadata("Html", new PropertyChangedCallback(OnHtmlChanged)));

        public string Html { get { return (string)GetValue(HtmlProperty); } set { SetValue(HtmlProperty, value); } }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Parse(Html);
        }

        private void Parse(string html)
        {
            Inlines.Clear();
            HtmlTagTree tree = new HtmlTagTree();
            HtmlParser parser = new HtmlParser(tree); //output
            parser.Parse(new StringReader(html));     //input

            HtmlUpdater updater = new HtmlUpdater(this); //output
            updater.Update(tree);   
        }

        public static void OnHtmlChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            HtmlTextBlock sender = (HtmlTextBlock)s;
            sender.Parse((string)e.NewValue);		
        }

        public HtmlTextBlock()
        {       
            
        }

    }
}
