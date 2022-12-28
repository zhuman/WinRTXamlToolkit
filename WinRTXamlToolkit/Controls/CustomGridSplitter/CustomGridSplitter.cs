using System;
using System.ComponentModel;
using WinRTXamlToolkit.Controls.Extensions;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace WinRTXamlToolkit.Controls
{
    /// <summary>
    /// Specifies the rows or columns that are resized
    /// by a WinRTXamlToolkit.Controls.CustomGridSplitter control.
    /// </summary>
    public enum GridResizeBehavior
    {
        /// <summary>
        /// Space is redistributed based on the value of the
        /// Microsoft.UI.Xaml.FrameworkElement.HorizontalAlignment and
        /// Microsoft.UI.Xaml.FrameworkElement.VerticalAlignment properties.
        /// </summary>
        BasedOnAlignment = 0,
        /// <summary>
        /// For a horizontal WinRTXamlToolkit.Controls.CustomGridSplitter, space is redistributed
        /// between the row that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        /// and the next row that is below it. For a vertical WinRTXamlToolkit.Controls.CustomGridSplitter,
        /// space is redistributed between the column that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        /// and the next column that is to the right.
        /// </summary>
        CurrentAndNext = 1,
        /// <summary>
        /// For a horizontal WinRTXamlToolkit.Controls.CustomGridSplitter, space is redistributed
        /// between the row that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        /// and the next row that is above it. For a vertical WinRTXamlToolkit.Controls.CustomGridSplitter,
        /// space is redistributed between the column that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter
        /// and the next column that is to the left.
        /// </summary>
        PreviousAndCurrent = 2,
        /// <summary>
        /// For a horizontal WinRTXamlToolkit.Controls.CustomGridSplitter, space is redistributed
        /// between the rows that are above and below the row that is specified for the
        /// WinRTXamlToolkit.Controls.CustomGridSplitter. For a vertical WinRTXamlToolkit.Controls.CustomGridSplitter,
        /// space is redistributed between the columns that are to the left and right
        /// of the column that is specified for the WinRTXamlToolkit.Controls.CustomGridSplitter.
        /// </summary>
        PreviousAndNext = 3,
    }

    /// <summary>
    /// Specifies whether a WinRTXamlToolkit.Controls.CustomGridSplitter control redistributes
    /// space between rows or between columns.
    /// </summary>
    public enum GridResizeDirection
    {
        /// <summary>
        /// Space is redistributed based on the values of the Microsoft.UI.Xaml.FrameworkElement.HorizontalAlignment,
        /// Microsoft.UI.Xaml.FrameworkElement.VerticalAlignment, Microsoft.UI.Xaml.FrameworkElement.ActualWidth,
        /// and Microsoft.UI.Xaml.FrameworkElement.ActualHeight properties of the WinRTXamlToolkit.Controls.CustomGridSplitter.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Space is redistributed between columns.
        /// </summary>
        Columns = 1,
        /// <summary>
        /// Space is redistributed between rows.
        /// </summary>
        Rows = 2,
    }

    /// <summary>
    /// A control similar to the GridSplitter seen in WPF and Sivlerlight.
    /// </summary>
    [TemplateVisualState(GroupName = OrientationStatesGroupName, Name = VerticalOrientationStateName)]
    [TemplateVisualState(GroupName = OrientationStatesGroupName, Name = HorizontalOrientationStateName)]
    [TemplateVisualState(GroupName = FocusStatesGroupName, Name = FocusedStateName)]
    [TemplateVisualState(GroupName = FocusStatesGroupName, Name = UnfocusedStateName)]
    [StyleTypedProperty(Property = "PreviewStyle", StyleTargetType = typeof(GridSplitterPreviewControl))]
    public class CustomGridSplitter : Control
    {
        #region Template Part and Visual State names
        private const string OrientationStatesGroupName = "OrientationStates";
        private const string VerticalOrientationStateName = "VerticalOrientation";
        private const string HorizontalOrientationStateName = "HorizontalOrientation";
        private const string FocusStatesGroupName = "FocusStates";
        private const string FocusedStateName = "Focused";
        private const string UnfocusedStateName = "Unfocused";
        #endregion

        private const double DefaultKeyboardIncrement = 1d;
        private Point _lastPosition;
        private Point _previewDraggingStartPosition;
        private bool _isDragging;
        private bool _isDraggingPreview;
        private GridResizeDirection _effectiveResizeDirection;
        private Grid _parentGrid;

        private Grid _previewPopupHostGrid;
        private Popup _previewPopup;
        private Grid _previewGrid;
        private CustomGridSplitter _previewGridSplitter;
        private Border _previewControlBorder;
        private GridSplitterPreviewControl _previewControl;

        /// <summary>
        /// Occurs when dragging completes.
        /// </summary>
        public event EventHandler DraggingCompleted;

        #region ResizeBehavior
        /// <summary>
        /// ResizeBehavior Dependency Property
        /// </summary>
        public static readonly DependencyProperty ResizeBehaviorProperty =
            DependencyProperty.Register(
                "ResizeBehavior",
                typeof(GridResizeBehavior),
                typeof(CustomGridSplitter),
                new PropertyMetadata(GridResizeBehavior.BasedOnAlignment));

        /// <summary>
        /// Gets or sets the ResizeBehavior property. This dependency property 
        /// indicates which columns or rows are resized relative
        /// to the column or row for which the GridSplitter control is defined.
        /// </summary>
        public GridResizeBehavior ResizeBehavior
        {
            get { return (GridResizeBehavior)GetValue(ResizeBehaviorProperty); }
            set { SetValue(ResizeBehaviorProperty, value); }
        }
        #endregion

        #region ResizeDirection
        /// <summary>
        /// ResizeDirection Dependency Property
        /// </summary>
        public static readonly DependencyProperty ResizeDirectionProperty =
            DependencyProperty.Register(
                "ResizeDirection",
                typeof(GridResizeDirection),
                typeof(CustomGridSplitter),
                new PropertyMetadata(GridResizeDirection.Auto, OnResizeDirectionChanged));

        /// <summary>
        /// Gets or sets the ResizeDirection property. This dependency property 
        /// indicates whether the CustomGridSplitter control resizes rows or columns.
        /// </summary>
        public GridResizeDirection ResizeDirection
        {
            get { return (GridResizeDirection)GetValue(ResizeDirectionProperty); }
            set { SetValue(ResizeDirectionProperty, value); }
        }

        /// <summary>
        /// Handles changes to the ResizeDirection property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnResizeDirectionChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (CustomGridSplitter)d;
            GridResizeDirection oldResizeDirection = (GridResizeDirection)e.OldValue;
            GridResizeDirection newResizeDirection = target.ResizeDirection;
            target.OnResizeDirectionChanged(oldResizeDirection, newResizeDirection);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes
        /// to the ResizeDirection property.
        /// </summary>
        /// <param name="oldResizeDirection">The old ResizeDirection value</param>
        /// <param name="newResizeDirection">The new ResizeDirection value</param>
        protected virtual void OnResizeDirectionChanged(
            GridResizeDirection oldResizeDirection, GridResizeDirection newResizeDirection)
        {
            this.DetermineResizeCursor();
            this.UpdateVisualState();
        }
        #endregion

        #region KeyboardIncrement
        /// <summary>
        /// KeyboardIncrement Dependency Property
        /// </summary>
        public static readonly DependencyProperty KeyboardIncrementProperty =
            DependencyProperty.Register(
                "KeyboardIncrement",
                typeof(double),
                typeof(CustomGridSplitter),
                new PropertyMetadata(DefaultKeyboardIncrement));

        /// <summary>
        /// Gets or sets the KeyboardIncrement property. This dependency property 
        /// indicates the distance that each press of an arrow key moves
        /// a CustomGridSplitter control.
        /// </summary>
        public double KeyboardIncrement
        {
            get { return (double)GetValue(KeyboardIncrementProperty); }
            set { SetValue(KeyboardIncrementProperty, value); }
        }
        #endregion

        #region ShowsPreview
        /// <summary>
        /// ShowsPreview Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowsPreviewProperty =
            DependencyProperty.Register(
                "ShowsPreview",
                typeof(bool),
                typeof(CustomGridSplitter),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the ShowsPreview property. This dependency property
        /// indicates whether the preview control should be shown when dragged
        /// instead of directly updating the grid.
        /// </summary>
        public bool ShowsPreview
        {
            get { return (bool)GetValue(ShowsPreviewProperty); }
            set { SetValue(ShowsPreviewProperty, value); }
        }
        #endregion

        #region PreviewStyle
        /// <summary>
        /// PreviewStyle Dependency Property
        /// </summary>
        public static readonly DependencyProperty PreviewStyleProperty =
            DependencyProperty.Register(
                "PreviewStyle",
                typeof(Style),
                typeof(CustomGridSplitter),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the PreviewStyle property. This dependency property 
        /// indicates the style of the preview.
        /// </summary>
        public Style PreviewStyle
        {
            get { return (Style)GetValue(PreviewStyleProperty); }
            set { SetValue(PreviewStyleProperty, value); }
        }
        #endregion

        #region DetermineEffectiveResizeDirection()
        private GridResizeDirection DetermineEffectiveResizeDirection()
        {
            if (ResizeDirection == GridResizeDirection.Columns)
            {
                return GridResizeDirection.Columns;
            }

            if (ResizeDirection == GridResizeDirection.Rows)
            {
                return GridResizeDirection.Rows;
            }

            // Based on GridResizeDirection Enumeration documentation from
            // http://msdn.microsoft.com/en-us/library/WinRTXamlToolkit.Controls.gridresizedirection(v=VS.110).aspx

            // Space is redistributed based on the values of the HorizontalAlignment, VerticalAlignment, ActualWidth, and ActualHeight properties of the CustomGridSplitter.

            // * If the HorizontalAlignment is not set to Stretch, space is redistributed between columns.
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }

            // * If the HorizontalAlignment is set to Stretch and the VerticalAlignment is not set to Stretch, space is redistributed between rows.
            if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
                this.VerticalAlignment != VerticalAlignment.Stretch)
            {
                return GridResizeDirection.Rows;
            }

            // * If the following conditions are true, space is redistributed between columns:
            //   * The HorizontalAlignment is set to Stretch.
            //   * The VerticalAlignment is set to Stretch.
            //   * The ActualWidth is less than or equal to the ActualHeight.
            if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
                this.VerticalAlignment == VerticalAlignment.Stretch &&
                this.ActualWidth <= this.ActualHeight)
            {
                return GridResizeDirection.Columns;
            }

            // * If the following conditions are true, space is redistributed between rows:
            //   * HorizontalAlignment is set to Stretch.
            //   * VerticalAlignment is set to Stretch.
            //   * ActualWidth is greater than the ActualHeight.
            //if (this.HorizontalAlignment == HorizontalAlignment.Stretch &&
            //    this.VerticalAlignment == VerticalAlignment.Stretch &&
            //    this.ActualWidth > this.ActualHeight)
            {
                return GridResizeDirection.Rows;
            }
        }
        #endregion

        #region DetermineEffectiveResizeBehavior()
        private GridResizeBehavior DetermineEffectiveResizeBehavior()
        {
            if (ResizeBehavior == GridResizeBehavior.CurrentAndNext)
            {
                return GridResizeBehavior.CurrentAndNext;
            }

            if (ResizeBehavior == GridResizeBehavior.PreviousAndCurrent)
            {
                return GridResizeBehavior.PreviousAndCurrent;
            }

            if (ResizeBehavior == GridResizeBehavior.PreviousAndNext)
            {
                return GridResizeBehavior.PreviousAndNext;
            }

            // Based on GridResizeBehavior Enumeration documentation from
            // http://msdn.microsoft.com/en-us/library/WinRTXamlToolkit.Controls.gridresizebehavior(v=VS.110).aspx

            // Space is redistributed based on the value of the
            // HorizontalAlignment and VerticalAlignment properties.

            var effectiveResizeDirection =
                DetermineEffectiveResizeDirection();

            // If the value of the ResizeDirection property specifies
            // that space is redistributed between rows,
            // the redistribution follows these guidelines:

            if (effectiveResizeDirection == GridResizeDirection.Rows)
            {
                // * When the VerticalAlignment property is set to Top,
                //   space is redistributed between the row that is specified
                //   for the GridSplitter and the row that is above that row.
                if (this.VerticalAlignment == VerticalAlignment.Top)
                {
                    return GridResizeBehavior.PreviousAndCurrent;
                }

                // * When the VerticalAlignment property is set to Bottom,
                //   space is redistributed between the row that is specified
                //   for the GridSplitter and the row that is below that row.
                if (this.VerticalAlignment == VerticalAlignment.Bottom)
                {
                    return GridResizeBehavior.CurrentAndNext;
                }

                // * When the VerticalAlignment property is set to Center,
                //   space is redistributed between the row that is above and
                //   the row that is below the row that is specified
                //   for the GridSplitter.
                // * When the VerticalAlignment property is set to Stretch,
                //   space is redistributed between the row that is above
                //   and the row that is below the row that is specified
                //   for the GridSplitter.
                return GridResizeBehavior.PreviousAndNext;
            }

            // If the value of the ResizeDirection property specifies
            // that space is redistributed between columns,
            // the redistribution follows these guidelines:

            // * When the HorizontalAlignment property is set to Left,
            //   space is redistributed between the column that is specified
            //   for the GridSplitter and the column that is to the left.
            if (this.HorizontalAlignment == HorizontalAlignment.Left)
            {
                return GridResizeBehavior.PreviousAndCurrent;
            }

            // * When the HorizontalAlignment property is set to Right,
            //   space is redistributed between the column that is specified
            //   for the GridSplitter and the column that is to the right.
            if (this.HorizontalAlignment == HorizontalAlignment.Right)
            {
                return GridResizeBehavior.CurrentAndNext;
            }

            // * When the HorizontalAlignment property is set to Center,
            //   space is redistributed between the columns that are to the left
            //   and right of the column that is specified for the GridSplitter.
            // * When the HorizontalAlignment property is set to Stretch,
            //   space is redistributed between the columns that are to the left
            //   and right of the column that is specified for the GridSplitter.
            return GridResizeBehavior.PreviousAndNext;
        }
        #endregion

        #region DetermineResizeCursor()
        private void DetermineResizeCursor()
        {
            var effectiveResizeDirection =
                this.DetermineEffectiveResizeDirection();

            if (effectiveResizeDirection == GridResizeDirection.Columns)
            {
                FrameworkElementExtensions.SetCursor(this, new CoreCursor(CoreCursorType.SizeWestEast, 1));
            }
            else
            {
                FrameworkElementExtensions.SetCursor(this, new CoreCursor(CoreCursorType.SizeNorthSouth, 1));
            }
        }
        #endregion

        #region CTOR - CustomGridSplitter()
        // The below code throws for some reason, so the focus properties 
        // for keyboard support had to be moved to the constructor.
        // 
        //static CustomGridSplitter()
        //{
        //    FocusableProperty.OverrideMetadata(
        //        typeof(CustomGridSplitter),
        //        new UIPropertyMetadata(true));
        //    IsTabStopProperty.OverrideMetadata(
        //        typeof(CustomGridSplitter),
        //        new UIPropertyMetadata(true));
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGridSplitter" /> class.
        /// </summary>
        public CustomGridSplitter()
        {
            this.DefaultStyleKey = typeof(CustomGridSplitter);
            this.IsTabStop = true;
            this.DetermineResizeCursor();
            this.LayoutUpdated += OnLayoutUpdated;
            this.UpdateVisualState();
        }
        #endregion

        /// <summary>
        /// Called before the GotFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            UpdateVisualState();
        }

        /// <summary>
        /// Called before the LostFocus event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            UpdateVisualState();
        }

        /// <summary>
        /// Called when the LayoutUpdated event occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLayoutUpdated(object sender, object e)
        {
            UpdateVisualState();
        }

        #region UpdateVisualState()
        private void UpdateVisualState()
        {
            var resizeDirection = DetermineEffectiveResizeDirection();

            if (resizeDirection == GridResizeDirection.Columns)
            {
                VisualStateManager.GoToState(this, VerticalOrientationStateName, true);
            }
            else
            {
                VisualStateManager.GoToState(this, HorizontalOrientationStateName, true);
            }

            if (this.FocusState == Microsoft.UI.Xaml.FocusState.Unfocused)
            {
                VisualStateManager.GoToState(this, UnfocusedStateName, true);
            }
            else
            {
                VisualStateManager.GoToState(this, FocusedStateName, true);
            }
        }
        #endregion

        #region Pointer event handlers
        /// <summary>
        /// Called before the PointerEntered event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerEntered(Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.DetermineResizeCursor();
        }

        private uint? _dragPointer;

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_dragPointer != null)
                return;

            _dragPointer = e.Pointer.PointerId;
            _effectiveResizeDirection = this.DetermineEffectiveResizeDirection();
            _parentGrid = GetGrid();
            _previewDraggingStartPosition = e.GetCurrentPoint(_parentGrid).Position;
            _lastPosition = _previewDraggingStartPosition;
            _isDragging = true;

            if (ShowsPreview)
            {
                //this.Dispatcher.Invoke(
                //    CoreDispatcherPriority.High,
                //    (s, e2) => StartPreviewDragging(e),
                //    this,
                //    null);
                StartPreviewDragging(e);
            }
            else
                StartDirectDragging(e);
        }

        private void StartPreviewDragging(Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _isDraggingPreview = true;
            _previewPopup = new Popup
            {
                Width = _parentGrid.ActualWidth,
                Height = _parentGrid.ActualHeight
            };

            _previewPopup.IsOpen = true;
            _previewPopupHostGrid = new Grid
            {
                VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch,
                HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch
            };

            _parentGrid.Children.Add(_previewPopupHostGrid);
            if (_parentGrid.RowDefinitions.Count > 0)
                Grid.SetRowSpan(_previewPopupHostGrid, _parentGrid.RowDefinitions.Count);
            if (_parentGrid.ColumnDefinitions.Count > 0)
                Grid.SetColumnSpan(_previewPopupHostGrid, _parentGrid.ColumnDefinitions.Count);
            _previewPopupHostGrid.Children.Add(_previewPopup);

            _previewGrid = new Grid
            {
                Width = _parentGrid.ActualWidth,
                Height = _parentGrid.ActualHeight
            };

            _previewPopup.Child = _previewGrid;

            foreach (var definition in _parentGrid.RowDefinitions)
            {
                var definitionCopy = new RowDefinition
                {
                    Height = definition.Height,
                    MaxHeight = definition.MaxHeight,
                    MinHeight = definition.MinHeight
                };

                _previewGrid.RowDefinitions.Add(definitionCopy);
            }

            foreach (var definition in _parentGrid.ColumnDefinitions)
            {
                var w = definition.Width;
                var mxw = definition.MaxWidth;
                var mnw = definition.MinWidth;

                var definitionCopy = new ColumnDefinition();

                definitionCopy.Width = w;
                definition.MinWidth = mnw;
                if (!double.IsInfinity(definition.MaxWidth))
                {
                    definition.MaxWidth = mxw;
                }
                //{
                //    Width = definition.Width,
                //    MaxWidth = definition.MaxWidth,
                //    MinWidth = definition.MinWidth
                //};

                _previewGrid.ColumnDefinitions.Add(definitionCopy);
            }

            _previewGridSplitter = new CustomGridSplitter
            {
                Opacity = 0.0,
                ShowsPreview = false,
                Width = this.Width,
                Height = this.Height,
                Margin = this.Margin,
                VerticalAlignment = this.VerticalAlignment,
                HorizontalAlignment = this.HorizontalAlignment,
                ResizeBehavior = this.ResizeBehavior,
                ResizeDirection = this.ResizeDirection,
                KeyboardIncrement = this.KeyboardIncrement
            };

            Grid.SetColumn(_previewGridSplitter, Grid.GetColumn(this));
            var cs = Grid.GetColumnSpan(this);
            if (cs > 0)
                Grid.SetColumnSpan(_previewGridSplitter, cs);
            Grid.SetRow(_previewGridSplitter, Grid.GetRow(this));
            var rs = Grid.GetRowSpan(this);
            if (rs > 0)
                Grid.SetRowSpan(_previewGridSplitter, rs);
            _previewGrid.Children.Add(_previewGridSplitter);

            _previewControlBorder = new Border
            {
                Width = this.Width,
                Height = this.Height,
                Margin = this.Margin,
                VerticalAlignment = this.VerticalAlignment,
                HorizontalAlignment = this.HorizontalAlignment,
            };

            Grid.SetColumn(_previewControlBorder, Grid.GetColumn(this));
            if (cs > 0)
                Grid.SetColumnSpan(_previewControlBorder, cs);
            Grid.SetRow(_previewControlBorder, Grid.GetRow(this));
            if (rs > 0)
                Grid.SetRowSpan(_previewControlBorder, rs);
            _previewGrid.Children.Add(_previewControlBorder);

            _previewControl = new GridSplitterPreviewControl();
            if (this.PreviewStyle != null)
                _previewControl.Style = this.PreviewStyle;
            _previewControlBorder.Child = _previewControl;

            _previewPopup.Child = _previewGrid;
            //await this.previewGridSplitter.WaitForLoadedAsync();

            //this.previewGridSplitter.OnPointerPressed(e);
            _previewGridSplitter._dragPointer = e.Pointer.PointerId;
            _previewGridSplitter._effectiveResizeDirection = this.DetermineEffectiveResizeDirection();
            _previewGridSplitter._parentGrid = _previewGrid;
            _previewGridSplitter._lastPosition = e.GetCurrentPoint(_previewGrid).Position;
            _previewGridSplitter._isDragging = true;
            _previewGridSplitter.StartDirectDragging(e);
            _previewGridSplitter.DraggingCompleted += PreviewGridSplitter_DraggingCompleted;
        }

        private void PreviewGridSplitter_DraggingCompleted(object sender, EventArgs e)
        {
            for (int i = 0; i < _previewGrid.RowDefinitions.Count; i++)
            {
                _parentGrid.RowDefinitions[i].Height =
                    _previewGrid.RowDefinitions[i].Height;
            }

            for (int i = 0; i < _previewGrid.ColumnDefinitions.Count; i++)
            {
                _parentGrid.ColumnDefinitions[i].Width =
                    _previewGrid.ColumnDefinitions[i].Width;
            }

            _previewGridSplitter.DraggingCompleted -= PreviewGridSplitter_DraggingCompleted;
            _parentGrid.Children.Remove(_previewPopupHostGrid);

            _isDragging = false;
            _isDraggingPreview = false;
            _dragPointer = null;
            _parentGrid = null;
            if (this.DraggingCompleted != null)
                this.DraggingCompleted(this, EventArgs.Empty);
        }

        private void StartDirectDragging(Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _isDraggingPreview = false;
            this.CapturePointer(e.Pointer);
            this.Focus(FocusState.Pointer);
        }

        /// <summary>
        /// Called before the PointerMoved event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerMoved(Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!_isDragging ||
                _dragPointer != e.Pointer.PointerId)
            {
                return;
            }

            var position = e.GetCurrentPoint(_parentGrid).Position;

            if (_isDraggingPreview)
                ContinuePreviewDragging(position);
            else
                ContinueDirectDragging(position);

            _lastPosition = position;
        }

        private void ContinuePreviewDragging(Point position)
        {
        }

        private void ContinueDirectDragging(Point position)
        {
            if (_effectiveResizeDirection == GridResizeDirection.Columns)
            {
                var deltaX = position.X - _lastPosition.X;
                this.ResizeColumns(_parentGrid, deltaX);
            }
            else
            {
                var deltaY = position.Y - _lastPosition.Y;
                this.ResizeRows(_parentGrid, deltaY);
            }
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!_isDragging ||
                _dragPointer != e.Pointer.PointerId)
            {
                return;
            }

            this.ReleasePointerCapture(e.Pointer);
            _isDragging = false;
            _isDraggingPreview = false;
            _dragPointer = null;
            _parentGrid = null;
            if (this.DraggingCompleted != null)
                this.DraggingCompleted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            GridResizeDirection effectiveResizeDirection =
                this.DetermineEffectiveResizeDirection();

            if (effectiveResizeDirection == GridResizeDirection.Columns)
            {
                if (e.Key == VirtualKey.Left)
                {
                    this.ResizeColumns(this.GetGrid(), -KeyboardIncrement);
                    e.Handled = true;
                }
                else if (e.Key == VirtualKey.Right)
                {
                    this.ResizeColumns(this.GetGrid(), KeyboardIncrement);
                    e.Handled = true;
                }
            }
            else
            {
                if (e.Key == VirtualKey.Up)
                {
                    this.ResizeRows(this.GetGrid(), -KeyboardIncrement);
                    e.Handled = true;
                }
                else if (e.Key == VirtualKey.Down)
                {
                    this.ResizeRows(this.GetGrid(), KeyboardIncrement);
                    e.Handled = true;
                }
            }
        }
        #endregion

        #region ResizeColumns()
        private void ResizeColumns(Grid grid, double deltaX)
        {
            GridResizeBehavior effectiveGridResizeBehavior =
                this.DetermineEffectiveResizeBehavior();

            int column = Grid.GetColumn(this);
            int leftColumn;
            int rightColumn;

            switch (effectiveGridResizeBehavior)
            {
                case GridResizeBehavior.PreviousAndCurrent:
                    leftColumn = column - 1;
                    rightColumn = column;
                    break;
                case GridResizeBehavior.PreviousAndNext:
                    leftColumn = column - 1;
                    rightColumn = column + 1;
                    break;
                default:
                    leftColumn = column;
                    rightColumn = column + 1;
                    break;
            }

            if (rightColumn >= grid.ColumnDefinitions.Count)
            {
                return;
            }

            var leftColumnDefinition = grid.ColumnDefinitions[leftColumn];
            var rightColumnDefinition = grid.ColumnDefinitions[rightColumn];
            var leftColumnGridUnitType = leftColumnDefinition.Width.GridUnitType;
            var rightColumnGridUnitType = rightColumnDefinition.Width.GridUnitType;
            var leftColumnActualWidth = leftColumnDefinition.ActualWidth;
            var rightColumnActualWidth = rightColumnDefinition.ActualWidth;
            var leftColumnMaxWidth = leftColumnDefinition.MaxWidth;
            var rightColumnMaxWidth = rightColumnDefinition.MaxWidth;
            var leftColumnMinWidth = leftColumnDefinition.MinWidth;
            var rightColumnMinWidth = rightColumnDefinition.MinWidth;

            //deltaX = 200;
            if (leftColumnActualWidth + deltaX > leftColumnMaxWidth)
            {
                deltaX = Math.Max(
                    0,
                    leftColumnDefinition.MaxWidth - leftColumnActualWidth);
            }

            if (leftColumnActualWidth + deltaX < leftColumnMinWidth)
            {
                deltaX = Math.Min(
                    0,
                    leftColumnDefinition.MinWidth - leftColumnActualWidth);
            }

            if (rightColumnActualWidth - deltaX > rightColumnMaxWidth)
            {
                deltaX = -Math.Max(
                    0,
                    rightColumnDefinition.MaxWidth - rightColumnActualWidth);
            }

            if (rightColumnActualWidth - deltaX < rightColumnMinWidth)
            {
                deltaX = -Math.Min(
                    0,
                    rightColumnDefinition.MinWidth - rightColumnActualWidth);
            }

            var newLeftColumnActualWidth = leftColumnActualWidth + deltaX;
            var newRightColumnActualWidth = rightColumnActualWidth - deltaX;

            //grid.BeginInit();

            double totalStarColumnsWidth = 0;
            double starColumnsAvailableWidth = grid.ActualWidth;

            if (leftColumnGridUnitType ==
                    GridUnitType.Star ||
                rightColumnGridUnitType ==
                    GridUnitType.Star)
            {
                foreach (var columnDefinition in grid.ColumnDefinitions)
                {
                    if (columnDefinition.Width.GridUnitType ==
                        GridUnitType.Star)
                    {
                        totalStarColumnsWidth +=
                            columnDefinition.Width.Value;
                    }
                    else
                    {
                        starColumnsAvailableWidth -=
                            columnDefinition.ActualWidth;
                    }
                }
            }

            if (leftColumnGridUnitType == GridUnitType.Star)
            {
                if (rightColumnGridUnitType == GridUnitType.Star)
                {
                    // If both columns are star columns
                    // - totalStarColumnsWidth won't change and
                    // as much as one of the columns grows
                    // - the other column will shrink by the same value.

                    // If there is no width available to star columns
                    // - we can't resize two of them.
                    if (starColumnsAvailableWidth < 1)
                    {
                        return;
                    }

                    var oldStarWidth = leftColumnDefinition.Width.Value;
                    var newStarWidth = Math.Max(
                        0,
                        totalStarColumnsWidth * newLeftColumnActualWidth /
                            starColumnsAvailableWidth);
                    leftColumnDefinition.Width =
                        new GridLength(newStarWidth, GridUnitType.Star);

                    rightColumnDefinition.Width =
                        new GridLength(
                            Math.Max(
                                0,
                                rightColumnDefinition.Width.Value -
                                    newStarWidth + oldStarWidth),
                            GridUnitType.Star);
                }
                else
                {
                    var newStarColumnsAvailableWidth =
                        starColumnsAvailableWidth +
                        rightColumnActualWidth -
                        newRightColumnActualWidth;

                    if (newStarColumnsAvailableWidth - newLeftColumnActualWidth >= 1)
                    {
                        var newStarWidth = Math.Max(
                            0,
                            (totalStarColumnsWidth -
                             leftColumnDefinition.Width.Value) *
                            newLeftColumnActualWidth /
                            (newStarColumnsAvailableWidth - newLeftColumnActualWidth));

                        leftColumnDefinition.Width =
                            new GridLength(newStarWidth, GridUnitType.Star);
                    }
                }
            }
            else
            {
                leftColumnDefinition.Width =
                    new GridLength(
                        newLeftColumnActualWidth, GridUnitType.Pixel);
            }

            if (rightColumnGridUnitType ==
                GridUnitType.Star)
            {
                if (leftColumnGridUnitType !=
                    GridUnitType.Star)
                {
                    var newStarColumnsAvailableWidth =
                        starColumnsAvailableWidth +
                        leftColumnActualWidth -
                        newLeftColumnActualWidth;

                    if (newStarColumnsAvailableWidth - newRightColumnActualWidth >= 1)
                    {
                        var newStarWidth = Math.Max(
                            0,
                            (totalStarColumnsWidth -
                             rightColumnDefinition.Width.Value) *
                            newRightColumnActualWidth /
                            (newStarColumnsAvailableWidth - newRightColumnActualWidth));
                        rightColumnDefinition.Width =
                            new GridLength(newStarWidth, GridUnitType.Star);
                    }
                }
                // else handled in the left column width calculation block
            }
            else
            {
                rightColumnDefinition.Width =
                    new GridLength(
                        newRightColumnActualWidth, GridUnitType.Pixel);
            }

            //grid.EndInit();
        }
        #endregion

        #region ResizeRows()
        private void ResizeRows(Grid grid, double deltaX)
        {
            GridResizeBehavior effectiveGridResizeBehavior =
                this.DetermineEffectiveResizeBehavior();

            int row = Grid.GetRow(this);
            int upperRow;
            int lowerRow;

            switch (effectiveGridResizeBehavior)
            {
                case GridResizeBehavior.PreviousAndCurrent:
                    upperRow = row - 1;
                    lowerRow = row;
                    break;
                case GridResizeBehavior.PreviousAndNext:
                    upperRow = row - 1;
                    lowerRow = row + 1;
                    break;
                default:
                    upperRow = row;
                    lowerRow = row + 1;
                    break;
            }

            if (lowerRow >= grid.RowDefinitions.Count)
            {
                return;
            }

            var upperRowDefinition = grid.RowDefinitions[upperRow];
            var lowerRowDefinition = grid.RowDefinitions[lowerRow];
            var upperRowGridUnitType = upperRowDefinition.Height.GridUnitType;
            var lowerRowGridUnitType = lowerRowDefinition.Height.GridUnitType;
            var upperRowActualHeight = upperRowDefinition.ActualHeight;
            var lowerRowActualHeight = lowerRowDefinition.ActualHeight;
            var upperRowMaxHeight = upperRowDefinition.MaxHeight;
            var lowerRowMaxHeight = lowerRowDefinition.MaxHeight;
            var upperRowMinHeight = upperRowDefinition.MinHeight;
            var lowerRowMinHeight = lowerRowDefinition.MinHeight;

            //deltaX = 200;
            if (upperRowActualHeight + deltaX > upperRowMaxHeight)
            {
                deltaX = Math.Max(
                    0,
                    upperRowDefinition.MaxHeight - upperRowActualHeight);
            }

            if (upperRowActualHeight + deltaX < upperRowMinHeight)
            {
                deltaX = Math.Min(
                    0,
                    upperRowDefinition.MinHeight - upperRowActualHeight);
            }

            if (lowerRowActualHeight - deltaX > lowerRowMaxHeight)
            {
                deltaX = -Math.Max(
                    0,
                    lowerRowDefinition.MaxHeight - lowerRowActualHeight);
            }

            if (lowerRowActualHeight - deltaX < lowerRowMinHeight)
            {
                deltaX = -Math.Min(
                    0,
                    lowerRowDefinition.MinHeight - lowerRowActualHeight);
            }

            var newUpperRowActualHeight = upperRowActualHeight + deltaX;
            var newLowerRowActualHeight = lowerRowActualHeight - deltaX;

            //grid.BeginInit();

            double totalStarRowsHeight = 0;
            double starRowsAvailableHeight = grid.ActualHeight;

            if (upperRowGridUnitType ==
                    GridUnitType.Star ||
                lowerRowGridUnitType ==
                    GridUnitType.Star)
            {
                foreach (var rowDefinition in grid.RowDefinitions)
                {
                    if (rowDefinition.Height.GridUnitType ==
                        GridUnitType.Star)
                    {
                        totalStarRowsHeight +=
                            rowDefinition.Height.Value;
                    }
                    else
                    {
                        starRowsAvailableHeight -=
                            rowDefinition.ActualHeight;
                    }
                }
            }

            if (upperRowGridUnitType == GridUnitType.Star)
            {
                if (lowerRowGridUnitType == GridUnitType.Star)
                {
                    // If both rows are star rows
                    // - totalStarRowsHeight won't change and
                    // as much as one of the rows grows
                    // - the other row will shrink by the same value.

                    // If there is no width available to star rows
                    // - we can't resize two of them.
                    if (starRowsAvailableHeight < 1)
                    {
                        return;
                    }

                    var oldStarHeight = upperRowDefinition.Height.Value;
                    var newStarHeight = Math.Max(
                        0,
                        totalStarRowsHeight * newUpperRowActualHeight /
                            starRowsAvailableHeight);
                    upperRowDefinition.Height =
                        new GridLength(newStarHeight, GridUnitType.Star);

                    lowerRowDefinition.Height =
                        new GridLength(
                            Math.Max(
                                0,
                                lowerRowDefinition.Height.Value -
                                    newStarHeight + oldStarHeight),
                            GridUnitType.Star);
                }
                else
                {
                    var newStarRowsAvailableHeight =
                        starRowsAvailableHeight +
                        lowerRowActualHeight -
                        newLowerRowActualHeight;

                    if (newStarRowsAvailableHeight - newUpperRowActualHeight >= 1)
                    {
                        var newStarHeight = Math.Max(
                            0,
                            (totalStarRowsHeight -
                             upperRowDefinition.Height.Value) *
                            newUpperRowActualHeight /
                            (newStarRowsAvailableHeight - newUpperRowActualHeight));

                        upperRowDefinition.Height =
                            new GridLength(newStarHeight, GridUnitType.Star);
                    }
                }
            }
            else
            {
                upperRowDefinition.Height =
                    new GridLength(
                        newUpperRowActualHeight, GridUnitType.Pixel);
            }

            if (lowerRowGridUnitType ==
                GridUnitType.Star)
            {
                if (upperRowGridUnitType !=
                    GridUnitType.Star)
                {
                    var newStarRowsAvailableHeight =
                        starRowsAvailableHeight +
                        upperRowActualHeight -
                        newUpperRowActualHeight;

                    if (newStarRowsAvailableHeight - newLowerRowActualHeight >= 1)
                    {
                        var newStarHeight = Math.Max(
                            0,
                            (totalStarRowsHeight -
                             lowerRowDefinition.Height.Value) *
                            newLowerRowActualHeight /
                            (newStarRowsAvailableHeight - newLowerRowActualHeight));
                        lowerRowDefinition.Height =
                            new GridLength(newStarHeight, GridUnitType.Star);
                    }
                }
                // else handled in the upper row width calculation block
            }
            else
            {
                lowerRowDefinition.Height =
                    new GridLength(
                        newLowerRowActualHeight, GridUnitType.Pixel);
            }

            //grid.EndInit();
        }
        #endregion

        #region GetGrid()
        private Grid GetGrid()
        {
            var grid = this.Parent as Grid;

            if (grid == null)
            {
                throw new InvalidOperationException(
                    "CustomGridSplitter only works when hosted in a Grid.");
            }
            return grid;
        }
        #endregion
    }

    /// <summary>
    /// A primitive control used for representing a preview of the manipulated CustomGridSplitter
    /// </summary>
    public class GridSplitterPreviewControl : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridSplitterPreviewControl" /> class.
        /// </summary>
        public GridSplitterPreviewControl()
        {
            this.DefaultStyleKey = typeof(GridSplitterPreviewControl);
        }
    }
}
