﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace WinRTXamlToolkit.Controls
{
    public enum NumericUpDownValueBarVisibility
    {
        Visible,
        Collapsed,
        Auto
    }

    public sealed class NumericUpDown : RangeBase
    {
        private UIElement _dragOverlay;
        private TextBox _valueTextBox;
        private RepeatButton _decrementButton;
        private RepeatButton _incrementButton;
        private FrameworkElement _valueBar;
        private bool _isChangingTextWithCode;
        private bool _isChangingValueWithCode;

        private double _unusedManipulationDelta;

        #region ValueFormat
        /// <summary>
        /// ValueFormat Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueFormatProperty =
            DependencyProperty.Register(
                "ValueFormat",
                typeof(string),
                typeof(NumericUpDown),
                new PropertyMetadata("F2", OnValueFormatChanged));

        /// <summary>
        /// Gets or sets the ValueFormat property. This dependency property 
        /// indicates the format of the value string.
        /// </summary>
        public string ValueFormat
        {
            get { return (string)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ValueFormat property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnValueFormatChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (NumericUpDown)d;
            string oldValueFormat = (string)e.OldValue;
            string newValueFormat = target.ValueFormat;
            target.OnValueFormatChanged(oldValueFormat, newValueFormat);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ValueFormat property.
        /// </summary>
        /// <param name="oldValueFormat">The old ValueFormat value</param>
        /// <param name="newValueFormat">The new ValueFormat value</param>
        private void OnValueFormatChanged(
            string oldValueFormat, string newValueFormat)
        {
            UpdateValueText();
        }
        #endregion

        #region ValueBarVisibility
        /// <summary>
        /// ValueBarVisibility Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueBarVisibilityProperty =
            DependencyProperty.Register(
                "ValueBarVisibility",
                typeof(NumericUpDownValueBarVisibility),
                typeof(NumericUpDown),
                new PropertyMetadata(NumericUpDownValueBarVisibility.Auto, OnValueBarVisibilityChanged));

        /// <summary>
        /// Gets or sets the ValueBarVisibility property. This dependency property 
        /// indicates whether the value bar should be shown.
        /// </summary>
        public NumericUpDownValueBarVisibility ValueBarVisibility
        {
            get { return (NumericUpDownValueBarVisibility)GetValue(ValueBarVisibilityProperty); }
            set { SetValue(ValueBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ValueBarVisibility property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnValueBarVisibilityChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (NumericUpDown)d;
            NumericUpDownValueBarVisibility oldValueBarVisibility = (NumericUpDownValueBarVisibility)e.OldValue;
            NumericUpDownValueBarVisibility newValueBarVisibility = target.ValueBarVisibility;
            target.OnValueBarVisibilityChanged(oldValueBarVisibility, newValueBarVisibility);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ValueBarVisibility property.
        /// </summary>
        /// <param name="oldValueBarVisibility">The old ValueBarVisibility value</param>
        /// <param name="newValueBarVisibility">The new ValueBarVisibility value</param>
        private void OnValueBarVisibilityChanged(
            NumericUpDownValueBarVisibility oldValueBarVisibility, NumericUpDownValueBarVisibility newValueBarVisibility)
        {
            UpdateValueBar();
        }
        #endregion

        #region IsReadOnly
        /// <summary>
        /// IsReadOnly Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(NumericUpDown),
                new PropertyMetadata(false, OnIsReadOnlyChanged));

        /// <summary>
        /// Gets or sets the IsReadOnly property. This dependency property 
        /// indicates whether the box should only allow to read the values by copying and pasting them.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Handles changes to the IsReadOnly property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsReadOnlyChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (NumericUpDown)d;
            bool oldIsReadOnly = (bool)e.OldValue;
            bool newIsReadOnly = target.IsReadOnly;
            target.OnIsReadOnlyChanged(oldIsReadOnly, newIsReadOnly);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the IsReadOnly property.
        /// </summary>
        /// <param name="oldIsReadOnly">The old IsReadOnly value</param>
        /// <param name="newIsReadOnly">The new IsReadOnly value</param>
        private void OnIsReadOnlyChanged(
            bool oldIsReadOnly, bool newIsReadOnly)
        {
            UpdateIsReadOnlyDependants();
        }
        #endregion

        public NumericUpDown()
        {
            this.DefaultStyleKey = typeof(NumericUpDown);
        }

        #region OnApplyTemplate()
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _valueTextBox = GetTemplateChild("ValueTextBox") as TextBox;
            _dragOverlay = GetTemplateChild("DragOverlay") as UIElement;
            _decrementButton = GetTemplateChild("DecrementButton") as RepeatButton;
            _incrementButton = GetTemplateChild("IncrementButton") as RepeatButton;
            _valueBar = GetTemplateChild("ValueBar") as FrameworkElement;

            if (_valueTextBox != null)
            {
                _valueTextBox.LostFocus += OnValueTextBoxLostFocus;
                _valueTextBox.GotFocus += OnValueTextBoxGotFocus;
                _valueTextBox.Text = Value.ToString();
                _valueTextBox.TextChanged += OnValueTextBoxTextChanged;
            }

            if (_dragOverlay != null)
            {
                _dragOverlay.Tapped += OnDragOverlayTapped;
                _dragOverlay.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
                _dragOverlay.ManipulationDelta += OnDragOverlayManipulationDelta;
            }

            if (_decrementButton != null)
            {
                _decrementButton.Click += OnDecrementButtonClick;
                var pcc =
                    new PropertyChangeEventSource<bool>
                        (_decrementButton, "IsPressed");
                pcc.ValueChanged += OnDecrementButtonIsPressedChanged;
            }

            if (_incrementButton != null)
            {
                _incrementButton.Click += OnIncrementButtonClick;
                var pcc =
                    new PropertyChangeEventSource<bool>
                        (_incrementButton, "IsPressed");
                pcc.ValueChanged += OnIncrementButtonIsPressedChanged;
            }

            if (_valueBar != null)
            {
                _valueBar.SizeChanged += OnValueBarSizeChanged;
                UpdateValueBar();
            }

            UpdateIsReadOnlyDependants();
        }

        private void OnValueTextBoxTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            if (_isChangingTextWithCode)
                return;

            double val;

            if (double.TryParse(_valueTextBox.Text, NumberStyles.Any, CultureInfo.CurrentUICulture, out val))
            {
                _isChangingValueWithCode = true;
                Value = val;
                _isChangingValueWithCode = false;
            }
        }
        #endregion

        #region Button event handlers
        private void OnDecrementButtonIsPressedChanged(object decrementButton, bool isPressed)
        {
        }

        private void OnDecrementButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Value -= SmallChange;
        }

        private void OnIncrementButtonIsPressedChanged(object incrementButton, bool isPressed)
        {
        }

        private void OnIncrementButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Value += SmallChange;
        }
        #endregion

        private void OnValueTextBoxGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_dragOverlay != null)
            {
                _dragOverlay.IsHitTestVisible = false;
            }
        }

        private void OnValueTextBoxLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_dragOverlay != null)
            {
                _dragOverlay.IsHitTestVisible = true;
                UpdateValueText();
            }
        }

        private void OnDragOverlayManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs manipulationDeltaRoutedEventArgs)
        {
            if (!this.IsEnabled)
                return;

            double delta;

            if (Math.Abs(manipulationDeltaRoutedEventArgs.Delta.Translation.X) >
                Math.Abs(manipulationDeltaRoutedEventArgs.Delta.Translation.Y))
            {
                delta = manipulationDeltaRoutedEventArgs.Delta.Translation.X;
            }
            else
            {
                delta = -manipulationDeltaRoutedEventArgs.Delta.Translation.Y;
            }

            ApplyManipulationDelta(delta);
        }

        private void ApplyManipulationDelta(double delta)
        {
            if (Math.Sign(delta) == Math.Sign(_unusedManipulationDelta))
                _unusedManipulationDelta += delta;
            else
                _unusedManipulationDelta = delta;

            if (Value == Maximum && _unusedManipulationDelta > 0)
            {
                _unusedManipulationDelta = 0;
                return;
            }

            if (Value == Minimum && _unusedManipulationDelta < 0)
            {
                _unusedManipulationDelta = 0;
                return;
            }

            double smallerScreenDimension;

            if (Window.Current != null)
            {
                smallerScreenDimension = Math.Min(Window.Current.Bounds.Width, Window.Current.Bounds.Height);
            }
            else
            {
                smallerScreenDimension = 768;
            }

            if (_unusedManipulationDelta <= 0 && this.Value == this.Minimum)
            {
                _unusedManipulationDelta = 0;
                return;
            }

            if (_unusedManipulationDelta >= 0 && this.Value == this.Maximum)
            {
                _unusedManipulationDelta = 0;
                return;
            }

            var speed = Maximum - Minimum;
            var screenAdjustedDelta = speed * _unusedManipulationDelta / smallerScreenDimension;
            this.Value += screenAdjustedDelta;
            _unusedManipulationDelta = 0;
        }

        private void OnDragOverlayTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            if (this.IsEnabled &&
                _valueTextBox != null)
            {
                _valueTextBox.Focus(FocusState.Programmatic);
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);

            UpdateValueBar();

            if (!_isChangingValueWithCode)
            {
                UpdateValueText();
            }
        }

        private void UpdateValueBar()
        {
            if (_valueBar == null)
                return;

            var effectiveValueBarVisibility = ValueBarVisibility;

            if (effectiveValueBarVisibility == NumericUpDownValueBarVisibility.Auto)
            {
                effectiveValueBarVisibility = NumericUpDownValueBarVisibility.Visible;
            }

            if (effectiveValueBarVisibility == NumericUpDownValueBarVisibility.Collapsed)
            {
                _valueBar.Visibility = Visibility.Collapsed;
                return;
            }

            _valueBar.Clip =
                new RectangleGeometry
                {
                    Rect = new Rect
                    {
                        X = 0,
                        Y = 0,
                        Height = _valueBar.ActualHeight,
                        Width = _valueBar.ActualWidth * (Value - Minimum) / (Maximum - Minimum)
                    }
                };
        }

        private void OnValueBarSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            UpdateValueBar();
        }

        private void UpdateValueText()
        {
            if (_valueTextBox != null)
            {
                _isChangingTextWithCode = true;
                _valueTextBox.Text = Value.ToString(ValueFormat);
                _isChangingTextWithCode = false;
            }
        }

        private void UpdateIsReadOnlyDependants()
        {
            if (_decrementButton != null)
            {
                _decrementButton.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            }

            if (_incrementButton != null)
            {
                _incrementButton.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }

    public sealed class NumericUpDownAccelerationCollection : List<NumericUpDownAcceleration>
    {
        //TODO: Consider adding this
    }

    public sealed class NumericUpDownAcceleration : DependencyObject
    {
        #region Seconds
        /// <summary>
        /// Seconds Dependency Property
        /// </summary>
        public static readonly DependencyProperty SecondsProperty =
            DependencyProperty.Register(
                "Seconds",
                typeof(int),
                typeof(NumericUpDownAcceleration),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the Seconds property. This dependency property 
        /// indicates the number of seconds before the acceleration takes place.
        /// </summary>
        public int Seconds
        {
            get { return (int)GetValue(SecondsProperty); }
            set { SetValue(SecondsProperty, value); }
        }
        #endregion

        #region Increment
        /// <summary>
        /// Increment Dependency Property
        /// </summary>
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(
                "Increment",
                typeof(double),
                typeof(NumericUpDownAcceleration),
                new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets the Increment property. This dependency property 
        /// indicates the increment that takes place every button click
        /// after the button has been pressed for the duration specified
        /// by the Seconds property.
        /// </summary>
        public double Increment
        {
            get { return (double)GetValue(IncrementProperty); }
            set { SetValue(IncrementProperty, value); }
        }
        #endregion
    }
}
