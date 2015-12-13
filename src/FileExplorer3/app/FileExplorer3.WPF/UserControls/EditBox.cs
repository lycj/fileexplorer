///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// LYCJ (c) 2010 - http://www.quickzip.org/components                                                            //
// Release under LGPL license.                                                                                   //
//                                                                                                               //
// This code used part of ATC Avalon Team's work                                                                 //
// (http://blogs.msdn.com/atc_avalon_team/archive/2006/03/14/550934.aspx)                                        //
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;

namespace FileExplorer.WPF.UserControls
{
    public class EditBox : Control
    {
        #region Constructor

        static EditBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditBox), new FrameworkPropertyMetadata(typeof(EditBox)));
        }

        public EditBox()
        {

        }



        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TextBlock textBlock = GetTemplateChild("PART_DisplayTextBlockPart") as TextBlock;
            Debug.Assert(textBlock != null, "No TextBlock!");
            this.Focusable = false;
            _adorner = new EditBoxAdorner(this, textBlock);

            RoutedEventHandler attachedHandler = null;

            attachedHandler = delegate
            {
                textBlock.Loaded -= attachedHandler;
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(textBlock);
                if (layer != null)
                    layer.Add(_adorner);
            };


            textBlock.Loaded += attachedHandler;

        }



        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (!IsEditing)
            {
                _canBeEdit = true;
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            _canBeEdit = false;
        }

        #endregion

        #region Data
        private EditBoxAdorner _adorner;
        public RoutedUICommand OpenCommand;
        internal bool _canBeEdit = false;     //Whether EditBox can switch into Editable mode (Mouse over EditBox And Not Editing)

        #endregion

        #region Public Properties

        
        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(System.Windows.TextWrapping),
            typeof(EditBox), new FrameworkPropertyMetadata(System.Windows.TextWrapping.Wrap));

        public object TextWrapping
        {
            get { return GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }


        public static readonly DependencyProperty DisplayValueProperty = DependencyProperty.Register("DisplayValue", typeof(object),
            typeof(EditBox), new FrameworkPropertyMetadata());

        public object DisplayValue
        {
            get { return GetValue(DisplayValueProperty); }
            set { SetValue(DisplayValueProperty, value); }
        }

        public static readonly DependencyProperty ActualValueProperty = DependencyProperty.Register("ActualValue", typeof(object),
            typeof(EditBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public object ActualValue
        {
            get { return GetValue(ActualValueProperty); }
            set { SetValue(ActualValueProperty, value); }
        }

        public static DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditBox),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsEditingChanged), new CoerceValueCallback(IsEditingCoerce)));

        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public static void IsEditingChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            EditBox eb = (EditBox)s;
            eb._adorner.UpdateVisibilty((bool)e.NewValue);
        }

        public static object IsEditingCoerce(DependencyObject s, object v)
        {
            EditBox eb = (EditBox)s;
            if (!eb.IsEditable)
                return false;
            return v;
        }


        public static DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(EditBox),
            new FrameworkPropertyMetadata(false));

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        #endregion

    }
}
