using System;
using System.Collections.Generic;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinRTXamlToolkit.Controls.Extensions;

namespace WinRTXamlToolkit.Sample.Views
{
    public sealed partial class ScrollViewerExtensionsTestView : UserControl
    {
        private Random r = new Random();

        public ScrollViewerExtensionsTestView()
        {
            this.InitializeComponent();
            var items = new List<dynamic>();

            // For anonymous type binding info check this article:
            // http://timheuer.com/blog/archive/2012/04/10/anonymous-type-binding-metro-style-app.aspx
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    items.Add(new
                              {
                                  Row = j,
                                  Column = i,
                                  Brush = new SolidColorBrush(Color.FromArgb(255, (byte)(i * 63), 255, (byte)(j * 63))),
                                  Text = (i * 5 + j).ToString()
                              });
                }

            foreach (var item in items)
            {
                var itemPresenter =
                    (FrameworkElement)((DataTemplate)this.Resources["TestItemTemplate"]).LoadContent();
                itemPresenter.DataContext = item;
                Grid.SetColumn(itemPresenter, item.Column);
                Grid.SetRow(itemPresenter, item.Row);
                scrolledGrid.Children.Add(itemPresenter);
            }
        }

        private void OnAnimatedScrollTestButtonClick(object sender, RoutedEventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            scrollViewer.ScrollToVerticalOffsetWithAnimationAsync(r.NextDouble() * (scrollViewer.ExtentHeight - scrollViewer.ViewportHeight));
            scrollViewer.ScrollToHorizontalOffsetWithAnimationAsync(r.NextDouble() * (scrollViewer.ExtentWidth - scrollViewer.ViewportWidth));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //scrollViewer.ScrollToHorizontalOffsetWithAnimationAsync(500 - scrollViewer.HorizontalOffset);
            //scrollViewer.ScrollToVerticalOffsetWithAnimationAsync(500 - scrollViewer.VerticalOffset);
        }

        private async void OnAnimatedZoomTestButtonClick(object sender, RoutedEventArgs e)
        {
            await scrollViewer.ZoomToFactorWithAnimationAsync((float)
                (r.NextDouble() * (scrollViewer.MaxZoomFactor - scrollViewer.MinZoomFactor) + scrollViewer.MinZoomFactor));
        }
    }
}
