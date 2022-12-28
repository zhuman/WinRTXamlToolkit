using System;
using Windows.UI.Popups;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls;

namespace WinRTXamlToolkit.Sample.Views
{
    public sealed partial class InputDialogTestView : UserControl
    {
        public InputDialogTestView()
        {
            this.InitializeComponent();
        }

        private async void RegularStyleTest(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog();
            var result = await dialog.ShowAsync("This is the title", "This is the content/message", "Option 1", "Option 2", "Option 3");
            var content =
                string.Format(
                    "Text: {0}, Button: {1}",
                    dialog.InputText ?? "",
                    result ?? "");
            await new MessageDialog(content, "Result").ShowAsync();
        }

        private async void CustomStyleTest(object sender, RoutedEventArgs e)
        {
            var dialog = new InputDialog();
            dialog.AcceptButton = "Option 3 (Accept)";
            dialog.CancelButton = "Option 2 (Cancel)";
            // For some reason it has to be defined here - does not work when set using a style setter.
            dialog.ButtonsPanelOrientation = Orientation.Vertical;
            dialog.Style = (Style)this.Resources["CustomInputDialogStyle"];
            var result = await dialog.ShowAsync("This is the title", "This is the content/message", "Option 1", "Option 2 (Cancel)", "Option 3 (Accept)");
            var content =
                string.Format(
                    "Text: {0}, Button: {1}",
                    dialog.InputText ?? "",
                    result ?? "");
#pragma warning disable 4014
            new MessageDialog(content, "Result").ShowAsync();
#pragma warning restore 4014
        }

        private async void GridHostedTest(object sender, RoutedEventArgs e)
        {
            await GridHostedDialog.ShowAsync(
                "Grid-hosted InputDialog",
                "This dialog is defined as a child of a Grid",
                "OK");
        }

        private async void ContentControlHostedTest(object sender, RoutedEventArgs e)
        {
            await ContentControlHostedDialog.ShowAsync(
                "ContentControl-hosted InputDialog",
                "This dialog is defined as Content of a ContentControl",
                "OK");
        }
    }
}
