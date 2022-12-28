using System;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;

namespace WinRTXamlToolkit.Sample.Views
{
    public sealed partial class WrapPanelTestView : UserControl
    {
        Random _random = new Random();

        public WrapPanelTestView()
        {
            this.InitializeComponent();
        }

        private void OnTestButtonClick(object sender, RoutedEventArgs e)
        {
            wrapGrid.Children.Clear();
            wrapPanel.Children.Clear();

            for (int i = 0; i < 10; i++)
            {
                var w = _random.Next(50, 250);
                var h = _random.Next(50, 250);
                var c = Color.FromArgb(
                    (byte)_random.Next(125, 256),
                    (byte)_random.Next(256),
                    (byte)_random.Next(256),
                    (byte)_random.Next(256));
                wrapGrid.Children.Add(new Rectangle
                {
                    Width = w,
                    Height = h,
                    Fill = new SolidColorBrush(c)
                });
                wrapPanel.Children.Add(new Rectangle
                {
                    Width = w,
                    Height = h,
                    Fill = new SolidColorBrush(c)
                });
            }
        }

        private void OnRbWrapPanelOrientationHorizontalChecked(object sender, RoutedEventArgs e)
        {
            if (wrapPanel != null)
                wrapPanel.Orientation = Orientation.Horizontal;
        }

        private void OnRbWrapPanelOrientationVerticalChecked(object sender, RoutedEventArgs e)
        {
            if (wrapPanel != null)
                wrapPanel.Orientation = Orientation.Vertical;
        }

        private void OnRbWrapGridOrientationHorizontalChecked(object sender, RoutedEventArgs e)
        {
            if (wrapGrid != null)
                wrapGrid.Orientation = Orientation.Horizontal;
        }

        private void OnRbWrapGridOrientationVerticalChecked(object sender, RoutedEventArgs e)
        {
            if (wrapGrid != null)
                wrapGrid.Orientation = Orientation.Vertical;
        }
    }
}
