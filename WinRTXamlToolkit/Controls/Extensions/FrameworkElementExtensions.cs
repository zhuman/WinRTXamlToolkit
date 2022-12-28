using System.ComponentModel;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Input;

namespace WinRTXamlToolkit.Controls.Extensions
{
    /// <summary>
    /// Extensions for the FrameworkElement class.
    /// </summary>
    public static class FrameworkElementExtensions
    {
        #region ClipToBounds
        /// <summary>
        /// ClipToBounds Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ClipToBoundsProperty =
            DependencyProperty.RegisterAttached(
                "ClipToBounds",
                typeof(bool),
                typeof(FrameworkElementExtensions),
                new PropertyMetadata(false, OnClipToBoundsChanged));

        /// <summary>
        /// Gets the ClipToBounds property. This dependency property 
        /// indicates whether the element should be clipped to its bounds.
        /// </summary>
        public static bool GetClipToBounds(DependencyObject d)
        {
            return (bool)d.GetValue(ClipToBoundsProperty);
        }

        /// <summary>
        /// Sets the ClipToBounds property. This dependency property 
        /// indicates whether the element should be clipped to its bounds.
        /// </summary>
        public static void SetClipToBounds(DependencyObject d, bool value)
        {
            d.SetValue(ClipToBoundsProperty, value);
        }

        /// <summary>
        /// Handles changes to the ClipToBounds property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnClipToBoundsChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool oldClipToBounds = (bool)e.OldValue;
            bool newClipToBounds = (bool)d.GetValue(ClipToBoundsProperty);

            if (newClipToBounds)
                SetClipToBoundsHandler(d, new ClipToBoundsHandler());
            else
                SetClipToBoundsHandler(d, null);
        }
        #endregion

        #region ClipToBoundsHandler
        /// <summary>
        /// ClipToBoundsHandler Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ClipToBoundsHandlerProperty =
            DependencyProperty.RegisterAttached(
                "ClipToBoundsHandler",
                typeof(ClipToBoundsHandler),
                typeof(FrameworkElementExtensions),
                new PropertyMetadata(null, OnClipToBoundsHandlerChanged));

        /// <summary>
        /// Gets the ClipToBoundsHandler property. This dependency property 
        /// indicates the handler that handles the updates to the clipping geometry when ClipToBounds is set to true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ClipToBoundsHandler GetClipToBoundsHandler(DependencyObject d)
        {
            return (ClipToBoundsHandler)d.GetValue(ClipToBoundsHandlerProperty);
        }

        /// <summary>
        /// Sets the ClipToBoundsHandler property. This dependency property 
        /// indicates the handler that handles the updates to the clipping geometry when ClipToBounds is set to true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetClipToBoundsHandler(DependencyObject d, ClipToBoundsHandler value)
        {
            d.SetValue(ClipToBoundsHandlerProperty, value);
        }

        /// <summary>
        /// Handles changes to the ClipToBoundsHandler property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which
        /// the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnClipToBoundsHandlerChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ClipToBoundsHandler oldClipToBoundsHandler = (ClipToBoundsHandler)e.OldValue;
            ClipToBoundsHandler newClipToBoundsHandler = (ClipToBoundsHandler)d.GetValue(ClipToBoundsHandlerProperty);

            if (oldClipToBoundsHandler != null)
                oldClipToBoundsHandler.Detach();
            if (newClipToBoundsHandler != null)
                newClipToBoundsHandler.Attach((FrameworkElement)d);
        }
        #endregion

        #region HasNonDefaultValue()
        /// <summary>
        /// Determines whether the element has a non default value of the specified dependency property.
        /// </summary>
        /// <param name="this">The this.</param>
        /// <param name="dp">The dp.</param>
        /// <returns>
        ///   <c>true</c> if the element has a non default value of the specified dependency property; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasNonDefaultValue(
            this DependencyObject @this,
            DependencyProperty dp)
        {
            var localValue = @this.ReadLocalValue(dp);
            return localValue != DependencyProperty.UnsetValue;
        } 
        #endregion
    }

    #region ClipToBoundsHandler
    /// <summary>
    /// Handles the ClipToBounds attached behavior defined by the attached property
    /// of the <see cref="FrameworkElementExtensions"/> class.
    /// </summary>
    public class ClipToBoundsHandler
    {
        private FrameworkElement _fe;

        /// <summary>
        /// Attaches to the specified framework element.
        /// </summary>
        /// <param name="fe">The fe.</param>
        public void Attach(FrameworkElement fe)
        {
            _fe = fe;
            UpdateClipGeometry();
            fe.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            if (_fe == null)
                return;

            UpdateClipGeometry();
        }

        private void UpdateClipGeometry()
        {
            _fe.Clip =
                new RectangleGeometry
                {
                    Rect = new Rect(0, 0, _fe.ActualWidth, _fe.ActualHeight)
                };
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach()
        {
            if (_fe == null)
                return;

            _fe.SizeChanged -= OnSizeChanged;
            _fe = null;
        }
    } 
    #endregion
}
