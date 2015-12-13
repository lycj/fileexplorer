using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace QuickZip.Logic
{
    /// <summary>
    /// Attached property that can be used to activate a model.
    /// </summary>
    public static class ActivateModel
    {
        public static readonly DependencyProperty ModelProperty
           = DependencyProperty.RegisterAttached("Model", typeof(DataModel), typeof(ActivateModel),
                new PropertyMetadata(new PropertyChangedCallback(OnModelInvalidated)));

        public static DataModel GetModel(DependencyObject sender)
        {
            return (DataModel)sender.GetValue(ModelProperty);
        }

        public static void SetModel(DependencyObject sender, DataModel model)
        {
            sender.SetValue(ModelProperty, model);
        }

        /// <summary>
        /// Callback when the Model property is set or changed.
        /// </summary>
        private static void OnModelInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)dependencyObject;

            // Add handlers if necessary
            if (e.OldValue == null && e.NewValue != null)
            {
                element.Loaded += OnElementLoaded;
                element.Unloaded += OnElementUnloaded;
            }

            // Or, remove if necessary
            if (e.OldValue != null && e.NewValue == null)
            {
                element.Loaded -= OnElementLoaded;
                element.Unloaded -= OnElementUnloaded;
            }

            // If loaded, deactivate old model and activate new one
            if (element.IsLoaded)
            {
                if (e.OldValue != null)
                {
                    ((DataModel)e.OldValue).Deactivate();
                }

                if (e.NewValue != null)
                {
                    ((DataModel)e.NewValue).Activate();
                }
            }
        }
 
        /// <summary>
        /// Activate the model when the element is loaded.
        /// </summary>
        static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            DataModel model = GetModel(element);
            model.Activate();
        }

        /// <summary>
        /// Deactivate the model when the element is unloaded.
        /// </summary>
        static void OnElementUnloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            DataModel model = GetModel(element);
            model.Deactivate();
        }

    }
}


