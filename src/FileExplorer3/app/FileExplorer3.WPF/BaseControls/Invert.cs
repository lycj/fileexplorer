using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{
    public class Invert : ContentControl
    {
        #region Constructor

        #endregion

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateStates(false);
            this.AddValueChanged(Invert.IsEnabledProperty, (o, e) => UpdateStates(true));
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            UpdateStates(true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            UpdateStates(true);
        }


        private void UpdateStates(bool useTransition)
        {
            if (!IsEnabled)
            {
                VisualStateManager.GoToState(this, "Disabled", useTransition);
                this.SetValue(ForegroundProperty, DisabledForegroundBrush);
                this.SetValue(BackgroundProperty, DisabledBackgroundBrush);
            }
            else
                if (IsMouseOver)
                {
                    VisualStateManager.GoToState(this, "MouseOver", useTransition);
                    this.SetValue(ForegroundProperty, MouseOverForegroundBrush);
                    this.SetValue(BackgroundProperty, MouseOverBackgroundBrush);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Normal", useTransition);
                    this.SetValue(ForegroundProperty, NormalForegroundBrush);
                    this.SetValue(BackgroundProperty, NormalBackgroundBrush);
                }

        }

        #endregion

        #region Data

        #endregion

        #region Public Properties

        public Brush NormalForegroundBrush
        {
            get { return (Brush)GetValue(NormalForegroundBrushProperty); }
            set { SetValue(NormalForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty NormalForegroundBrushProperty =
            DependencyProperty.Register("NormalForegroundBrush", typeof(Brush),
            typeof(Invert), new UIPropertyMetadata(Brushes.Black));

        public Brush NormalBackgroundBrush
        {
            get { return (Brush)GetValue(NormalBackgroundBrushProperty); }
            set { SetValue(NormalBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty NormalBackgroundBrushProperty =
            DependencyProperty.Register("NormalBackgroundBrush", typeof(Brush),
            typeof(Invert), new UIPropertyMetadata(Brushes.Transparent));


        public Brush MouseOverForegroundBrush
        {
            get { return (Brush)GetValue(MouseOverForegroundBrushProperty); }
            set { SetValue(MouseOverForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty MouseOverForegroundBrushProperty =
            DependencyProperty.Register("MouseOverForegroundBrush", typeof(Brush),
            typeof(Invert), new UIPropertyMetadata(Brushes.Transparent));

        public Brush MouseOverBackgroundBrush
        {
            get { return (Brush)GetValue(MouseOverBackgroundBrushProperty); }
            set { SetValue(MouseOverBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBackgroundBrushProperty =
            DependencyProperty.Register("MouseOverBackgroundBrush", typeof(Brush),
            typeof(Invert), new UIPropertyMetadata(Brushes.Black));



        public Brush DisabledForegroundBrush
        {
            get { return (Brush)GetValue(DisabledForegroundBrushProperty); }
            set { SetValue(DisabledForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty DisabledForegroundBrushProperty =
            DependencyProperty.Register("DisabledForegroundBrush", typeof(Brush),
            typeof(Invert), new UIPropertyMetadata(Brushes.Transparent));

        public Brush DisabledBackgroundBrush
        {
            get { return (Brush)GetValue(DisabledBackgroundBrushProperty); }
            set { SetValue(DisabledBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty DisabledBackgroundBrushProperty =
            DependencyProperty.Register("DisabledBackgroundBrush", typeof(Brush),
            typeof(Invert), new UIPropertyMetadata(Brushes.Gray));

        #endregion
    }
}
