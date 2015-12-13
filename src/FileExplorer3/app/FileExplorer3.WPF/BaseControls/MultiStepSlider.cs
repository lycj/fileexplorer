using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Data;
using QuickZip.Converters;
using System.Threading;
using FileExplorer.WPF.Utils;

namespace FileExplorer.WPF.BaseControls
{
    public class Step : IComparable
    {
        /// <summary>
        /// Position inside the embeddedSlider, which is either 0..100 or 0..ActualHeight
        /// </summary>
        public double Posision { get; private set; }
        /// <summary>
        /// Represented value
        /// </summary>
        public double Value { get; private set; }
        /// <summary>
        /// Whether add a step stop there
        /// </summary>
        public bool StepStop { get; private set; }
        public Step(double position, double value, bool stepStop)
        {
            Posision = position;
            Value = value;
            StepStop = stepStop;
        }
        public override string ToString()
        {
            return String.Format("P={0}, V={1}, SS={2}", Posision, Value, StepStop);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is Step)
                return Posision.CompareTo(obj as Step);
            return 0;
        }

        #endregion
    }

    public enum PositionType { ptRelative, ptPercent }

    public class MultiStepSlider : Control
    {
        static MultiStepSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiStepSlider), new FrameworkPropertyMetadata(typeof(MultiStepSlider)));
        }

        public MultiStepSlider()
        {
            _valueToSliderValueConverter = new DynamicConverter<double, double>(
                x => valueToSliderValue(x),
                x => sliderValueToValue(x));

            //_valueToSliderValueConverter = new DynamicConverter<double, double>(
            //   x => Math.Round(valueToSliderValue(x)),
            //   x => Math.Round(sliderValueToValue(x)));

            //_valueToSliderValueConverter = new DynamicConverter<double, double>(
            //  x => x,
            //  x => x);
        }

        #region Methods
        public Step GetStep(int idx)
        {
            if (idx <= -1)
                return new Step(0, Minimum, false);
            if (idx >= Steps.Count)
                return new Step(PositionType == PositionType.ptPercent ? 100.0d :
                    Orientation == Orientation.Horizontal ? _embeddedSlider.ActualWidth :
                        _embeddedSlider.ActualHeight, Maximum, false);
            else return Steps[idx];
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _embeddedSlider = this.Template.FindName("embeddedSlider", this) as Slider;
            Debug.Assert(_embeddedSlider != null);
            _embeddedSlider.SizeChanged += delegate { UpdateMaximum(); };


        }

        bool isWithinStepStop(int idx, double position)
        {
            if (idx <= -1 || idx >= Steps.Count)
                return false;

            Step curStep = GetStep(idx);

            return (position > curStep.Posision - (SnapFrequency / 2)) &&
                (position < curStep.Posision + (SnapFrequency / 2));
        }

        double sliderValueToValue(double newValue)
        {
            double retVal;
            if (Steps == null)
                return 0;

            int lowStepIdx = -1;                    //-1 = Minimum
            int highStepIdx = Steps.Count;          //Steps.Count = Maximum
            for (int i = -1; i <= Steps.Count; i++)
            {
                if (GetStep(i).Posision <= newValue)
                    lowStepIdx = i;
            }

            for (int i = Steps.Count; i >= -1; i--)
            {
                if (GetStep(i).Posision >= newValue)
                    highStepIdx = i;
            }

            if (lowStepIdx == highStepIdx)
                return GetStep(lowStepIdx).Value;

            Step lowStep = GetStep(lowStepIdx);
            Step highStep = GetStep(highStepIdx);
            if (lowStep.Value > highStep.Value)
            {
                var temp = lowStep; lowStep = highStep; highStep = lowStep; //Swap
            }
            while (highStep.Value == lowStep.Value && lowStep.Value != Minimum)
                lowStep = GetStep(--lowStepIdx);

            double ratio = (highStep.Value - lowStep.Value) / (highStep.Posision - lowStep.Posision);
            if (double.IsNaN(ratio))
                ratio = 0;
            double position = newValue;

            //if (lowStep.StepStop && isWithinStepStop(lowStepIdx, position))
            //    retVal = lowStep.Value;
            //else
            //    if (highStep.StepStop && isWithinStepStop(highStepIdx, position))
            //        retVal = highStep.Value;
            //    else
            retVal = (ratio * (position - lowStep.Posision)) + lowStep.Value;

            Debug.WriteLine(String.Format("s2v: {0} -> {1}", newValue, retVal));
            return retVal;
        }



        double valueToSliderValue(double newValue)
        {
            double retVal;

            if (Steps == null)
                return 0;

            if (newValue == 0)
                return 0.0d;

            int lowStepIdx = -1;                        //-1 = Minimum
            int highStepIdx = Steps.Count;          //Steps.Count = Maximum
            for (int i = -1; i < Steps.Count; i++)
            {
                if (GetStep(i).Value <= newValue)
                    lowStepIdx = i;
            }

            for (int i = Steps.Count; i > -1; i--)
            {
                if (GetStep(i).Value >= newValue)
                    highStepIdx = i;
            }

            Step lowStep = GetStep(lowStepIdx);
            Step highStep = GetStep(highStepIdx);
            if (Steps.Count != 0)
                while (highStep.Posision == lowStep.Posision && lowStep.Value != newValue)
                    lowStep = GetStep(--lowStepIdx);

            double ratio = (highStep.Value - lowStep.Value) / (highStep.Posision - lowStep.Posision);
            //if (ratio > 0.9)
            //    ratio = 1;
            double step = newValue;
            if (double.IsInfinity(ratio))
                return Minimum;

            if (lowStep.Value == newValue)
                retVal = lowStep.Posision;
            else
                if (highStep.Value == newValue)
                    retVal = highStep.Posision;
                else
                    retVal = ((step - lowStep.Value) / ratio) + lowStep.Posision;

            Debug.WriteLine(String.Format("v2s: {0} -> {1}", newValue, retVal));
            return retVal;
        }

        public static void OnValueChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Write("MultiStepSlider" +  e.NewValue.ToString());
            ////Value ---> Position

            //MultiStepSlider mss = (MultiStepSlider)sender;
            //mss.valueToSliderValue((double)e.NewValue);
        }

        public void UpdateMaximum()
        {
            if (_embeddedSlider != null)
            {
                double origValue = Value;
                if (PositionType == PositionType.ptPercent)
                    _embeddedSlider.Maximum = 100.0d;
                else
                {
                    //_embeddedSlider.FindName("Template.FindName
                    if (Orientation == Orientation.Horizontal)

                        _embeddedSlider.Maximum = _embeddedSlider.ActualWidth;
                    else _embeddedSlider.Maximum = _embeddedSlider.ActualHeight;
                }
                SnapFrequency = _embeddedSlider.Maximum * 0.05;

                //Value = -1; //Reset the slider                
                Value = origValue;
            }
        }


        public void AddStep(double position, double value, bool stepStop)
        {
            Steps.Add(new Step(position, value, stepStop));
            _embeddedSlider.Ticks.Add(position);
        }

        public void AddStep(double position, double value)
        {
            AddStep(position, value, false);
        }

        public void ClearStep(double position, double value)
        {
            Steps.Clear();
            _embeddedSlider.Ticks.Clear();
        }

        private void setupBindings()
        {
            //_embeddedSlider.SetValue(Slider.ValueProperty, valueToSliderValue(this.Value));
            //Binding sliderValueBinding = new Binding("Value");
            //sliderValueBinding.Source = this;
            //sliderValueBinding.Mode = BindingMode.TwoWay;
            ////sliderValueBinding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;

            //sliderValueBinding.Converter = _valueToSliderValueConverter;            
            //_embeddedSlider.SetBinding(Slider.ValueProperty, sliderValueBinding);

            UpdateMaximum();

            _embeddedSlider.AddValueChanged(Slider.ValueProperty, (o, e) =>
                {
                    if (_embeddedSlider.IsFocused)
                    {
                        Value = sliderValueToValue(_embeddedSlider.Value);
                    }
                    //Value = (double)_valueToSliderValueConverter.ConvertBack(
                    //    _embeddedSlider.Value, typeof(double), null, 
                    //    Thread.CurrentThread.CurrentCulture);
                    ////_embeddedSlider.GetBindingExpression(Slider.ValueProperty).UpdateTarget();
                });

            this.AddValueChanged(ValueProperty, (o, e) =>
            {
                if (!_embeddedSlider.IsFocused)
                    _embeddedSlider.SetValue(Slider.ValueProperty, valueToSliderValue(this.Value));
            });

            _embeddedSlider.Value = valueToSliderValue(Value);
        }


        public static void OnStepsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MultiStepSlider mss = (MultiStepSlider)sender;

            if (mss._embeddedSlider != null)
            {
                //double origValue = mss.sliderValueToValue(mss._embeddedSlider.Value);
                mss._embeddedSlider.Ticks.Clear();
                foreach (Step step in ((ObservableCollection<Step>)e.NewValue))
                    mss._embeddedSlider.Ticks.Add(step.Posision);
                //mss.valueToSliderValue(origValue);
                //mss.Value = origValue;
                mss.setupBindings();
            }
        }

        public static void OnPositionTypeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MultiStepSlider mss = (MultiStepSlider)sender;
            mss.UpdateMaximum();
        }

        public static void OnOrientationChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MultiStepSlider mss = (MultiStepSlider)sender;
            mss.UpdateMaximum();
        }

        public static void OnMinMaxChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //MultiStepSlider mss = (MultiStepSlider)sender;
            //BindingExpression be = mss.GetBindingExpression(MultiStepSlider.ValueProperty);
            //Debug.Write(mss._embeddedSlider.Value.ToString();
            //be.UpdateSource();
            //Debug.WriteLine(mss._embeddedSlider.Value);
        }



        #endregion

        #region Data

        private Slider _embeddedSlider;
        //private bool _updating = false;
        private DynamicConverter<double, double> _valueToSliderValueConverter;

        #endregion


        #region Public Properties


        public static readonly DependencyProperty StepsProperty = DependencyProperty.Register("Steps", typeof(ObservableCollection<Step>), typeof(MultiStepSlider),
            new PropertyMetadata(new ObservableCollection<Step>(), new PropertyChangedCallback(OnStepsChanged)));

        public ObservableCollection<Step> Steps
        {
            get { return (ObservableCollection<Step>)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }

        public static readonly DependencyProperty PositionTypeProperty = DependencyProperty.Register("PositionType", typeof(PositionType), typeof(MultiStepSlider),
            new PropertyMetadata(PositionType.ptPercent, new PropertyChangedCallback(OnPositionTypeChanged)));

        public PositionType PositionType
        {
            get { return (PositionType)GetValue(PositionTypeProperty); }
            set { SetValue(PositionTypeProperty, value); }
        }


        public static readonly DependencyProperty OrientationProperty = Slider.OrientationProperty.AddOwner(typeof(MultiStepSlider),
            new PropertyMetadata(new PropertyChangedCallback(OnOrientationChanged)));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty = Slider.MaximumProperty.AddOwner(typeof(MultiStepSlider),
            new PropertyMetadata(new PropertyChangedCallback(OnMinMaxChanged)));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = Slider.MinimumProperty.AddOwner(typeof(MultiStepSlider),
            new PropertyMetadata(new PropertyChangedCallback(OnMinMaxChanged)));

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = Slider.ValueProperty.AddOwner(typeof(MultiStepSlider),
            new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty SnapFrequencyProperty = DependencyProperty.Register("SnapFrequency",
            typeof(double), typeof(MultiStepSlider), new PropertyMetadata(10.0d));

        public double SnapFrequency
        {
            get { return (double)GetValue(SnapFrequencyProperty); }
            set { SetValue(SnapFrequencyProperty, value); }
        }

        ///// <summary>
        ///// Value of embedded Slider, 0..100
        ///// </summary>
        //internal static readonly DependencyProperty InnerSliderValueProperty = DependencyProperty.Register("InnerSliderValue", 
        //    typeof(double), typeof(MultiStepSlider), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        //internal double InnerSliderValue
        //{
        //    get { return (double)GetValue(InnerSliderValueProperty); }
        //    set { SetValue(InnerSliderValueProperty, value); }
        //}
        #endregion

    }
}
