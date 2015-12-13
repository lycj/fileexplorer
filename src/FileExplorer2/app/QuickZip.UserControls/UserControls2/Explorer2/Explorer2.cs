using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;

namespace QuickZip.UserControls
{
    [TemplatePart(Name = "PART_Breadcrumb", Type = typeof(Breadcrumb2))]
    [TemplatePart(Name = "PART_DirectoryTree", Type = typeof(TreeView))]
    [TemplatePart(Name = "PART_FileList", Type = typeof(FileList2))]
    [TemplatePart(Name = "PART_WebBrowser", Type = typeof(BindableWebBrowser))]
    [TemplatePart(Name = "PART_Navigator", Type = typeof(Navigator2))]
    [TemplatePart(Name = "PART_Previewer", Type = typeof(MediaPlayer2))]
    public class Explorer2 : Control
    {

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _breadcrumb = (Breadcrumb2)this.Template.FindName("PART_Breadcrumb", this);
            _dTree = (TreeView)this.Template.FindName("PART_DirectoryTree", this);
            _flist = (FileList2)this.Template.FindName("PART_FileList", this);
            _navigator = (Navigator2)this.Template.FindName("PART_Navigator", this);
            _previewer = (MediaPlayer2)this.Template.FindName("PART_Previewer", this);

            if (_navigator != null)
            {
                if (_navigator.GoBackCommand != null)
                    this.InputBindings.Add(new InputBinding(_navigator.GoBackCommand, new ExtraMouseGesture(ExtraMouseAction.Back)));
                if (_navigator.GoNextCommand != null)
                    this.InputBindings.Add(new InputBinding(_navigator.GoNextCommand, new ExtraMouseGesture(ExtraMouseAction.Next)));
            }
            this.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(
                (RoutedEventHandler)delegate(object obj, RoutedEventArgs args)
                {
                    if (args.OriginalSource is TreeViewItem)
                        (args.OriginalSource as TreeViewItem).BringIntoView();

                }));

            //updateMediaPlayerBindings();
        }

        //void updateMediaPlayerBindings()
        //{
        //    if (EntryFullNamePath != null && _mediaPlayer != null)
        //    {
        //        Binding mpBinding = new Binding(EntryFullNamePath);
        //        mpBinding.Source = DataContext;
        //        mpBinding.Mode = BindingMode.OneWay;
        //        _mediaPlayer.SetBinding(BasicMediaPlayer.SourceProperty, mpBinding);
        //    }
        //}

        #region Data

        Breadcrumb2 _breadcrumb;
        FileList2 _flist;
        TreeView _dTree;
        Navigator2 _navigator;
        MediaPlayer2 _previewer;

        #endregion

        #region Public Properties



        //public string EntryFullNamePath
        //{
        //    get { return (string)GetValue(EntryFullNamePathProperty); }
        //    set { SetValue(EntryFullNamePathProperty, value); }
        //}
        
        //public static readonly DependencyProperty EntryFullNamePathProperty =
        //    DependencyProperty.Register("EntryFullNamePath", typeof(string),
        //    typeof(Explorer2), new UIPropertyMetadata("CurrentBrowserViewModel.EmbeddedFile.FullName", new PropertyChangedCallback((o, e) => { (o as Explorer2).updateMediaPlayerBindings(); })));

        

        #endregion
    }
}
