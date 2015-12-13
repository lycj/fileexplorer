using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Collections;
using System.Diagnostics;

namespace QuickZip.UserControls
{
    public class SimpleAutoCompleteTextBox : AutoCompleteTextBoxBase
    {
        #region Constructor

        public SimpleAutoCompleteTextBox()
            : base()
        {
            base.Suggestions.Add(new Suggestion("DUMMY"));
        }

        #endregion

        #region Methods

        void updateSearchSource()
        {
            if (this.GetBindingExpression(SimpleAutoCompleteTextBox.SearchTextProperty) != null)
                this.GetBindingExpression(SimpleAutoCompleteTextBox.SearchTextProperty).UpdateSource();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            //base.OnTextChanged(e);
            if (_loaded && Visibility == System.Windows.Visibility.Visible)
            {
                SetValue(SearchTextProperty, Text);
                updateSearchSource();
            }
        }

        #endregion

        #region Data

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

           
        }

        public static void OnHasSuggestionsChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            SimpleAutoCompleteTextBox tbox = (SimpleAutoCompleteTextBox)sender;            

            tbox.IsPopupOpened = (
                tbox.HasSuggestions &&
                tbox.Visibility == Visibility.Visible &&
                tbox.Suggestions != null && tbox.Suggestions.Count > 0);
        }

        #endregion

        #region Public Properties

        public new IList Suggestions
        {
            get { return (IList)GetValue(SuggestionsProperty); }
            set { SetValue(SuggestionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Suggestions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register("Suggestions", typeof(IList), 
            typeof(SimpleAutoCompleteTextBox), 
            new UIPropertyMetadata(null));



        public bool HasSuggestions
        {
            get { return (bool)GetValue(HasSuggestionsProperty); }
            set { SetValue(HasSuggestionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasSuggestions.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasSuggestionsProperty =
            DependencyProperty.Register("HasSuggestions", typeof(bool), typeof(SimpleAutoCompleteTextBox),
            new UIPropertyMetadata(false, OnHasSuggestionsChanged));

        


        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SimpleAutoCompleteTextBox), 
            new UIPropertyMetadata());



        #endregion

    }
}
